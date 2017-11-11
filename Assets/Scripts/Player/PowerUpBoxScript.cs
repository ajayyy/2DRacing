using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PowerUpBoxScript : MonoBehaviour {

	public Sprite[] powerUpSprites = new Sprite[0]; //the sprites

	public GameObject spriteMask = null; //the sprite mask for the power up box

	int scrollAmount; //how many times it has scrolled so far (used to know when to stop the scrolling)
	public bool scrolling; //if the box is scrolling
    public bool empty = true; //if the box is empty
    public int chosenSprite; //the sprite that was chosen

    AudioSource audioSource; //to play the tick sound

	void Start () {
		if (transform.parent.GetComponent <PlayerMovement> ().powerUpBoxScript == null) {
			transform.parent.GetComponent <PlayerMovement> ().powerUpBoxScript = this;
		} else {
			Destroy (gameObject);
		}

        audioSource = GetComponent<AudioSource>();
	}
	
	void Update () {

	}

	public void reset(){
		empty = true;
		spriteMask.SetActive (false);
	}

	public void AnimationEnded(GameObject icon){
        //choose a random sprite
        int sprite = Random.Range(0, powerUpSprites.Length);

        //set it as the sprite
        icon.GetComponent <Image>().sprite = powerUpSprites[sprite];

		if (scrollAmount > 9) {
			icon.GetComponent <Animator> ().SetTrigger ("done");
        }
        if (scrollAmount == 10) {
            chosenSprite = sprite; //this is the sprite that will end up being the final one
        }
        if (scrollAmount > 10) {
            scrolling = false;

            scrollAmount = 0;
        } else {
            scrollAmount++;
        }

    }

	public void StartBoxMovement(){

		if (scrolling || !empty) //if there is already something there, then don't start again
			return;

        scrolling = true;
        empty = false;

        spriteMask.SetActive (true);

        foreach(Animator animator in spriteMask.GetComponentsInChildren<Animator>()) { //for every animator, restart it
			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("powerup done")) {
				animator.SetTrigger ("restart"); 
			}
        }

	}

}