using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    //projectile variables
    public float explosionRadius;
    public float explosionForce;

    private float explosionModifier; //change how high player moves based on crouching, jumping, etc

    //public AudioClip explosion;
    public AudioSource explosion;

    private SmokeTrail trail;
    //public GameObject explosionHolder;
    //private ParticleSystem explosionParticles;
    private Explosion explosionParticles;

    private void Awake() {
        trail = transform.Find("SmokeTrail_Cartoon").GetComponent<SmokeTrail>();
        explosionParticles = transform.Find("Explosion").GetComponent<Explosion>();
        explosion = GetComponent<AudioSource>();
        //explosionParticles = explosionHolder.GetComponent<ParticleSystem>();
    }

    //use when shooting close to a wall, otherwise OnCollisionEnter won't get fired
    private void OnTriggerEnter(Collider other) { 
        if (PlayerMovement.crouching && PlayerMovement.jumping) {
            explosionModifier = 2.0f;
        }
        else if (PlayerMovement.crouching) {
            explosionModifier = 1.5f;
        }
        else if (PlayerMovement.jumping) {
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
                    if (rb.CompareTag("Player")) {
                        rb.AddExplosionForce(explosionForce * explosionModifier, transform.position, explosionRadius);
                    }
                    if (rb.CompareTag("Enemy")) { //if an enemy is in explosion radius 
                        rb.GetComponent<EnemyAi>().DisableAi(); //disable navigation to allow upwards lift
                        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 2f);
                    }
                    else {
                        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                    }

                }
            }
            //AudioSource.PlayClipAtPoint(explosion, 0.9f * Camera.main.transform.position + 0.1f * transform.position, 1f); //makes audio louder by playing at a closer position to camera
            explosion.Play();
            if (trail)
                trail.AboutToDie(); //let smoke trail persist after rocket is deleted


            explosionParticles.Play();
            explosionParticles.AboutToDie();
            Destroy(gameObject);
        }
    }

    //more precise collision data means explosion doesn't clip in walls/floors 
    private void OnCollisionEnter(Collision collision) {
        if (PlayerMovement.crouching && PlayerMovement.jumping) {
            explosionModifier = 2.0f;
        }
        else if (PlayerMovement.crouching) {
            explosionModifier = 1.5f;
        }
        else if (PlayerMovement.jumping) {
            explosionModifier = 1.2f;
        }
        else {
            explosionModifier = 1.0f;
        }

        if (collision.transform.CompareTag("Environment") || collision.transform.CompareTag("Enemy")) { //mabye just have an explodable tag 
            Debug.Log("hit");
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
            //AudioSource.PlayClipAtPoint(explosion, 0.9f * Camera.main.transform.position + 0.1f * transform.position, 1f); //makes audio louder by playing at a closer position to camera
            explosion.Play();
            if (trail)
                trail.AboutToDie(); //let smoke trail persist after rocket is deleted

            //play explosion and let it persist after rocket death 
            explosionParticles.Play();
            explosionParticles.AboutToDie();
            Destroy(gameObject);
        }
    }
}
