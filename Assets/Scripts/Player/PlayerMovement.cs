using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public int playerNum = 1; //player 1 or 2 (or more later with lan + online multiplayer)

	/// <summary>
	/// Stores the powerup box, and the active powerup
	/// </summary>
	public PowerUpBoxScript powerUpBoxScript;

	Rigidbody2D body;

    //speeds
	float speed = 70000;
	float rotSpeed = 550000;

	//maximum velocity
	public float maxSpeed = 700;

	//max rotation speed
	float maxRotSpeed = 200;

	//friction on the rotation
	float rotFriction = 120;

    //movement friction
	float friction = 750;

	public float lastCollision; //time of last collision, used to make a cooldown after hitting a turtle
    float collisionCooldown = 1.5f; //how long in seconds to wait after collision

	void Start () {
		body = GetComponent <Rigidbody2D>();
	}

	void Update () {

		//accellerate
		if (Forward () && Time.time - lastCollision >= collisionCooldown && GameController.instance.gameStarted) {
			//add force based on direction
			body.AddForce (transform.up * Time.deltaTime * speed);

			CapMaxSpeed ();
		}

		//brake/reverse
		if (Reverse() && Time.time - lastCollision >= collisionCooldown && GameController.instance.gameStarted) {
			
			//now it is the reverse button
			body.AddForce (- transform.up * Time.deltaTime * speed);

			CapMaxSpeed();
		}

		//add rotation based on key press
		if (Left () && Time.time - lastCollision >= collisionCooldown && GameController.instance.gameStarted) {

			body.AddTorque (Time.deltaTime * rotSpeed);

		}
		if (Right () && Time.time - lastCollision >= collisionCooldown && GameController.instance.gameStarted) {
			body.AddTorque (-Time.deltaTime * rotSpeed);

		}

		//cap rotation speed
		CapMaxRotSpeed();

		//add friction
		RotationFriction (); //friction can be done while the game is stopped
		MovementFriction ();
	}

	void CapMaxSpeed(){
		//the max distance they can move in a second is maxSpeed (in a second because everything is multiplied by Time.deltaTime)
		float distance = Vector2.Distance (body.velocity, Vector2.zero);
		if (distance >= maxSpeed) {
			float angle = MathHelper.GetRadians (body.velocity);
			body.velocity = MathHelper.RadianToVector2 (angle) * maxSpeed;
		}
	}

	void CapMaxRotSpeed(){
		//make sure torque is not over the maxRotSpeed
		if (body.angularVelocity >= maxRotSpeed) {
			body.angularVelocity = maxRotSpeed;
		}

		//make sure torque is not under the maxRotSpeed
		if (body.angularVelocity <= -maxRotSpeed) {
			body.angularVelocity = -maxRotSpeed;
		}
	}

	void MovementFriction(){
		
		//movement friction (because doing it manually gives more control than using physics materials)

		float angle = MathHelper.GetRadians (body.velocity);
		body.velocity -= new Vector2 (Mathf.Cos (angle) * friction * Time.deltaTime, Mathf.Sin (angle) * friction * Time.deltaTime);
	}

	void RotationFriction(){
		//rotation friction
		if (body.angularVelocity > 0) {
			//if more than zero, it needs to be made lower
			body.angularVelocity -= rotFriction * Time.deltaTime;

			if (body.angularVelocity < 0)
				body.angularVelocity = 0;

		} else if (body.angularVelocity < 0) {
			//if less than zero, it needs to be made higher
			body.angularVelocity += rotFriction * Time.deltaTime;

			if (body.angularVelocity > 0)
				body.angularVelocity = 0;
		}
	}

	void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag.Equals("Greenshell") || collision.gameObject.tag.Equals("Redshell")) {
            lastCollision = Time.time; //this makes it so that the player cannot move for a bit after colliding
        }
	}

    //these functions are used to make it so that the different players have different controls
	bool Forward(){
		if (playerNum == 0) {
			return Input.GetKey (KeyCode.W);
		} else {
			return Input.GetKey (KeyCode.I);
		}
	}

	bool Reverse(){
		if (playerNum == 0) {
			return Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.S);
		} else {
			return Input.GetKey (KeyCode.RightShift) || Input.GetKey (KeyCode.K);
		}
	}

	bool Right(){
		if (playerNum == 0) {
			return Input.GetKey (KeyCode.D);
		} else {
			return Input.GetKey (KeyCode.L);
		}
	}

	bool Left(){
		if (playerNum == 0) {
			return Input.GetKey (KeyCode.A);
		} else {
			return Input.GetKey (KeyCode.J);
		}
	}
}
