using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour {

    private float mouseSensitivity = 1f;

    public CharacterController controller;
    public float playerSpeed = 12f;
    public readonly float walkingSpeed = 12f;
    public readonly float crouchSpeed = 7f;
    private const float playerHeight = 3.8f;
    public readonly float crouchHeight = 2.6f;

    public float playerMass = 80f;

    public float gravityDownForce = 20f;
    public float movementSharpnessOnGround = 20f;
    public float maxSpeedOnGround = 10f;
    public Vector3 characterVelocity;
    public float jumpForce = 9f;
    public float accelerationSpeedInAir = 12f;
    public float maxSpeedInAir = 20f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public bool isGrounded;
    public bool isCrouch;

    public Vector3 impactVector;

    private Vector3 initialPlayerPosition;

    private float cameraVerticalAngle;
    public float characterVelocityY;
    public Vector3 characterVelocityMomentum;
    private Camera playerCamera;


    public Transform gun;
    private HookshotGun hookshot;

    public State state;

    public enum State {
        Normal,
        HookshotThrown,
        HookshotFlyingPlayer,
    }

    private void Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 165;
        Cursor.lockState = CursorLockMode.Locked;
        initialPlayerPosition = transform.position;       
    }

    private void Awake() {
        controller = GetComponent<CharacterController>();
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        //cameraFov = playerCamera.GetComponent<CameraFov>();
        //speedLinesParticleSystem = transform.Find("Camera").Find("SpeedLinesParticleSystem").GetComponent<ParticleSystem>();
        Cursor.lockState = CursorLockMode.Locked;
        state = State.Normal;
        hookshot = gun.GetComponent<HookshotGun>();
    }

    void Update() {
        GroundCheck();
        switch (state) {
            default:
            case State.Normal:
                HandleCharacterLook();
                HandleCharacterMovement();
                hookshot.HandleHookshotStart();
                break;
            case State.HookshotThrown:
                hookshot.HandleHookshotThrow();
                HandleCharacterLook();
                HandleCharacterMovement();
                break;
            case State.HookshotFlyingPlayer:
                isGrounded = false;
                HandleCharacterLook();
                hookshot.HandleHookshotMovement();
                HandleCharacterMovement();
                break;
        }
    }

    private void LateUpdate() {
        switch (state) {
            case State.HookshotThrown:
                hookshot.DrawRope();
                break;
            case State.HookshotFlyingPlayer:
                hookshot.DrawRope();
                break;
        }
    }

    void GroundCheck() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && characterVelocity.y < 0) {
            characterVelocity.y = -2f;
        }
    }

    public Vector3 GetMoveInput() {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        move = Vector3.ClampMagnitude(move, 1);
        return move;
    }

    void HandleCharacterMovement() {
        Vector3 worldspaceMoveInput = transform.TransformVector(GetMoveInput());

        if (isGrounded) {
            Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround;
            characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);

            if (isGrounded && Input.GetButtonDown("Jump")) {
                //cancel out downward force
                characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);

                characterVelocity += Vector3.up * jumpForce;
                isGrounded = false;
            }

            if (Input.GetButtonDown("Crouch") && !isCrouch) {
                controller.height = crouchHeight;
                isCrouch = true;
                maxSpeedOnGround = crouchSpeed;
                //Debug.Log("Crouched");
            }
            if (Input.GetButtonUp("Crouch")) {
                controller.height = playerHeight;
                isCrouch = false;
                maxSpeedOnGround = walkingSpeed;
                // Debug.Log("Standing");
            }
        }
        else {
            //add air acceleration
            characterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;

            //limit horizontal air speed
            float verticalVelocity = characterVelocity.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir);

            //apply gravity
            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
            //characterVelocity += horizontalVelocity * Time.deltaTime;
        }

        //move character
        //characterVelocity += impactVector;
        controller.Move(characterVelocity * Time.deltaTime);
    }

    public void HandleCharacterLook() {
        float lookX = Input.GetAxisRaw("Mouse X");
        float lookY = Input.GetAxisRaw("Mouse Y");

        // Rotate the transform with the input speed around its local Y axis
        transform.Rotate(new Vector3(0f, lookX * mouseSensitivity, 0f), Space.Self);

        // Add vertical inputs to the camera's vertical angle
        cameraVerticalAngle += lookY * mouseSensitivity;

        // Limit the camera's vertical angle to min/max
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);

        // Apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
    }

    public void Knockback(float force, Vector3 direction) {
        characterVelocity -= force * direction / playerMass;
        //controller.Move(characterVelocity * Time.deltaTime); //idk what this does and it makes me nervous 

    }

    public void Kill() {
        controller.enabled = false; //why do i 
        transform.position = initialPlayerPosition;
        controller.enabled = true; //have to do this 
    }

}
