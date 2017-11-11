using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redshell : MonoBehaviour {

    Rigidbody2D body; //rigidbody

    Animator animator; //animator

    public GameObject target; //The target for the player to head toward (other player)

    float force = 1000; //the force to use when adding force

    float maxSpeed = 1200; //maximum speed it can go at

    List<int> trackPiecesOn = new List<int>(); //what track pieces the redshell is currently on

    void Start () {
        body = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
	}
	
	void FixedUpdate () {
		if(target != null) { //target will be null if it hasn't been set yet

            Vector3 target = new Vector3(this.target.transform.position.x, this.target.transform.position.y, this.target.transform.position.z);

            int currentTrackpiece = GetCurrentTrackpiece(); //track piece this redshell is on
            int targetTrackpiece = this.target.GetComponent<Player>().GetCurrentTrackpiece(); //target track piece (of the player)
            int tracksLegnth = GameController.instance.tracks.Count; //legnth of the track list (used to not have to write the full legnth)

            if (targetTrackpiece != currentTrackpiece) {
                //distances if going up the track list, and down
                int upDistance = 0;
                int downDistance = 0;

                //adds the track legnth if needed (because something at tracklegnth - 1 would need to go up if the target is at 2
                if(targetTrackpiece < currentTrackpiece) {
                    upDistance = (targetTrackpiece + tracksLegnth) - currentTrackpiece;
                }else {
                    upDistance = targetTrackpiece - currentTrackpiece;
                }

                if(targetTrackpiece > currentTrackpiece) {
                    downDistance = (targetTrackpiece + tracksLegnth) - currentTrackpiece;
                } else {
                    downDistance = currentTrackpiece - targetTrackpiece;
                }

                //depending on which one is bigger, go forward or back
                if(downDistance > upDistance) {
                    target = GameController.instance.tracks[(currentTrackpiece + 1) % tracksLegnth].transform.position;
                } else {
                    target = GameController.instance.tracks[(currentTrackpiece - 1) % tracksLegnth].transform.position;
                }
            }

            float angle = MathHelper.GetRadians(target - transform.position); //the angle that they are at (to turn toward)

            body.AddForce(MathHelper.RadianToVector3(angle) * force); //add force to move toward target

            // the max distance they can move in a second is maxSpeed (in a second because everything is multiplied by Time.deltaTime)
		    float distance = Vector2.Distance(body.velocity, Vector2.zero);
            if (distance >= maxSpeed) {
                float movementAngle = MathHelper.GetRadians(body.velocity);
                body.velocity = MathHelper.RadianToVector2(angle) * maxSpeed;
            }
        }
	}

    void OnCollisionEnter2D(Collision2D collision) {
        animator.SetTrigger("dead");
        body.velocity = Vector2.zero; //when dead, stop moving for the animation
    }

    public void AnimationEnded() {
        Destroy(gameObject);
    }

    /// <summary>
    /// The function is used since the redshell could be on multiple track pieces at once
    /// </summary>
    /// <returns></returns>
	public int GetCurrentTrackpiece() {
        int index = -1;
        foreach (int trackpiece in trackPiecesOn) {
            if (trackpiece > index) {
                index = trackpiece;
            }
        }

        if (index == -1) index = 1; //1 is default

        return index;
    }

    public void AddTrackpiece(int index) { //Add a track piece that this player is currently on
        trackPiecesOn.Add(index);
    }

    public void RemoveTrackpiece(int index) { //remove a track piece that this player is currently on
        trackPiecesOn.Remove(index);
    }
}
