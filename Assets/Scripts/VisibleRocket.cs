using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleRocket : MonoBehaviour {

    //public AudioClip explosion
    private AudioSource explosionSound;

    //particles 
    private SmokeTrail trail;
    private Explosion explosionParticles;

    //public static bool explode = false;

    private void Awake() {
        trail = transform.Find("SmokeTrail_Cartoon").GetComponent<SmokeTrail>();
        //explosionParticles = transform.Find("Explosion").GetComponent<Explosion>();
        //explosionSound = GetComponent<AudioSource>();
        //explosionParticles = explosionHolder.GetComponent<ParticleSystem>();
    }

    //private void OnCollisionEnter(Collision collision) {

    //    if (collision.transform.CompareTag("Environment") || collision.transform.CompareTag("Enemy")) { //mabye just have an explodable tag 
    //        if (explode) {
    //            Explode();
    //            explode = false;
    //        }

    //    }
    //}

    private void Update() {
        //if (explode) {
        //    Explode();
        //    explode = false;
        //}
    }

    public void Explode() { //maybe instaite visible rocket in invis. rocket start/awake method? that way call call explode 
        //explosionSound.Play();
        if (trail)
            trail.AboutToDie(); //let smoke trail persist after rocket is deleted

        //play explosion and let it persist after rocket death 
        //explosionParticles.Play();
        //if (explosionParticles)
        //    explosionParticles.AboutToDie();

        Destroy(gameObject);
    }
}
