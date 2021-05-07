using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour {
    public Transform startPos;

    private void OnTriggerEnter(Collider other) {
        other.gameObject.transform.position = startPos.position;
    }
}
