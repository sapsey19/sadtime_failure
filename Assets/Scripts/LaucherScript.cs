using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaucherScript : MonoBehaviour {

    public GameObject rocket;
    public Transform spawnPos;

    private float delay = 0.8f;

    float time = 0;

    void Update() {
        time += Time.fixedDeltaTime;
        if (time > delay) {
            if (Input.GetButton("Fire1")) {
                Instantiate(rocket, spawnPos.transform.position, transform.rotation);
                time = 0;
            }
        }
    }
}
