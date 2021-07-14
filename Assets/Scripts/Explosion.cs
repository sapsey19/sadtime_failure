using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    private ParticleSystem explosionParticles;

    void Start() {
        explosionParticles = GetComponent<ParticleSystem>();
        StartCoroutine(Persist());
    }

    public void AboutToDie() {
        transform.parent = null;
        var emission = explosionParticles.emission;
        emission.rateOverTime = 0f;
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void Play() {
        explosionParticles.Play();
    }

    IEnumerator Persist() {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
