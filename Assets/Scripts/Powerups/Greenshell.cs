using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greenshell : MonoBehaviour {

	int bounces; //how many bounces have happened
	int maxBounces = 3; //maximum bounces allowed

	Animator animator;

    Rigidbody2D body;

	// Use this for initialization
	void Start () {
		animator = GetComponent <Animator> ();

        body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D	collision){
		bounces++;

        //dead if maximum bounces have been reached

		if (bounces > maxBounces || collision.gameObject.tag.Equals ("Player")) { //if they are a player of if the max bounces have been reached, this shell is destroyed
			animator.SetTrigger ("dead");
            body.velocity = Vector2.zero;
		}
	}

	public void AnimationEnded() { //Called by animation trigger
		Destroy (gameObject);
	}
}
