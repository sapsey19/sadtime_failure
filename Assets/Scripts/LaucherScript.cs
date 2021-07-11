using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaucherScript : MonoBehaviour {

    public GameObject rocket;
    public Transform spawnPos;
    public LayerMask whatIsExplodable;
    public float rocketSpeed;

    private Camera cam;
    private bool inWall = false;

    public GameObject testhit;

    private readonly float delay = 0.8f;

    float time = 0;
    private Animation recoil;

    private void Start() {
        recoil = GetComponent<Animation>();
        cam = Camera.main;
    }

    void Update() {
        time += Time.deltaTime;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit testhit, 1000f, whatIsExplodable)) {
            //Vector3 direction = (testhit.point - spawnPos.transform.position).normalized; //get direction from center of screen to laucher position
            //Debug.Log(Vector3.Distance(testhit.point, spawnPos.position));

        }


        if (Input.GetButton("Fire1") && time > delay) {
            recoil.Play("LauncherRecoil");


            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000f, whatIsExplodable)) {
                Debug.Log(Vector3.Distance(testhit.point, spawnPos.position));
                if (Vector3.Distance(testhit.point, spawnPos.position) < 1.4f) { //if in wall
                    GameObject tempRocket = Instantiate(rocket, hit.point, transform.rotation);
                    tempRocket.GetComponent<Rigidbody>().AddForce(transform.forward * .01f , ForceMode.Impulse); //delte this mayb 
                    //testhit.transform.position = hit.point;

                }
                else {
                    GameObject tempRocket = Instantiate(rocket, spawnPos.position, transform.rotation);
                    Vector3 direction = (hit.point - spawnPos.transform.position).normalized; //get direction from center of screen to laucher position
                    tempRocket.GetComponent<Rigidbody>().AddForce(direction * rocketSpeed, ForceMode.Impulse);
                }
            }
            else {
                GameObject tempRocket = Instantiate(rocket, spawnPos.position, transform.rotation);
                tempRocket.GetComponent<Rigidbody>().AddForce(transform.forward * rocketSpeed, ForceMode.Impulse);
            }
            time = 0;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Environment")) {
            inWall = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Environment")) {
            Debug.Log("exiting wall");
            inWall = false;
        }
    }
}   

