using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that holds data about a box when it is created
/// </summary>
public class Box {

	/// <summary>
	/// The position.
	/// </summary>
	public Vector3 pos;

	/// <summary>
	/// The rotation.
	/// </summary>
	public Vector3 rotation;

	/// <summary>
	/// The type of box, part of a group or not
	/// </summary>
	public int type;

	public Box(Vector3 pos, Vector3 rotation, int type){
		this.pos = pos;
		this.rotation = rotation;
		this.type = type;
	}

	public Box(){

	}
}
