using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleRocket : MonoBehaviour {

    //projectile variables
    public float explosionRadius;
    public float explosionForce;
    private float explosionModifier; //change how far player moves based on crouching, jumping, etc

    private GameObject vRocket;


    private Explosion explosionParticles;
    private AudioSource explosionSound;
    //more precise collision data means explosion doesn't clip in walls/floors 

    private void Awake() {
        vRocket = GameObject.Find("VisibleRocket");
        if (vRocket == null)
            Debug.Log("empty lol sad");

        explosionSound = GetComponent<AudioSource>();
        explosionParticles = GetComponent<Explosion>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (PlayerMovement.crouching && !PlayerMovement.grounded) {
            explosionModifier = 1.5f;
        }
        else if (!PlayerMovement.grounded) {
            explosionModifier = 1f;
        }
        else if (PlayerMovement.crouching) {
            explosionModifier = 0.8f;
        }
        else {
            explosionModifier = 0.5f;
        }

        if (collision.transform.CompareTag("Environment") || collision.transform.CompareTag("Enemy")) { //mabye just have an explodable tag 
            //VisibleRocket.explode = true;
            //vRocket.GetComponent<VisibleRocket>().Explode();
            explosionParticles.Play();
            Collider[] hitObjects = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider hit in hitObjects) {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb) {
                    if (rb.CompareTag("Player")) {
                        rb.AddExplosionForce(explosionForce * explosionModifier, transform.position, explosionRadius);
                    }
                    else if (rb.CompareTag("Enemy")) { //if an enemy is in explosion radius 
                        rb.GetComponent<EnemyAi>().DisableAi(); //disable navigation to allow upwards lift
                        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 2f);
                    }
                    else {
                        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                    }
                    
                }
            }
            Destroy(gameObject);
        }
    }

    //use when shooting close to a wall, otherwise OnCollisionEnter won't get fired
    //private void OnTriggerEnter(Collider other) {

    //    if (PlayerMovement.crouching && PlayerMovement.jumping) {
    //        explosionModifier = 1.45f;
    //    }
    //    else if (PlayerMovement.crouching) {
    //        explosionModifier = 1.3f;
    //    }
    //    else if (PlayerMovement.jumping) {
    //        explosionModifier = 1.2f;
    //    }
    //    else {
    //        explosionModifier = 1.0f;
    //    }

    //    if (other.CompareTag("Environment") || other.CompareTag("Enemy")) { //mabye just have an explodable tag 
    //        Debug.Log("trigger");
    //        VisibleRocket.explode = true;
    //        Collider[] hitObjects = Physics.OverlapSphere(transform.position, explosionRadius);
    //        foreach (Collider hit in hitObjects) {
    //            Rigidbody rb = hit.GetComponent<Rigidbody>();
    //            if (rb) {
    //                if (rb.CompareTag("PlayerBox")) {
    //                    rb.AddExplosionForce(explosionForce * explosionModifier, transform.position, explosionRadius);
    //                }
    //                if (rb.CompareTag("Enemy")) { //if an enemy is in explosion radius 
    //                    rb.GetComponent<EnemyAi>().DisableAi(); //disable navigation to allow upwards lift
    //                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 2f);
    //                }
    //                else {
    //                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
    //                }
    //            }
    //        }

    //        Destroy(gameObject);
    //    }
    //}
}



