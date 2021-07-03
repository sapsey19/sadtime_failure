using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    //projectile variables
    public float speed;
    public float explosionRadius;
    public float explosionForce;

    private void Update() {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Environment")) {
            Collider[] hitObjects = Physics.OverlapSphere(transform.position, explosionRadius);
            //Debug.Log(hitObjects);
            foreach(Collider hit in hitObjects) {
                Debug.Log(hit.name);
                Rigidbody temp = hit.GetComponent<Rigidbody>();
                if(temp) {
                    temp.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }
            Destroy(gameObject);

        }
    }
}
