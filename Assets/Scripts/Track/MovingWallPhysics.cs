using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWallPhysics : MonoBehaviour {

    Animator anim;

	void Start () {
        anim = GetComponent<Animator>();
	}
	
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D collider) {
        anim.enabled = false; //getting phyisics working with animator would not work with the exponential movement I have, so this is the next best thing
    }

    void OnCollisionExit2D(Collision2D collider) {
        anim.enabled = true;
    }
}
