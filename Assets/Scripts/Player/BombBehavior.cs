using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehavior : MonoBehaviour {
    public GameObject fireBlockade;
    public GameObject destroyEffect;

    void OnDestroy()
    {
        //Fixing the fire rotation
        Quaternion fireRot = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        //Snapping the fire to the grid. This way fires will always line up with walls.
        Vector3 firePos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 2);
        Instantiate(fireBlockade, firePos, fireRot);
        Instantiate(destroyEffect, firePos, fireRot);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            Destroy(gameObject);
        }
    }
}
