using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Stores general stuff (player postion, laps etc)
/// </summary>
public class Player : MonoBehaviour {

	public int playerNum = 0; //player 1 or 2 (or more later with lan + online multiplayer)

	List<int> trackPiecesOn = new List<int>(); //what track pieces the player is currently on

	public Image placement; //the gameobject that shows what place you are in (1st, 2nd)

	public Sprite[] placements; //the sprites for 1st and second

    public Text lapsText; //text showing the laps

	public int laps = 1; //how many laps have the player done

    public int pieceWidth = 896; //how wide/tall (it is a square) a piece is

    public Text win, lose; //the Text to show if you win

    public Button mainMenu; //mainMenu button to enable when the win/lose screen appears

	void Start () {
		
	}
	
	void Update () {
		Player otherPlayer = GameController.instance.players [(playerNum + 1) % 2].GetComponent <Player> ();

		List<GameObject> tracks = GameController.instance.tracks;
		if (tracks.Count < 1) //tracks have not been generated yet
			return;

        //check who is first
        int playerLaps = laps;
        int otherPlayerLaps = otherPlayer.laps;

        // laps are incremented when you reach trackpiece 1, so it must add to it here to know which player is ahead
        if (GetCurrentTrackpiece() == 0) playerLaps++;
        if (otherPlayer.GetCurrentTrackpiece() == 0) otherPlayerLaps++;

        if ((GetCurrentTrackpiece() > otherPlayer.GetCurrentTrackpiece() && playerLaps == otherPlayerLaps) || playerLaps > otherPlayerLaps) {
			placement.sprite = placements [0];
			otherPlayer.placement.sprite = placements [1]; //set that this player is first
		} else if (GetCurrentTrackpiece() == otherPlayer.GetCurrentTrackpiece() && playerLaps == otherPlayerLaps) {
            //if they are on the same track piece and the same lap, then it has to resort to the distance to the next track piece

			float distance = Vector2.Distance (transform.position, tracks[(GetCurrentTrackpiece() + 1) % tracks.Count].transform.position);

			float otherPlayerDistance = Vector2.Distance (otherPlayer.transform.position, tracks[(GetCurrentTrackpiece() + 1) % tracks.Count].transform.position);

			if (distance < otherPlayerDistance) {
				placement.sprite = placements [0];
				otherPlayer.placement.sprite = placements [1];
			} else {
				placement.sprite = placements [1];
				otherPlayer.placement.sprite = placements [0];
			}
		}else {
            //if the player is a lap ahead, then it is first

			placement.sprite = placements [1];
			otherPlayer.placement.sprite = placements [0];
		}
	}

    /// <summary>
    /// The function is used since the player could be on multiple track pieces at once
    /// </summary>
    /// <returns></returns>
	public int GetCurrentTrackpiece(){
		int index = -1;
		foreach (int trackpiece in trackPiecesOn) {
			if (trackpiece > index) {
				index = trackpiece;
			}
		}

        if (index == -1) index = 1; //1 is default

		return index;
	}

	public void AddTrackpiece(int index){ //Add a track piece that this player is currently on
        if (index == 1 && GetCurrentTrackpiece() == 0) { //once you cross to the second one it is the finish line
            IncrementLaps(1);
        }

        trackPiecesOn.Add (index);
	}

	public void RemoveTrackpiece(int index){ //remove a track piece that this player is currently on
        trackPiecesOn.Remove (index);

        if (index == 1 && GetCurrentTrackpiece() == 0) { //player has gone backwards through the start
            IncrementLaps(-1);
        }
    }

    public void IncrementLaps(int amount) {

        if(laps >= GameController.instance.totalLaps && GameController.instance.gameStarted) {
            //this player won

            Player otherPlayer = GameController.instance.players[(playerNum + 1) % 2].GetComponent<Player>();

            win.gameObject.SetActive(true);
            otherPlayer.lose.gameObject.SetActive(true);

            mainMenu.gameObject.SetActive(true);
            otherPlayer.mainMenu.gameObject.SetActive(true);

            GameController.instance.gameStarted = false; //game is over

            return;
        }

        if (!GameController.instance.gameStarted) {
            return;
        }

        laps += amount;

        lapsText.text = laps + "/" + GameController.instance.totalLaps;
    }
}
