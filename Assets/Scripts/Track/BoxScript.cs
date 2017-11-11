using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour {

	Animator animator;

    bool collided; //currently collided with an object

	void Start () {
		animator = GetComponent <Animator> ();
	}

	void Update () {

        if (collided && animator.GetCurrentAnimatorStateInfo(0).IsName("dead")) {
            animator.enabled = false; //disable animator if it is dead, and collided so that it doesn't spawn while someone is on it
        }

        float h;
		float s;
		float v;
		Color.RGBToHSV (GetComponent <SpriteRenderer> ().color, out h, out s, out v);
		//adjust hue
		Color adjustedColor = Color.HSVToRGB (h + 0.4f * Time.deltaTime, s, v);
		GetComponent <SpriteRenderer> ().color = adjustedColor;

		//rotate box aroud
		transform.Rotate (new Vector3(0,0,200 * Time.deltaTime));


	}

	void OnTriggerEnter2D(Collider2D collision){
		animator.SetBool ("colliding", true);
        collided = true;

		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("idle")) {
			animator.SetTrigger ("dead");

            if (collision.gameObject.tag.Equals("Player")) {
				collision.gameObject.GetComponent <PlayerMovement> ().powerUpBoxScript.StartBoxMovement();
            }
		}
    }

	void OnTriggerExit2D(Collider2D collision){
        collided = false;
		animator.SetBool ("colliding", false);
		animator.enabled = true;
	}


}
