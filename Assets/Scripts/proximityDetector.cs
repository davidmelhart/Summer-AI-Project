using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proximityDetector : MonoBehaviour {
    private bool _playerDetected;
    public bool playerDetected {
        get {
            return _playerDetected;
        }
        set {
            _playerDetected = value;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            playerDetected = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            playerDetected = false;
        }
    }

    public void OnDrawGizmos() {
        if (transform.GetComponentInParent<TestMovement>().displayPath) {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, 15);
        }
    }
}
