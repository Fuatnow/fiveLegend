using UnityEngine;
using System.Collections;

public class RankItem : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		var gameCamera = GameObject.Find ("camera").GetComponent<GameCamera> ();
		float winWidth = gameCamera.winWidth;

		var bg = transform.Find ("bg").gameObject.GetComponent<tk2dSlicedSprite> ();
		var curD = bg.dimensions;
		curD.x = winWidth-10f;
		bg.dimensions = curD;
	}
}
