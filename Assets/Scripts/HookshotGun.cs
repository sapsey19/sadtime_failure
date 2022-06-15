using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookshotGun : MonoBehaviour {

    //[SerializeField] private Transform debugHitPointTransform;

    public Transform player;
    private PlayerMovement playerMovement;
    private Rigidbody playerRb;

    public Camera playerCamera;

    private LineRenderer lr;
    private Vector3 currentGrapplePosition;
    public Transform gunTip;
    private Vector3 grapplePoint;

    public LayerMask whatIsGrappleable;
    private readonly float maxDistance = 1000f; //only this high for testing lole 
    public float ropeSpeed = 1000f;

    private bool reachedDesination = false;

    private readonly float maxGrappleSpeed = 200f;

    private AudioSource hookFireSFX;

    public GameObject hook;

    private void Awake() {
        playerMovement = player.transform.GetComponent<PlayerMovement>();
        lr = transform.GetComponent<LineRenderer>();
        lr.enabled = false;
        playerRb = player.GetComponent<Rigidbody>();
        hookFireSFX = GetComponent<AudioSource>();
    }

    public void HandleHookshotStart() {
        hook.transform.position = gunTip.position;
        hook.transform.rotation = gunTip.rotation;
        if (HookshotInputDown()) {
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, maxDistance, whatIsGrappleable)) {
                hookFireSFX.Play();
                grapplePoint = hit.point; //point to move to 

                currentGrapplePosition = gunTip.position;

                lr.positionCount = 2;
                lr.enabled = true;

                playerMovement.state = PlayerMovement.State.HookshotThrown;
            }
        }
    }

    public void HandleHookshotThrow() { 
        //hook.transform.position += Vector3.forward;
        if (HookshotInputUp())
            StopHookshot();
        else { //if rope distance is close enough to wall, start grapple 
            if (Vector3.Distance(currentGrapplePosition, grapplePoint) < .5f) { //should be a faster way to check distance
                playerRb.velocity = Vector3.zero;
                //playerRb.useGravity = false;
                playerMovement.DisableGravity();
                playerMovement.state = PlayerMovement.State.HookshotFlyingPlayer;
            }
        }
    }

    public void HandleHookshotMovement() {        
        Vector3 hookshotDir = (grapplePoint - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, grapplePoint), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 300f;
       
        if (Vector3.Distance(transform.position, grapplePoint) < 7.5f) {
            float percentageOfMax = Vector3.Distance(transform.position, grapplePoint) / 15f;
            float speed = Mathf.MoveTowards(hookshotSpeedMin, hookshotSpeedMax, percentageOfMax * hookshotSpeedMax);
            playerRb.velocity = Vector3.ClampMagnitude(playerRb.velocity, speed);
        }
        if (Vector3.Distance(transform.position, grapplePoint) < 1f) {
            playerRb.velocity = Vector3.zero;
            playerRb.useGravity = false;
            reachedDesination = true;
        }
        else if (!reachedDesination) {
            playerRb.AddForce(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime); //move player 
        }

        playerRb.velocity = Vector3.ClampMagnitude(playerRb.velocity, maxGrappleSpeed); //limit move speed 

        if (HookshotInputUp()) {
            StopHookshot();
        }
    }

    public void StopHookshot() {
        playerMovement.state = PlayerMovement.State.Normal;
        //enable both rigibody gravity and extra gravity in playerMovement script 
        //playerRb.useGravity = true;
        playerMovement.EnableGravity();
        reachedDesination = false;
        lr.enabled = false;
    }
   

    public void DrawRope() {        
        currentGrapplePosition = Vector3.MoveTowards(currentGrapplePosition, grapplePoint, Time.deltaTime * ropeSpeed);
        
        hook.transform.position = currentGrapplePosition; //draw hook at end of line 

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    private bool HookshotInputDown() {
        return Input.GetMouseButtonDown(0); //why do i have like 3 different places where i have input 
    }

    private bool HookshotInputUp() {
        return Input.GetMouseButtonUp(0);  //this shouldnt exist methinks 
    }

}
