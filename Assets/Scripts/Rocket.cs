using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    public PlayerMovement player; 

    //projectile variables
    public float speed;
    public float explosionRadius;
    public float explosionForce;

    private float explosionModifier;

    public AudioClip explosion;

    private void Update() {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other) {
        if(PlayerMovement.crouching && PlayerMovement.jumping) {
            explosionModifier = 2.0f;
        }
        else if(PlayerMovement.crouching) {
            explosionModifier = 1.5f;
        }
        else if(PlayerMovement.jumping) {
            explosionModifier = 1.2f;
        }
        else {
            explosionModifier = 1.0f;
        }
        if(other.CompareTag("Environment")) {
            Collider[] hitObjects = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach(Collider hit in hitObjects) {
                Rigidbody temp = hit.GetComponent<Rigidbody>();
                if(temp) { //is not null 
                    temp.AddExplosionForce(explosionForce * explosionModifier, transform.position, explosionRadius);
                }
            }
            Debug.Log(Camera.main.transform.position);
            AudioSource.PlayClipAtPoint(explosion, 0.9f * Camera.main.transform.position + 0.1f * transform.position, 1f); //makes audio louder by playing at a closer position to camera
            Destroy(gameObject);
        }
    }
}
