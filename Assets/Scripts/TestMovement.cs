using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour {

    public Transform target;
    float speed = 2f;
    Vector3[] path;
    int targetIndex;
    public bool displayPath;

    proximityDetector proximityDetector;

    void Awake() {
        proximityDetector = transform.GetChild(0).GetComponent<proximityDetector>();
    }

    void Update() {
        if (proximityDetector.playerDetected) {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
        if (pathSuccessful) {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath() {
        Vector3 currentWaypoint = path[0];

        while (true) {
            if (transform.position == currentWaypoint) {
                targetIndex++;
                if(targetIndex >= path.Length) {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos() {
        if (path != null && displayPath) {
            for (int i=targetIndex; i < path.Length; i++) {
                Gizmos.color = Color.cyan;

                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                } else {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

}
