using UnityEngine;
using System.Collections;

public class UpSide : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
//		var deimensions = gameObject.GetComponent<tk2dSlicedSprite> ().dimensions;
//		var bgHeight = deimensions.y;
//		gameObject.GetComponent<tk2dSlicedSprite> ().anchor = tk2dBaseSprite.Anchor.UpperCenter;
		var bgPos = transform.position;
		float pY = GameCamera.instance.winHeight/200 - 6.4f;
		bgPos.y += pY;
		transform.position = bgPos;
	}
}
