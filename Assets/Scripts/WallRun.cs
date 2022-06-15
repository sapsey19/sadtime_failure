//using UnityEngine;
//using System.Linq;
//using UnityEngine.Rendering;

//[RequireComponent(typeof(PlayerMovement))]
//public class WallRun : MonoBehaviour {
//    [SerializeField] Transform orientation;

//    //detection
//    [SerializeField] float wallDistance = 10f;
//    [SerializeField] float minimumJumpHight = 1.5f;

//    public LayerMask whatIsWall;

//    //wall running
//    public float wallRunJumpForce = 10f;

//    //camera 
//    [SerializeField] private Camera cam;
//    [SerializeField] private float fov, wallRunFov, wallRunFovTime, camTilt, camTiltTime; 
//    public float tilt { get; private set; }


//    private Rigidbody rb;
//    private PlayerMovement m_PlayerCharacterController;
//    private PlayerInputHandler inputHandler;

//    bool wallLeft = false;
//    bool wallRight = false;

//    RaycastHit leftWallHit, leftForwardWallHit, rightWallHit, rightForwardWallHit;

//    private void Start() {
//        rb = GetComponent<Rigidbody>();
//        m_PlayerCharacterController = GetComponent<PlayerMovement>();
//        inputHandler = GetComponent<PlayerInputHandler>();
//    }

//    bool CanWallRun() {
//        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHight);
//    }

//    void CheckWall() {
//        Vector3 pos = rb.position;
//        wallLeft = Physics.Raycast(pos, -orientation.right, out leftWallHit, wallDistance, whatIsWall); //||
//            //Physics.Raycast(pos, (-orientation.right + transform.forward), out leftForwardWallHit, wallDistance);

//        wallRight = Physics.Raycast(pos, orientation.right, out rightWallHit, wallDistance, whatIsWall);

//        Debug.Log(pos);
//        //Debug.DrawLine(pos, leftWallHit.point, Color.red);
//        //Debug.DrawLine(pos, rightWallHit.point, Color.blue);
//        Debug.DrawRay(pos, -orientation.right * wallDistance, Color.red);
//        //Debug.DrawRay(pos, (-orientation.right + transform.forward), Color.green);
//    }

//    private void StartWallRun() {
//        m_PlayerCharacterController.DisableGravity();
//        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

//        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);

//        if (wallLeft) {
//            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
//            rb.AddForce(-orientation.right * 1 *  Time.deltaTime);
//        }
//        else if (wallRight) {
//            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
//            rb.AddForce(orientation.right * 1 * Time.deltaTime);
//        }
//        if (inputHandler.GetJumpInputDown()) {
//            if(wallLeft) {
//                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
//                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

//                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
//            }
//            else if(wallRight) {
//                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
//                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

//                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
//            }
//        }

//    }

//    private void StopWallRun() {
//        m_PlayerCharacterController.EnableGravity();
//        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunFovTime * Time.deltaTime);
//        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
//    }

//    private void FixedUpdate() {
//        CheckWall();
//        if (CanWallRun()) {
//            if (wallLeft || wallRight) {
//                StartWallRun();
//            }
//            else {
//               StopWallRun();
//            }
//        }
//        else
//           StopWallRun();
//    }
//}


using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

[RequireComponent(typeof(PlayerMovement))]
public class WallRun : MonoBehaviour {

    public float wallMaxDistance = 1;
    public float wallSpeedMultiplier = 1.2f;
    public float minimumHeight = 1.2f;
    public float maxAngleRoll = 20;
    [Range(0.0f, 1.0f)]
    public float normalizedAngleThreshold = 0.1f;

    public float jumpDuration = 1;
    public float cameraTransitionDuration = 1;

    public float wallGravityDownForce = 20f;

    public Transform orientation;


    PlayerMovement m_PlayerCharacterController;
    PlayerInputHandler m_InputHandler;
    private Rigidbody rb; 

    Vector3[] directions;
    RaycastHit[] hits;

    bool isWallRunning = false;
    Vector3 lastWallPosition;
    Vector3 lastWallNormal;
    float elapsedTimeSinceJump = 0;
    float elapsedTimeSinceWallAttach = 0;
    float elapsedTimeSinceWallDetatch = 0;
    bool jumping;
    float lastVolumeValue = 0;
    float noiseAmplitude;

    private bool rightWall, leftWall;

    [SerializeField] private Camera cam;
    [SerializeField] private float fov, wallRunFov, wallRunFovTime, camTilt, camTiltTime;
    public float tilt { get; private set; }

    bool isPlayerGrounded() => m_PlayerCharacterController.IsGrounded();

    public bool IsWallRunning() => isWallRunning;

    bool CanWallRun() {
        //float verticalAxis = Input.GetAxisRaw(GameConstants.k_AxisNameVertical);
        //Debug.Log("player grounded: " + isPlayerGrounded());
        return !isPlayerGrounded() && VerticalCheck();
    }

    bool VerticalCheck() {
        //Debug.Log("above min hight?: " + !Physics.Raycast(transform.position, Vector3.down, minimumHeight));
        return !Physics.Raycast(transform.position, Vector3.down, minimumHeight);
    }


    void Start() {
        m_PlayerCharacterController = GetComponent<PlayerMovement>();
        m_InputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody>();

        directions = new Vector3[]{
            Vector3.right,
            Vector3.right + Vector3.forward,
            Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.left
        };
    }


    public void LateUpdate() {
        isWallRunning = false;
        
        if (m_InputHandler.GetJumpInputDown()) {
            jumping = true;
            isWallRunning = false;
        }

        if (CanAttach()) {
            hits = new RaycastHit[directions.Length];

            for (int i = 0; i < directions.Length; i++) {
                Vector3 dir = orientation.transform.TransformDirection(directions[i]);

                Physics.Raycast(transform.position, dir, out hits[i], wallMaxDistance);
                if (hits[i].collider != null) {
                    Debug.DrawRay(transform.position, dir * hits[i].distance, Color.green);
                }
                else {
                    Debug.DrawRay(transform.position, dir * wallMaxDistance, Color.red);
                }
            }

            if (CanWallRun()) {
                hits = hits.ToList().Where(h => h.collider != null).OrderBy(h => h.distance).ToArray();
                if (hits.Length > 0) {
                    rightWall = Physics.Raycast(transform.position, orientation.right, wallMaxDistance);
                    leftWall = Physics.Raycast(transform.position, -orientation.right, wallMaxDistance); //debug this 
                    //Debug.Log("right wall: " + rightWall + " left wall: " + leftWall);
                    OnWall(hits[0]);
                    lastWallPosition = hits[0].point;
                    lastWallNormal = hits[0].normal;
                }
            }
        }

        if (isWallRunning) {
            elapsedTimeSinceWallDetatch = 0;
            elapsedTimeSinceWallAttach += Time.deltaTime;
            m_PlayerCharacterController.DisableGravity();
        }
        else {
            elapsedTimeSinceWallAttach = 0;
            elapsedTimeSinceWallDetatch += Time.deltaTime;
            m_PlayerCharacterController.EnableGravity();
            tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
        }
    }

    bool CanAttach() {
        if (jumping) {
            elapsedTimeSinceJump += Time.deltaTime;
            if (elapsedTimeSinceJump > jumpDuration) {
                elapsedTimeSinceJump = 0;
                jumping = false;
            }
            return false;
        }

        return true;
    }

    void OnWall(RaycastHit hit) {
        float d = Vector3.Dot(hit.normal, Vector3.up);
        if (d >= -normalizedAngleThreshold && d <= normalizedAngleThreshold) {
            m_PlayerCharacterController.DisableGravity();
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            // Vector3 alongWall = Vector3.Cross(hit.normal, Vector3.up);
            float vertical = Input.GetAxisRaw(GameConstants.k_AxisNameVertical);
            Vector3 alongWall = transform.TransformDirection(Vector3.forward);

            Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);
            Debug.DrawRay(transform.position, lastWallNormal * 10, Color.magenta);

            

            rb.AddForce(orientation.transform.forward * wallSpeedMultiplier * Time.deltaTime);
            if (rightWall) {
                tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
                rb.AddForce(orientation.transform.right * 5000 * Time.deltaTime);
            }
            else if (leftWall) {
                tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
                rb.AddForce(-orientation.transform.right * 5000 * Time.deltaTime);
            }
            //Debug.Log("onwall"); 
            isWallRunning = true;
            //Debug.Log(CalculateSide());
        }
    }

    float CalculateSide() {
        if (isWallRunning) {
            Vector3 heading = lastWallPosition - transform.position;
            Vector3 perp = Vector3.Cross(transform.forward, heading);
            float dir = Vector3.Dot(perp, transform.up);
            return dir;
        }
        return 0;
    }

    //public float GetCameraRoll() {
    //    float dir = CalculateSide();
    //    float cameraAngle = m_PlayerCharacterController.playerCamera.transform.eulerAngles.z;
    //    float targetAngle = 0;
    //    if (dir != 0) {
    //        targetAngle = Mathf.Sign(dir) * maxAngleRoll;
    //    }
    //    return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(elapsedTimeSinceWallAttach, elapsedTimeSinceWallDetatch) / cameraTransitionDuration);
    //}

    public Vector3 GetWallJumpDirection() {
        Debug.Log("here");
        if (isWallRunning) {
            return lastWallNormal + Vector3.up;
        }
        return Vector3.zero;
    }
}
