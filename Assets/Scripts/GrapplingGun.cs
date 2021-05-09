using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour {

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, cam, player;
    private float maxDistance = 100f;
    private SpringJoint joint;

    private float distanceFromPoint;
    public float grappleSpeed = 10f;

    //ignore this 
    public GameObject rocket;

    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1)) {
            StopGrapple();
        }

        if(IsGrappling()) {
            joint.maxDistance -= distanceFromPoint * grappleSpeed;
        }

        //ignore this 
        if(Input.GetMouseButtonDown(0)) {
            Instantiate(rocket, gunTip.position, cam.transform.rotation);
        }
    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
    }


    /// Call whenever we want to start a grapple
    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, whatIsGrappleable)) {
            //player.gameObject.GetComponent<Rigidbody>().useGravity = false;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * .8f;
            joint.minDistance = distanceFromPoint * 0.0025f;

            //Adjust these values to fit your game.
            joint.spring = 8f;
            joint.damper = 5f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

        }
    }

  
    /// Call whenever we want to stop a grapple
    void StopGrapple() {
        player.gameObject.GetComponent<Rigidbody>().useGravity = true;
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
