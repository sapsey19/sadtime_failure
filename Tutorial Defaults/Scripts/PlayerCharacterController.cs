using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacterController : MonoBehaviour {
    public Camera playerCamera;
    public float gravityDownForce = 20f;
    public LayerMask groundCheckLayers = -1;
    public float groundCheckDistance = 0.05f;

    public float maxSpeedOnGround = 10f;
    public float movementSharpnessOnGround = 15;
    public float maxSpeedInAir = 10f;
    public float accelerationSpeedInAir = 25f;

    public float rotationSpeed = 200f;

    public float jumpForce = 9f;

    public float cameraHeightRatio = 0.9f;
    public float capsuleHeightStanding = 1.8f;


    public Vector3 characterVelocity { get; set; }
    public bool isGrounded { get; private set; }
    public bool hasJumpedThisFrame { get; private set; }
    public float height { get; private set; }

    PlayerInputHandler m_InputHandler;
    CharacterController m_Controller;
    Actor m_Actor;
    Vector3 m_GroundNormal;
    Vector3 m_CharacterVelocity;
    float m_LastTimeJumped = 0f;
    float m_CameraVerticalAngle = 0f;
    float m_footstepDistanceCounter;
    float m_TargetCharacterHeight;

    const float k_JumpGroundingPreventionTime = 0.2f;
    const float k_GroundCheckDistanceInAir = 0.07f;

    void Start() {
        m_Controller = GetComponent<CharacterController>();
        m_InputHandler = GetComponent<PlayerInputHandler>();
        m_Actor = GetComponent<Actor>();
        m_Controller.enableOverlapRecovery = true;
    }

    void Update() {
        hasJumpedThisFrame = false;

        bool wasGrounded = isGrounded;
        GroundCheck();
        HandleCharacterMovement();
    }

    void GroundCheck() {
        float chosenGroundCheckDistance = isGrounded ? (m_Controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;
        isGrounded = false;
        m_GroundNormal = Vector3.up;

        if(Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime) {
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height), m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore)) {
                m_GroundNormal = hit.normal;

                if(Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(m_GroundNormal)) {
                    isGrounded = true;

                    if(hit.distance > m_Controller.skinWidth) {
                        m_Controller.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    void HandleCharacterMovement() {
        transform.Rotate(new Vector3(0f, (m_InputHandler.GetLookInputHorizontal() * rotationSpeed), 0f), Space.Self);

        m_CameraVerticalAngle += m_InputHandler.GetLookInputVertical() * rotationSpeed;
        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);
        playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
    }

    bool IsNormalUnderSlopeLimit(Vector3 normal) {
        return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
    }

    Vector3 GetCapsuleBottomHemisphere() {
        return transform.position + (transform.up * m_Controller.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule
    Vector3 GetCapsuleTopHemisphere(float atHeight) {
        return transform.position + (transform.up * (atHeight - m_Controller.radius));
    }

    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal) {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }
}
