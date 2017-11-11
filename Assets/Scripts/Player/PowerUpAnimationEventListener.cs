using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpAnimationEventListener : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

    /// <summary>
    /// Calls the function in the powerupboxscript so that it knows when animations ended
    /// </summary>
	public void AnimationEnded() {
		transform.parent.parent.parent.parent.GetComponent <PlayerMovement> ().powerUpBoxScript.AnimationEnded (gameObject); //Player GameObject
	}
}
