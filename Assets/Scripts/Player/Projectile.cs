using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private float clearTime = 30f;
    private float timeStamp;
    public GameObject destroyEffect;

    void OnDestroy()
    {
        Instantiate(destroyEffect, transform.position, transform.rotation);
    }

    // Use this for initialization
    void Start()
    {
        timeStamp = Time.time + clearTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeStamp <= Time.time)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            Destroy(gameObject);
        }
    }
}
