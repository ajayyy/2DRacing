using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour {

	Rigidbody2D body;

	void Start () {
		body = GetComponent <Rigidbody2D> ();
	}
	
	void FixedUpdate () {
		body.angularVelocity = 70; //since it uses Rigidbody it can actually push players around, unlike if using an animation
	}
}
