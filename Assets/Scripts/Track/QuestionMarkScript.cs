using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionMarkScript : MonoBehaviour {

	bool flip; //direction of size pulsing

	float pulseSpeed = 0.3f; //for the growing and shrinking of the question mark

	void Start () {
		
	}
	
	void Update () {

		//pulse question mark size
		int multiplier = 1;
		if (flip)
			multiplier = -1;

		transform.localScale = new Vector2(transform.localScale.x + pulseSpeed * multiplier * Time.deltaTime, transform.localScale.y + pulseSpeed * multiplier * Time.deltaTime);
		if (transform.localScale.x > 1.2f) //x and y are the same so only check for one
			flip = true;
		if (transform.localScale.x < 0.8)
			flip = false;
	}
}
