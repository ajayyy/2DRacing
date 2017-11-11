using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static GameController instance; //the instance of the gamecontroller, variable used by other classes to access it

    public bool gameStarted = false; //has the game started (set by the countdown)

	public GameObject[] trackPrefabs = new GameObject[0]; //the track pieces to spawn
	public GameObject[] obstaclePrefabs = new GameObject[0]; //the obstacles to spawn

    public List<GameObject> tracks = new List<GameObject> (); //all the spawned tracks will be added to this list when generation is finished

    public GameObject box; // question mark box prefab for spawning it

	public GameObject[] players = new GameObject[0]; //list of players

	static System.Random rand = new System.Random (); //random object since you can't use UnityEngine.Random in other threads

	//for tracks
	public List<int> directions = new List<int>(); // direction from 1 - 4 (going clockwise)
    public List<Vector2> positions = new List<Vector2>(); //position where 1 unit is one track size
    public List<int> pieces = new List<int>(); //what type of pieces (0,1,2) (straight,right,left)

	Thread generationThread; //the thread used for generation (to not clog up the main thread)

	//to know when the thread is done
	bool doneGenerating = false;

	//to tell thread to end
	bool ending = false;

    //Total laps for this map
    public int totalLaps = 3;

    // The text to tell the user it is generating
    public GameObject[] generatingText = new GameObject[0];

    //The countdown game object to enable after generation
    public GameObject[] countdownText = new GameObject[0];


    void Awake () {
		if (instance == null || instance == this) {
			instance = this;
		} else {
			Destroy (gameObject);
		}

        ThreadStart generationRef = new ThreadStart(GenerateEverything); //start the generation thread
		generationThread = new Thread(generationRef);
		generationThread.Start();


    }

    void Update () {
		if (doneGenerating) {
			SpawnEverything (); //spawn once the generation is done, this must be done on the main thread since it uses unity functions, and unity functions are not thread safe
			doneGenerating = false;

            //disable generation text
            foreach(GameObject gameObject in generatingText) {
                gameObject.SetActive(false);
            }

            //enable countedown
            foreach (GameObject gameObject in countdownText) {
                gameObject.SetActive(true);
            }
        }
	}

	void OnDestroy() {
		if (generationThread.IsAlive) {
//			generationThread.Interrupt ();
//			generationThread.Abort ();
			ending = true;
			generationThread.Join (); //kill the thread if it is still alive, setting ending to true is nessesary, or else the hanging thread is left, and the game cannot be run again from the editor without killing it
		}
	}

    //Questionmark Box Spawning
    public void SpawnBoxes() {
        List<Box> boxes = new List<Box>(); 

        while (true) {
            boxes = GenerateBoxSpawns();

            bool containsMultiBox = false; //was a group of boxes spawned (type of not 1)
            foreach (Box currentBox in boxes) {
                if (currentBox.type != 1) {
                    containsMultiBox = true;
                    break;
                }
            }

            if (containsMultiBox)
                break;
        }

        foreach (Box currentBox in boxes) {
            GameObject currentBoxObject = Instantiate(box);
            currentBoxObject.transform.position = currentBox.pos;
            currentBoxObject.transform.eulerAngles = currentBox.rotation;
        }
    }

    /// <summary>
    /// generates the spawns
    /// </summary>
    public List<Box> GenerateBoxSpawns() {
        List<Box> boxes = new List<Box>(); //box data is stored here (originally this was split up between generation and spawning, but it didn't work right
        //the spawning of boxes and obstacles take so little time it's ok to spawm them on the main thread

        for (int i = 0; i < tracks.Count; i++) {
            if (i < 3)
                continue; //don't do anything to the first few track pieces

            GameObject trackPiece = tracks[i];

            if (rand.Next(0, 3) == 0) { // 1 in 2 chance of a box

                if (!trackPiece.name.Contains("Straight")) //only spawn them on straights
                    continue;

                if (rand.Next(0, 3) == 0) {
                    Box[] currentBoxes = new Box[5];

                    for (int q = 0; q < currentBoxes.Length; q++) {
                        currentBoxes[q] = new Box();

                        //set position
                        currentBoxes[q].pos = trackPiece.transform.position;

                        currentBoxes[q].pos += MathHelper.DegreeToVector3(trackPiece.transform.eulerAngles.z) * 170 * (q - 2);

                        //make it rotated with the track
                        currentBoxes[q].rotation = trackPiece.transform.eulerAngles;

                        currentBoxes[q].pos += MathHelper.DegreeToVector3(trackPiece.transform.eulerAngles.z - 90) * 400;

                        currentBoxes[q].type = 1; //to show that it is not a single box
                    }

                    foreach (Box currentBox in currentBoxes) {
                        boxes.Add(currentBox);
                    }


                } else {

                    Box currentBox = new Box();

                    //choose random spawn location (left, middle, right)
                    int spawnPosition = rand.Next(-1, 2);
                    //TODO make this formations instead of positions

                    //set position
                    currentBox.pos = trackPiece.transform.position;

                    currentBox.pos += MathHelper.DegreeToVector3(trackPiece.transform.eulerAngles.z) * Mathf.Max(box.GetComponentInChildren<SpriteRenderer>().bounds.size.x, box.GetComponentInChildren<SpriteRenderer>().bounds.size.y) * spawnPosition;

                    //make it rotated with the track
                    currentBox.rotation = trackPiece.transform.eulerAngles;

                    currentBox.pos += MathHelper.DegreeToVector3(trackPiece.transform.eulerAngles.z - 90) * 400;

                    boxes.Add(currentBox);

                }
            }
        }

        return boxes;
    }

    //Obstacle Spawning
    public void SpawnObstacles() {
        for (int i = 0; i < tracks.Count; i++) {
            if (i < 3)
                continue; //don't do anything to the first few track pieces

            GameObject trackPiece = tracks[i];

            if (rand.Next(0, 2) == 0) { // 1 in 2 chance of an obstacle
                int obstacleIndex = rand.Next(0, obstaclePrefabs.Length);

                GameObject obstacle = Instantiate(obstaclePrefabs[obstacleIndex]);

                //choose random spawn location (left, middle, right)
                int spawnPosition = rand.Next(-1, 2);

                if (obstacleIndex == 3) {
                    spawnPosition = 0; //it is the moving wall, it always starts in the middle
                }

                //set position
                obstacle.transform.position = trackPiece.transform.position;

                if (trackPiece.name.Contains("Straight")) { //if it's a straight peice (with turn peices having it not in the middle makes it too hard)
                                                            //move it based on spawn location
                    obstacle.transform.position += MathHelper.DegreeToVector3(trackPiece.transform.eulerAngles.z) * Mathf.Max(obstacle.GetComponentInChildren<SpriteRenderer>().bounds.size.x, obstacle.GetComponentInChildren<SpriteRenderer>().bounds.size.y) * spawnPosition;

                    //make it rotated with the track
                    obstacle.transform.eulerAngles = trackPiece.transform.eulerAngles;

                    print(spawnPosition + " spawnPosition");
                }

                i++; //don't have obstacles 2 times in a row
            }
        }
    }

    public void GenerateEverything(){
        bool failed = false;
        do { //generate once, and if it fails, do it again
            failed = false;

            Tuple<int, bool> lastData = PreGenerateTracks(Vector2.zero, 0, new Vector2(5, 5), 10);

            if (!failed) failed = lastData.Item2;

            lastData = PreGenerateTracks(new Vector2(5, 5), lastData.Item1, new Vector3(-5, 5), 20);

            if (!failed) failed = lastData.Item2;

            PreGenerateTracks(new Vector2(-5, 5), lastData.Item1, Vector2.zero, 30);

            if (!failed) failed = lastData.Item2;
        } while (failed && !ending); //if failed try again

        if (directions[directions.Count - 1] == 3) { // left
            pieces[0] = 1; //put a turn right piece to make it go up
        }

        if (directions[directions.Count - 1] == 1) { // right
            pieces[0] = 2; //put a turn left piece to make it go up
        }

        doneGenerating = true;
		print ("done generating");
	}

	public void SpawnEverything(){
		print ("spawning");

        SpawnTracks();

        SpawnBoxes();

        SpawnObstacles();

	}

	//spawns the tracks
	public void SpawnTracks(){
		Vector2 pos = Vector2.zero;

        List<Vector2> spawnedPositions = new List<Vector2>(); //temperary variable that positions will be set to in the end

		for (int i = 0; i < directions.Count && !ending; i++) { //directions.Count is 1 shorter, so it is used with all mod calls
			GameObject trackPiece = Instantiate (trackPrefabs[pieces[i]]);

			tracks.Add (trackPiece);

			trackPiece.GetComponent<TrackPieceScript> ().index = i; //to make the track piece know what index it is for knowing what trackpieces a player is on

			trackPiece.transform.position = pos * (896-26);
			trackPiece.transform.Rotate (new Vector3(0, 0, directions[MathHelper.mod(i-1, directions.Count)] * (-90)));

            // enables curve if it is needed, and disables the straight piece already there
			if (pieces [(i+1) % directions.Count] == 1 && pieces [i] != 1 && pieces[(i+2) % directions.Count] != pieces[(i+1) % directions.Count]) { //if pieces[i+1] is a right turn, this piece isn't, and the piece [i+2] isn't i+1
				Transform curve = trackPiece.transform.Find ("Side 1/curve");
				curve.gameObject.SetActive (true);
				Transform oldPiece = trackPiece.transform.Find ("Side 1/bar -3");
				oldPiece.gameObject.SetActive (false);
			}else if (pieces [(i + 1) % directions.Count] == 2 && pieces [i] != 2 && pieces[(i+2) % directions.Count] != pieces[(i+1) % directions.Count]) { //if pieces[i+1] is a left turn, this piece isn't, and the piece [i+2] isn't i+1
				Transform curve = trackPiece.transform.Find ("Side 0/curve");
				curve.gameObject.SetActive (true);
				Transform oldPiece = trackPiece.transform.Find ("Side 0/bar -3");
				oldPiece.gameObject.SetActive (false);
			}

			if (pieces [MathHelper.mod(i - 1, directions.Count)] != 0 && pieces[i] != pieces[MathHelper.mod(i - 1, directions.Count)]) { //the peice[-1] is a turn, and this peice is not the same turn
				Transform oldPiece = null;
				if (pieces [i] != 0) {
					oldPiece = trackPiece.transform.Find ("Side 2/bar 3");
				} else {
					if (pieces [MathHelper.mod(i - 1, directions.Count)] == 1) {
						oldPiece = trackPiece.transform.Find ("Side 1/bar 3");
					} else {
						oldPiece = trackPiece.transform.Find ("Side 0/bar 3");
					}
				}
				oldPiece.gameObject.SetActive (false);
			}

            spawnedPositions.Add(pos);

            pos += GetMovementFromDirection (directions [MathHelper.mod(i, directions.Count)]);
		}

        positions = spawnedPositions;
	}

	//Terrain Spawning, returns last rotation
	public Tuple<int, bool> PreGenerateTracks(Vector2 startPos, int startRotation, Vector2 target, int maxMovements){

		Vector2 pos = new Vector2 (startPos.x, startPos.y);//keep track of location

        //used to be able to reset changes made
		List<Vector2> startPositions = new List<Vector2> (positions);
		List<int> starDirections = new List<int> (directions);
		List<int> startPieces = new List<int> (pieces);

        int maxGenerations = 35000; //after this time, it will give up and try again (maybe the generator blocked off access to the target)
        int generations = 0;

		while (!ending) {
			bool complete = GenerateTrackToTarget (maxMovements, target, pos, startRotation);
			if (complete)
				break;

            //if it failed, reset the changes and try again
            directions = new List<int>(starDirections);
            pieces = new List<int>(startPieces);
            positions = new List<Vector2>(startPositions);

            generations++;
            if (generations > maxGenerations) return new Tuple<int, bool>(0, true); //it failed, too many attemps

        }

		return new Tuple<int, bool>(directions [directions.Count-1], false); //returns if it failes, and the last direction (to know where to start next time
	}


	/**
	 * Will return status
	 */
	public bool GenerateTrackToTarget(int maxMovements, Vector2 target, Vector2 pos, int startRotation){
		Vector2 startPos = new Vector2(pos.x, pos.y);

		int movements = 0;

        //the start direction should be the start rotation, or whatever it was left off at last time
		int direction = startRotation;

		if(positions.Count == 0) { //only do this for the first piece
			pieces.Add (0);

            positions.Add (startPos);
		}

		while (!ending) {
			Vector2 startFramePos = new Vector2 (pos.x, pos.y); // the pos at the start of this frame of generation
			int startFrameDirection = direction;

			int turnDirection = rand.Next (0, 4)-1;
			if (turnDirection < 0)
				turnDirection = 0; // more likely to get straights

            if(pieces.Count <= 1) { //second piece should always be straight (it's going to be the start piece)
                turnDirection = 0;
            }

			pieces.Add (turnDirection);

			//check if we've already been there
//			if(GetMovementFromDirection (direction)

			pos += GetMovementFromDirection (direction); //move direction of last time (even if there is a turn piece, it has to be placed forward by one
            positions.Add (pos);
			directions.Add (direction);

            //if turn is right, add to the direction, if left then remove
			switch (turnDirection) {
			case 1:
				direction++;
				direction = MathHelper.mod (direction, 4); //cap direcition at 4
                break;
			case 2:
				direction--;
				direction = MathHelper.mod (direction, 4); //cap direcition at 4
                break;
			}

			if (pos.Equals (target)) {
                pieces[pieces.Count - 1] = 0; //last piece should always be a straight

				pos = startPos; //reset pos back to start pos
				return true;
			}

			if (positions.Contains (pos + GetMovementFromDirection (direction)) && pos + GetMovementFromDirection (direction) != target) { //can't be target or else it will be in an infinite loop
				pos = startFramePos;
				direction = startFrameDirection;
                positions.RemoveAt(positions.Count-1);
				pieces.RemoveAt(pieces.Count-1);
				directions.RemoveAt(directions.Count-1);

                //reset last frame and try again
			}

			movements++;
			if (movements > maxMovements) {
				pos = startPos;
				return false; //reset pos back to start pos so that it can generate again from the same point
            }
		}

		return false;

	}

    /// <summary>
    /// Returns a movement based on the direction (1-4 clockwise)
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
	private Vector2 GetMovementFromDirection(int direction){
		switch (direction) {
		case 0:
			return new Vector2 (0, 1);
		case 1:
			return new Vector2 (1, 0);
		case 2:
			return new Vector2 (0, -1);
		case 3:
			return new Vector2 (-1, 0);
		}
		return Vector2.zero;
	}
}
