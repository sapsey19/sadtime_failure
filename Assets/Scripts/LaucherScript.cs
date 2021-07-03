using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaucherScript : MonoBehaviour {

    public GameObject rocket;
    public Transform spawnPos;

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Instantiate(rocket, spawnPos.transform.position, transform.rotation);
        }
    }
}
