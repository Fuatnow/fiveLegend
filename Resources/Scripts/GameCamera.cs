using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour 
{
	public float winWidth = 0;
	public float winHeight = 0;
	public static GameCamera instance = null;
	
	float devHeight = 12.8f;
	float devWidth = 7.2f;

	// Use this for initialization
	void Awake () 
	{
		Singleton.getInstance ("gameConfig");
		instance = this;
		//float screenWidth = Screen.width;
		float screenHeight = Screen.height;
		
		Debug.Log ("screenHeight = " + screenHeight);
		
		//this.GetComponent<Camera>().orthographicSize = screenHeight / 200.0f;
		
		float orthographicSize = this.GetComponent<Camera> ().orthographicSize;
		
		float aspectRatio = Screen.width * 1.0f / Screen.height;
		
		float cameraWidth = orthographicSize * 2 * aspectRatio;
		
		Debug.Log ("cameraWidth = " + cameraWidth);
		
		if (cameraWidth < devWidth)
		{
			orthographicSize = devWidth / (2 * aspectRatio);
			Debug.Log ("new orthographicSize = " + orthographicSize);
			this.GetComponent<Camera> ().orthographicSize = orthographicSize;
		}

		winHeight = orthographicSize * 200;
		winWidth = winHeight * aspectRatio;
		print ("winHeight:" + winHeight + " winWidth:" + winWidth);
	}

	public float getDisResolutionHeight()
	{
		return winHeight/100 - devHeight;
	}
}