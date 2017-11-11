using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovementSound : MonoBehaviour {

    AudioSource audioSource;

	void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.loop = true;
    }

    void Update () {
        PlayerMovement player = transform.parent.GetComponent<PlayerMovement>();
        Rigidbody2D body = transform.parent.GetComponent<Rigidbody2D>();

        float overallSpeed = Vector2.Distance(body.velocity, Vector2.zero); //speed used for capping the max speed in PlayerMovement class

        audioSource.pitch = overallSpeed / player.maxSpeed * (1f - 0.1f) + 0.1f - Mathf.PingPong(Time.time/10f, 0.25f); //uses that speed to change the pitch/tempo. The ping pong is to make it less annoying by ossilating between high and low frequencies

        if (Mathf.Abs(overallSpeed) <= 5) audioSource.pitch = 0; //it should not be playing if the speed is that slow
	}
}
