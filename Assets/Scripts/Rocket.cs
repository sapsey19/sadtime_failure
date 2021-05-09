using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    public float rocketSpeed = 30f;

    public float blastRadius = 10f;
    public float explosionForce = 1000f;

    void Update() {
        //gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * Time.deltaTime * rocketSpeed);
        transform.position += Vector3.forward * Time.deltaTime * rocketSpeed;
    }

    private void OnTriggerEnter(Collider other) {
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach(Collider collider in colliders) {
            if(collider.gameObject.GetComponent<Rigidbody>() != null)
                collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, gameObject.transform.position, blastRadius);
        }
       
        Destroy(gameObject);
    }
}
