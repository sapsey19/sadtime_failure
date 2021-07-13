using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    //projectile variables
    public float explosionRadius;
    public float explosionForce;

    private float explosionModifier; //change how high player moves based on crouching, jumping, etc

    public AudioClip explosion;

    private SmokeTrail trail;
    //private AudioSource fireSound;

    private void Awake() {
        trail = transform.Find("CartoonSmoke").GetComponent<SmokeTrail>();
        //fireSound = GetComponent<AudioSource>();
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

        if (other.CompareTag("Environment") || other.CompareTag("Enemy")) { //mabye just have an explodable tag 
            Collider[] hitObjects = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider hit in hitObjects) {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb) {
                    if(rb.CompareTag("Player")) { 
                        rb.AddExplosionForce(explosionForce * explosionModifier, transform.position, explosionRadius);
                    }
                    if (rb.CompareTag("Enemy")) { //if an enemy is in explosion radius 
                        rb.GetComponent<EnemyAi>().DisableAi(); //disable navigation to allow upwards lift
                        rb.AddExplosionForce(explosionForce * explosionModifier, transform.position, explosionRadius, 10f);
                    }
                    else {
                        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                    }
                    
                }
            }
            AudioSource.PlayClipAtPoint(explosion, 0.9f * Camera.main.transform.position + 0.1f * transform.position, 1f); //makes audio louder by playing at a closer position to camera

            if (trail)
                trail.AboutToDie(); //let smoke trail persist after rocket is deleted 
            Destroy(gameObject);
        }
    }
}
