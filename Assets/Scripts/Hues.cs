using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Hues : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		float h;
		float s;
		float v;
		Color.RGBToHSV (GetComponent<Text>().color, out h, out s, out v);
		//adjust hue
		Color adjustedColor = Color.HSVToRGB (h + 0.4f * Time.deltaTime, s, v);
        GetComponent<Text>().color = adjustedColor;
		
	}
}
