using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookshotGun : MonoBehaviour {

    [SerializeField] private Transform debugHitPointTransform;

    public Transform player;
    private PlayerMovement2 playerCharacterController;
    private CharacterController characterController;

    private float cameraVerticalAngle;
    private float characterVelocityY;
    private Vector3 characterVelocityMomentum;
    public Camera playerCamera;
    //private CameraFov cameraFov;
    //private ParticleSystem speedLinesParticleSystem;
    private Vector3 hookshotPosition;
    private float hookshotSize;

    private LineRenderer lr;
    private Vector3 currentGrapplePosition;
    public Transform gunTip;
    private Vector3 grapplePoint;

    public LayerMask whatIsGrappleable;
    public float maxDistance = 600f;
    public float ropeSpeed = 1000f;

    private bool hitwall;

    private float moveDelay = 0;

    //*notices ur lerp* owo whats this 
    float timeElapsed;
    //float lerpDuration

    private void Awake() {
        characterController = player.transform.GetComponent<CharacterController>();
        playerCharacterController = player.transform.GetComponent<PlayerMovement2>();
        lr = transform.GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    public void HandleHookshotStart() {
        if (HookshotInputDown()) {
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, maxDistance, whatIsGrappleable)) {
                
                // Hit something
                debugHitPointTransform.position = hit.point;
                grapplePoint = hit.point;
                hookshotPosition = hit.point;
                hookshotSize = 0f;
                //hookshotTransform.gameObject.SetActive(true);
                //hookshotTransform.localScale = Vector3.zero;
                playerCharacterController.state = PlayerMovement2.State.HookshotThrown;

                lr.positionCount = 2;
                currentGrapplePosition = gunTip.position;

                lr.enabled = true;
            }
        }
    }

    public void HandleHookshotThrow() {
        playerCharacterController.state = PlayerMovement2.State.HookshotFlyingPlayer;

        //if (hitwall) {
        //    //playerCharacterController.state = PlayerMovement2.State.HookshotFlyingPlayer;
        //    //cameraFov.SetCameraFov(HOOKSHOT_FOV);
        //    //speedLinesParticleSystem.Play();
        //}
    }

    public void HandleHookshotMovement() {
        
        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 5f;

        // Move Character Controller
        if (moveDelay > .5f) {
            characterController.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);

        }
        else {
            moveDelay += Time.deltaTime;
        }


        //if (Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotPositionDistance) {
        //    // Reached Hookshot Position
        //    StopHookshot();
        //}

        if (HookshotInputUp()) {
            // Cancel Hookshot
            StopHookshot();
            moveDelay = 0f;
        }

        if (TestInputJump()) {
            // Cancelled with Jump
            float momentumExtraSpeed = 7f;
            characterVelocityMomentum = hookshotDir * hookshotSpeed * momentumExtraSpeed;
            float jumpSpeed = 40f;
            characterVelocityMomentum += Vector3.up * jumpSpeed;
            StopHookshot();
        }
    }

    public void StopHookshot() {
        playerCharacterController.state = PlayerMovement2.State.Normal;
        ResetGravityEffect();

        lr.enabled = false;
        //hookshotTransform.gameObject.SetActive(false);
        //cameraFov.SetCameraFov(NORMAL_FOV);
        //speedLinesParticleSystem.Stop();
    }
   

    public void DrawRope() {
        //currentGrapplePosition = Vector3.MoveTowards(currentGrapplePosition, grapplePoint, Time.deltaTime * 100f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    private bool HookshotInputDown() {
        return Input.GetMouseButtonDown(1);
    }

    private bool HookshotInputUp() {
        return Input.GetMouseButtonUp(1);
    }

    private bool TestInputJump() {
        return Input.GetKeyDown(KeyCode.Space);
    }
    private void ResetGravityEffect() {
        characterVelocityY = 0f;
    }


}
