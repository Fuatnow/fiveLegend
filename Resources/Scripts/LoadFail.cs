using UnityEngine;
using System.Collections;
using DG.Tweening;
public class LoadFail : MonoBehaviour {

	public tk2dTextMesh message = null;
	public tk2dSlicedSprite bg = null;
	// Use this for initialization
	void Awake () 
	{

		if(Application.systemLanguage == SystemLanguage.English)
		{
			message.text = "Failed to load , check whether the network connection .";
		}
	}

	void Start () 
	{
		reLoadFailAct ();
	}




	public void reLoadFailAct()
	{
		gameObject.SetActive (true);
		Sequence run = DOTween.Sequence();
		var pos = bg.transform.position;
		pos.y = -7.2f;
		bg.transform.position = pos;
		var move = bg.transform.DOMoveY (0,0.3f);
		var fade = DOTween.ToAlpha (() => bg.color, x => bg.color = x, 0.2f, 0.2f);
		run.Append(move).AppendInterval (1.8f).Append (fade).AppendCallback (() => {
			bg.color = new Color(bg.color.r,bg.color.g,bg.color.b,1.0f);
			gameObject.SetActive(false);
			transform.parent.gameObject.SetActive(false);
		});
	}
}
