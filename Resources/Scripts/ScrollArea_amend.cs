using UnityEngine;
using System.Collections;

public class ScrollArea_amend : MonoBehaviour {

	public GameObject bottomMask = null;
	// Use this for initialization
	void Start () {
	
		var area = gameObject.GetComponent<tk2dUIScrollableArea> ();
		var preLen = area.VisibleAreaLength;
		var dis = GameCamera.instance.getDisResolutionHeight ();
		preLen += dis;
		area.VisibleAreaLength = preLen;

		if (bottomMask != null) 
		{
			var pos = bottomMask.transform.position;
			pos.y -= dis;
			bottomMask.transform.position = pos;
		}
	}
}
