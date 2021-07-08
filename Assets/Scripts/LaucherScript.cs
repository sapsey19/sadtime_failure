using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaucherScript : MonoBehaviour {

    public GameObject rocket;
    public Transform spawnPos;

    public float delay = 1f;

    float time = 0;

    void Update() {
        time += Time.deltaTime;
        Debug.Log(time);
        if (time > delay) {
            if (Input.GetMouseButtonDown(0)) {
                Instantiate(rocket, spawnPos.transform.position, transform.rotation);
                time = 0;
            }
        }
    }
}
