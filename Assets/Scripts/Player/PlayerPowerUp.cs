using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUp : MonoBehaviour {

	public int playerNum = 1; //player 1 or 2 (or more later with lan + online multiplayer)

    //the power up box script for this player, taken from PlayerMovement class
    PowerUpBoxScript powerUpBoxScript;

    //prefabs for the power up objects (only for greenshell and redshell)
    public GameObject[] powerUpPrefabs = new GameObject[0];

    //star mdoe
    bool starMode = false; //should this start changing hues
    float starModeStartTime;
    float lastKnockback; //too make sure knockbacks don't happen one after the other (star power)

    //ghost mode
    bool ghostMode = false; //should it animate flashing and go through stuff
    float ghostModeStartTime;
    
    Rigidbody2D body;

	void Start () {
        body = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
        if(powerUpBoxScript == null) {
			powerUpBoxScript = GetComponent <PlayerMovement> ().powerUpBoxScript;
        }

		if (!powerUpBoxScript.empty && !powerUpBoxScript.scrolling && Item()) {
            //based on the chosen sprite, and action is taken

            switch (powerUpBoxScript.chosenSprite) {
                case 0: //star power
                    starMode = true;
                    starModeStartTime = Time.time;
                    powerUpBoxScript.reset();
                    break;
                case 1: //greenshell
                    GameObject powerUp = Instantiate(powerUpPrefabs[powerUpBoxScript.chosenSprite]);

                    //moves greenshell forward in direction of player
                    powerUp.transform.position = transform.position + transform.up * (GetComponent<SpriteRenderer>().bounds.size.y / 2 + powerUp.GetComponent<SpriteRenderer>().bounds.size.y / 2 + 50); //50 is buffer size
                    powerUp.GetComponent<Rigidbody2D>().AddForce(transform.up * 70000);

                    powerUpBoxScript.reset();
                    break;
                case 2: //red shell
                    GameObject powerUp1 = Instantiate(powerUpPrefabs[powerUpBoxScript.chosenSprite]);

                    //sets the target for the redshell
                    powerUp1.GetComponent<Redshell>().target = GameController.instance.players[(playerNum+1) % 2];

                    //moves redshell forward in direction of player
                    powerUp1.transform.position = transform.position + transform.up * (GetComponent<SpriteRenderer>().bounds.size.y / 2 + powerUp1.GetComponent<SpriteRenderer>().bounds.size.y / 2 + 50); //50 is buffer size
                    powerUp1.GetComponent<Rigidbody2D>().AddForce(transform.up * 30000);

                    powerUpBoxScript.reset();
                    break;
                case 3:
                    //changes the layer so that collisions don't happen against obstacles and that the player is not visible in the othe rplayers camera
                    gameObject.layer = LayerMask.NameToLayer("Player" + (playerNum + 1) + " Ghost Mode"); //The ghost mode labels do not collide with obstacles and player 1 can't see player 2s ghost mode and vice-versa

                    ghostMode = true;
                    ghostModeStartTime = Time.time;
                    powerUpBoxScript.reset();
                    break;
            }
        }

        if (starMode) {

            //changes hue over time
            float h;
            float s;
            float v;
            Color.RGBToHSV(GetComponent<SpriteRenderer>().color, out h, out s, out v);
            //adjust hue
            Color adjustedColor = Color.HSVToRGB(h + 0.4f * Time.deltaTime, s, v);
            GetComponent<SpriteRenderer>().color = adjustedColor;

            if(Time.time - starModeStartTime >= 15) {
                //reset color if time passed
                GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
                starMode = false;
            }
        }

        if (ghostMode) {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            //set the color to a slightly alpha color (ghostish effect)
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.25f);

            //this is called every frame because you could have star power AND ghost mode at the same time

            if (Time.time - ghostModeStartTime >= 15) {
                ghostMode = false;
                
                //reset layers and colors
                gameObject.layer = LayerMask.NameToLayer("Player");
                GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
            }
        }
	}

    void OnCollisionEnter2D(Collision2D collision) {

        //if this player collides with a player without star power
        if (starMode && collision.gameObject.tag.Equals("Player") && !collision.gameObject.GetComponent<PlayerPowerUp>().starMode && Time.time - lastKnockback >= 0.5f) {

            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.Normalize(body.velocity) * 80000);
            collision.gameObject.GetComponent<Rigidbody2D>().angularVelocity += 5000000; //doesn't actually rotate that much because of tha maximum rotation speed set in the PlayerMovement class

            collision.gameObject.GetComponent<PlayerMovement>().lastCollision = Time.time;

            lastKnockback = Time.time; //if the player bounces back and touches the player, it should not knockback for a bit, or else it causes bugs
        }

    }

    //controls for each player

    bool Item(){
		if (playerNum == 0) {
			return Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.E);
		} else {
			return Input.GetKey (KeyCode.Return) || Input.GetKey (KeyCode.O);
		}
	}
}
