using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPieceScript : MonoBehaviour {

	public int index = 0; //what index of track piece is this on

	void Start () {
		
	}
	
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag.Equals("Player")) { //used to determine which track the player is on
			collider.gameObject.GetComponent <Player> ().AddTrackpiece(index);
        }

        if (collider.gameObject.tag.Equals("Redshell")) { //used to determine which track the redshell is on
            collider.gameObject.GetComponent<Redshell>().AddTrackpiece(index);
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
		if (collider.gameObject.tag.Equals("Player")) { //used to deetermine which track the player is on
            collider.gameObject.GetComponent <Player> ().RemoveTrackpiece(index);
		}

        if (collider.gameObject.tag.Equals("Redshell")) { //used to determine which track the redshell is on
            collider.gameObject.GetComponent<Redshell>().RemoveTrackpiece(index);
        }
    }
}
