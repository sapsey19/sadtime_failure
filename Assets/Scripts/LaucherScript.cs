using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaucherScript : MonoBehaviour {

    public GameObject rocket;
    public Transform spawnPos;
    public LayerMask whatIsExplodable;

    public GameObject testhit;

    private readonly float delay = 0.8f;

    float time = 0;

    void Update() {
        time += Time.deltaTime;
        if (Input.GetButton("Fire1") && time > delay) {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 1f, whatIsExplodable)) { //if gun is close to/in wall
                Instantiate(rocket, hit.point, transform.rotation);
                //testhit.transform.position = hit.point;
            }
            else {
                Instantiate(rocket, spawnPos.transform.position, transform.rotation);
            }
            time = 0;
        }
    }
}

