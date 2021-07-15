using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaucherScript : MonoBehaviour {

    public GameObject rocket;
    public Transform spawnPos;
    public LayerMask whatIsExplodable;
    public float rocketSpeed;

    private Camera cam;

    public GameObject testhit;

    private readonly float delay = 0.8f;

    float time = 0;
    private Animation recoil;
    private AudioSource fireSound;

    private void Start() {
        fireSound = GetComponent<AudioSource>();
        recoil = GetComponent<Animation>();
        cam = Camera.main;
    }

    void Update() {
        time += Time.deltaTime;
        
        if (Input.GetButton("Fire1") && time > delay) {
            GameObject tempRocket;
            fireSound.Play();
            recoil.Play("LauncherRecoil");
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000f, whatIsExplodable)) {
                if (Vector3.Distance(hit.point, spawnPos.position) < 1.4f) { //if in wall
                    tempRocket = Instantiate(rocket, hit.point, transform.rotation);
                    tempRocket.GetComponent<CapsuleCollider>().isTrigger = true; //set collider to trigger, otherwise it gets stuck in walls for some reason
                    testhit.transform.position = hit.point;
                }
                else {
                    tempRocket = Instantiate(rocket, spawnPos.position, transform.rotation);
                    tempRocket.GetComponent<CapsuleCollider>().isTrigger = false; //set trigger back to false if not in wall, in order to use OnCollisionEnter
                    Vector3 direction = (hit.point - spawnPos.transform.position).normalized; //get direction from center of screen to laucher position
                    tempRocket.GetComponent<Rigidbody>().AddForce(direction * rocketSpeed, ForceMode.Impulse);
                }
            }
            else {
                tempRocket = Instantiate(rocket, spawnPos.position, transform.rotation);
                tempRocket.GetComponent<Rigidbody>().AddForce(transform.forward * rocketSpeed, ForceMode.Impulse);
            }
            time = 0;
        }
    }
}   

