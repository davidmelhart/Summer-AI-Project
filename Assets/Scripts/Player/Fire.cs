using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {
    public float dieOutTime = 5f;
    private float timeStamp;
    public GameObject destroyEffect;

    // Use this for initialization
    void Start () {
        timeStamp = Time.time + dieOutTime;
	}
	
	// Update is called once per frame
	void Update () {
		if(timeStamp <= Time.time)
        {
            Destroy(gameObject);
        }
	}

    void OnDestroy() {
        Instantiate(destroyEffect, transform.position, transform.rotation);
    }
}
