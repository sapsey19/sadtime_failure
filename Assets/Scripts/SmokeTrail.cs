using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrail : MonoBehaviour {

    private ParticleSystem smokeParticles;

    void Start() {
        smokeParticles = GetComponent<ParticleSystem>();
        StartCoroutine(Persist());
    }

    public void AboutToDie() {
        transform.parent = null;
        var emission = smokeParticles.emission;
        emission.rateOverTime = 0f;
        transform.localScale = new Vector3(1, 1, 1);
    }

    IEnumerator Persist() {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }

}
