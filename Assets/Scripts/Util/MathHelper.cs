using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper : MonoBehaviour {

    /// <summary>
    /// Converts the angle into a Vector2
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
	public static Vector2 RadianToVector2(float angle){
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}

    /// <summary>
    /// Converts the angle into a Vector2
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
	public static Vector2 DegreeToVector2(float angle){
		return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
	}

    /// <summary>
    /// Converts the angle into a Vector3
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
	public static Vector3 RadianToVector3(float angle){
		Vector2 vector = RadianToVector2(angle);
		return new Vector3 (vector.x, vector.y);
	}

    /// <summary>
    /// Converts the angle into a Vector3
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
	public static Vector3 DegreeToVector3(float angle){
		Vector2 vector = DegreeToVector2(angle);
		return new Vector3 (vector.x, vector.y);
	}

	public static float GetRadians(Vector2 movement){
		return Mathf.Atan2 (movement.y, movement.x);
	}

	/// % in c# is remainder not modulo. So, -1 % 4 == -1. With this function mod(-1, 4) == 3
    /// Only used if there are going to be negatives involved, otherwise just use normal %
	public static int mod(int x, int m) { 
		int r = x%m;
		return r<0 ? r+m : r;
	}
}
