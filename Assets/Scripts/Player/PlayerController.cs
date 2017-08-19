using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float movementSpeed;
    public float rotationSpeed = 0.1f;
    private Rigidbody2D rigidBody2D;

    //Direction of movement
    private Vector2 direction;
    private float inputX;
    private float inputY;

    private void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

	void FixedUpdate ()
	{
        //Facing mouse position
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Quaternion rot = Quaternion.LookRotation (transform.position - mousePosition, Vector3.forward);

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed);
		transform.eulerAngles = new Vector3 (0, 0, transform.eulerAngles.z);
        rigidBody2D.angularVelocity = 0;

        //Button controls
        inputY = Input.GetAxis ("Vertical") * movementSpeed;
        inputX = Input.GetAxis("Horizontal") * movementSpeed;
        direction = new Vector2(inputX, inputY);

        rigidBody2D.AddForce(direction);
	}
}