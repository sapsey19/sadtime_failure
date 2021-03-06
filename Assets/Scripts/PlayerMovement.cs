using System;
using System.Collections;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    PlayerInputHandler m_InputHandler;

    //Assingables
    public Transform playerCam;
    public Transform orientation;

    //Other
    private Rigidbody rb;

    //Rotation and look
    private float xRotation;
   

    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public static bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f; //how fast you stop moving (friction) 
    private readonly float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.8f, 1); //old settings: new Vector3(1, 0.5f, 1); 
    private Vector3 playerScale;
    public float slideForce = 4000; //how much you slide when you crouch 
    public float slideCounterMovement = 0.1f; //slide friction 

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    private bool cancellingGrounded;

    //Input
    float x, y;
    public static bool jumping, sprinting, crouching;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    //Wallrunning
    private WallRun wallRunComponent;

    //grappling hook stuff
    private Camera playerCamera;
    public Transform gun;
    private HookshotGun hookshot;

    private const float gravityValue = 2000f; //1200f default 
    private float gravityMultiplier = gravityValue;

    public State state;

    public GameObject hook;

    private AudioSource jump;

    float m_CameraVerticalAngle = 0f;
   

    public enum State {
        Normal,
        HookshotThrown,
        HookshotFlyingPlayer,
    }

    void Awake() {
        rb = GetComponent<Rigidbody>();

        playerCamera = playerCam.Find("Player Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        state = State.Normal;
        hookshot = gun.GetComponent<HookshotGun>();
        jump = GetComponent<AudioSource>();
    }

    void Start() {
        m_InputHandler = GetComponent<PlayerInputHandler>();
        Application.targetFrameRate = 144;
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        wallRunComponent = GetComponent<WallRun>();
        //Time.timeScale /= 4;
    }


    private void FixedUpdate() {
        switch (state) {
            default:
            case State.Normal:
                Movement();
                break;
            case State.HookshotThrown:
                Movement();
                break;
            case State.HookshotFlyingPlayer:
                hookshot.HandleHookshotMovement();
                break;
        }
        
    }

    private void Update() {
        MyInput();
        //Debug.Log(wallRunComponent.IsWallRunning());
        //CheckForWall();
        switch (state) {
            default:
            case State.Normal:
                Look();
                if(hookshot.isActiveAndEnabled) //check to see if grapple gun is active weapon (should be a better way but i do not possess the brain cells to figure it out at the moment please forgive me) -> the beter way is making an input class 
                    hookshot.HandleHookshotStart();
                break;
            case State.HookshotThrown:
                hookshot.HandleHookshotThrow();
                Look();
                break;
            case State.HookshotFlyingPlayer:
                Look();
                break;
        }
    }

    private void LateUpdate() {
        if (state != State.Normal) {
            hookshot.DrawRope(); //draws rope after physics update 
        }
    }

    // Find user input. Now in it's own class :) 
    private void MyInput() {
        x = m_InputHandler.GetMoveInput().x; //Input.GetAxisRaw("Horizontal");
        y = m_InputHandler.GetMoveInput().z; //Input.GetAxisRaw("Vertical");
        jumping = m_InputHandler.GetJumpInputHeld(); //Input.GetButton("Jump");
        crouching = m_InputHandler.GetCrouchInputDown(); //Input.GetKey(KeyCode.LeftControl);

        //Crouching
        //if (Input.GetKeyDown(KeyCode.LeftControl))
        if(m_InputHandler.GetCrouchInputDown())
        //if (crouching)
            StartCrouch();
        //if (Input.GetKeyUp(KeyCode.LeftControl))
        if (m_InputHandler.GetCrouchInputReleased())
            StopCrouch();
        if (m_InputHandler.GetFireInputReleased()) { // i don't like having grapple hook input in player move class... fix it 
            state = State.Normal; 
            hookshot.StopHookshot();
        }
    }

    private void StartCrouch() {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f) {
            if (grounded) {
                rb.AddForce(orientation.transform.forward * slideForce);
                Debug.Log("sliding");
            }
        }
    }

    private void StopCrouch() {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement() {
        //transform.localScale = crouchScale;
        //transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * gravityMultiplier);

        //Find actual velocity relative to where player is looking, used to add friction 
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement (friction)
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping)
            Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (grounded && crouching) {
            multiplierV = 2f;
        }

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV); //forward and back
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier); //side to side 
    }

    private void Jump() {
        if (grounded && readyToJump && !wallRunComponent.IsWallRunning()) {
            Debug.Log("should not be in here while wallrunning");
            readyToJump = false;
            

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //play jump audio
            jump.Play();

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(readyToJump && wallRunComponent.IsWallRunning()) {
            Debug.Log("should be in here while wallrunning");
            readyToJump = false;

            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);

            //rb.velocity += wallRunComponent.GetWallJumpDirection() * jumpForce;
            rb.AddForce(wallRunComponent.GetWallJumpDirection() * jumpForce);

            Invoke(nameof(ResetJump), jumpCooldown);

        }
    }

    private void ResetJump() {
        readyToJump = true;
    }

    private float desiredX;
    private void Look() {
        float mouseX = m_InputHandler.GetLookInputsHorizontal(); //Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = m_InputHandler.GetLookInputsVertical(); //Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation += mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, wallRunComponent.tilt); //wallRunComponent.tilt);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    //Find the velocity relative to where the player is looking
    //Useful for vectors calculations regarding movement and limiting movement
    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    //Handle ground detection
    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return; //???????????????????????????

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    //private void StopWallrun() {
    //    EnableGravity();
    //    readyToWallrun = false;
    //    //isWallRunning = false;
    //    //Invoke(nameof(ResetWallrun), wallrunCooldown);
    //}

    //private void CheckForWall() {
    //    //only allow wall running when not on ground 
    //    if (!grounded) {
    //        isWallRight = Physics.Raycast(transform.position, orientation.right, 1f, whatIsWall);
    //        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 1f, whatIsWall);
    //        if (isWallRight || isWallLeft) {
    //            readyToWallrun = true;
    //            isWallRunning = true;
    //            Wallrun();
    //        }
    //        //else StopWallrun();
    //    }
    //}

    //public bool hasWallJumped = false;

    //private void Wallrun() {
        
    //    if (isWallRunning) {
    //        DisableGravity();
    //        //hasWallJumped = true;
    //        //zero out Y velocity 
    //        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

    //        isWallRunning = true;
            

    //        if (rb.velocity.magnitude <= maxWallSpeed) {
    //            rb.AddForce(orientation.forward * wallrunForce * Time.deltaTime);

    //            //make player stick to wall
    //            if (isWallRight)
    //                rb.AddForce(orientation.right * wallrunForce  * Time.deltaTime);
    //            else
    //                rb.AddForce(-orientation.right * wallrunForce * Time.deltaTime);
    //        }
    //        if (rb.velocity.magnitude == 0) {
    //            StopWallrun();
    //            Debug.Log("magnitude 0");
    //        }
    //    }
    //}

    private void StopGrounded() {
        grounded = false;
    }

    //disables extra gravity added to rigibody
    public void DisableGravity () {
        rb.useGravity = false;
        gravityMultiplier = 0f;
    }

    public void EnableGravity() {
        rb.useGravity = true;
        gravityMultiplier = gravityValue;
    }

    public bool IsGrounded() {
        return grounded;
    }

    public void SetVelocity(Vector3 vel) {

    }
}