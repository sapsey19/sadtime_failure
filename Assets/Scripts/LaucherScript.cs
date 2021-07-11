using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaucherScript : MonoBehaviour {

    public GameObject rocket;
    public Transform spawnPos;
    public LayerMask whatIsExplodable;
    public float rocketSpeed;

    public GameObject testhit;

    private readonly float delay = 0.8f;

    float time = 0;
    private Animation recoil;

    private void Start() {
        recoil = GetComponent<Animation>();
    }
    void Update() {
        time += Time.deltaTime;
        if (Input.GetButton("Fire1") && time > delay) {
            recoil.Play("LauncherRecoil");
            GameObject tempRocket = Instantiate(rocket, spawnPos.position, transform.rotation);
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 1000f, whatIsExplodable)) {
                Vector3 direction = (hit.point - spawnPos.transform.position).normalized; //get direction from raycast to laucher
                tempRocket.GetComponent<Rigidbody>().AddForce(direction * rocketSpeed, ForceMode.Impulse);
            }
            else
                tempRocket.GetComponent<Rigidbody>().AddForce(transform.forward * rocketSpeed, ForceMode.Impulse);
            time = 0;
        }
    }
}

