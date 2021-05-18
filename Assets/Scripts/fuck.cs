using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fuck : MonoBehaviour {

    public GameObject projectile;
    private bool fire = false;

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            fire = true;
        }

        if(fire) {
            projectile.transform.position += Vector3.forward * Time.deltaTime * 10f;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("hit");
    }

}
