using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour {

    Animator animator;

    AudioSource audioSource;

    public AudioClip finalBeep; //the beep in the end is different

	void Start () {
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
	}
	
	void Update () {
		
	}

    public void PlaySound() {
        if(audioSource != null) { //it only needs to be played by one of the countdowns, so if it is null, then don't play
            audioSource.Play();
        }
    }

    public void TimerEnds() { //called when 1 is over, not when go is over
        GameController.instance.gameStarted = true;

        if (audioSource != null) {
            audioSource.PlayOneShot(finalBeep);
        }
    }

    public void AnimationEnded() {
        gameObject.SetActive(false);
    }
}
