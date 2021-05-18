using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    //projectile variables
    public float speed;

    public bool moving = false;

    private void Update() {
        if (!moving) {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Environment")) {
            moving = false;
        }
    }
}
