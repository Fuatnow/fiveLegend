using UnityEngine;
using System.Collections;

public class InitGameData : MonoBehaviour {


	void Awake()
	{
		initCurrentPlatform ();
	}

	// Use this for initialization
	void Start () 
	{
		Application.LoadLevel ("Game");
//		ScenesManager.LoadScene ();
	}

	
	
	void initCurrentPlatform()
	{
		var platform = "2x";
		
		if(Application.systemLanguage == SystemLanguage.English)
		{
			platform = "2x";
		}
		else
		{
			platform = "1x";
		}
		
		tk2dSystem.CurrentPlatform = platform;
	}
}
