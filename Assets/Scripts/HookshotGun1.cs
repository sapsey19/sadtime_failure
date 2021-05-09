using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookshotGun1 : MonoBehaviour {

    [SerializeField] private Transform debugHitPointTransform;

    public Transform player;
    private PlayerMovement3 playerMovement;
    private Rigidbody playerRb;

    public Camera playerCamera;
    //private CameraFov cameraFov;
    //private ParticleSystem speedLinesParticleSystem;
    //private Vector3 hookshotPosition;

    private LineRenderer lr;
    private Vector3 currentGrapplePosition;
    public Transform gunTip;
    private Vector3 grapplePoint;

    public LayerMask whatIsGrappleable;
    public float maxDistance = 100f;
    public float ropeSpeed = 1000f;

    private bool reachedDesination = false;
 

    private void Awake() {
        playerMovement = player.transform.GetComponent<PlayerMovement3>();
        lr = transform.GetComponent<LineRenderer>();
        lr.enabled = false;
        playerRb = player.GetComponent<Rigidbody>();
    }

    public void HandleHookshotStart() {
        if (HookshotInputDown()) {
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, maxDistance, whatIsGrappleable)) {
                debugHitPointTransform.position = hit.point;
                grapplePoint = hit.point;
                playerMovement.state = PlayerMovement3.State.HookshotThrown;

                lr.positionCount = 2;
                currentGrapplePosition = gunTip.position;

                lr.enabled = true; //shows rope 
            }
        }
    }

    public void HandleHookshotThrow() {
        if (HookshotInputUp())
            StopHookshot();
        else { 
            if (Vector3.Distance(currentGrapplePosition, grapplePoint) < .5f) {
                playerRb.velocity = Vector3.zero;
                playerRb.useGravity = false;
                playerMovement.DisableGravity();
                playerMovement.state = PlayerMovement3.State.HookshotFlyingPlayer;
            }
        }
    }

    public void HandleHookshotMovement() {
        
        Vector3 hookshotDir = (grapplePoint - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, grapplePoint), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 300f;

        if(Vector3.Distance(transform.position, grapplePoint) < 2f) {
            //playerRb.MovePosition(grapplePoint);
            playerRb.velocity = Vector3.zero;
            playerRb.useGravity = false;
            reachedDesination = true;
            Debug.Log("i have arrived");
        }
        else if (!reachedDesination){
            playerRb.AddForce(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);
        }
  

        if (HookshotInputUp()) {
            StopHookshot();
            playerRb.useGravity = true;
            reachedDesination = false;
            playerMovement.EnabledGravity();
        }
    }

    public void StopHookshot() {
        playerMovement.state = PlayerMovement3.State.Normal;
        ResetGravityEffect();

        lr.enabled = false;
        //cameraFov.SetCameraFov(NORMAL_FOV);
        //speedLinesParticleSystem.Stop();
    }
   

    public void DrawRope() {
        currentGrapplePosition = Vector3.MoveTowards(currentGrapplePosition, grapplePoint, Mathf.Pow(Time.deltaTime, .5f) * 50f); //maybe have it go faster as time goes on, Time.delta^2? 

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
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
        //playerMovement.characterVelocityY = 0f;
    }


}
