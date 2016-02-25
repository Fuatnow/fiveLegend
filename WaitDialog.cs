using UnityEngine;
using System.Collections;
using DG.Tweening;
public class WaitDialog : MonoBehaviour 
{
	public tk2dTextMesh message = null;
	public tk2dSprite waitIcon = null;
	// Use this for initialization
	void Awake () 
	{
		
		if(Application.systemLanguage == SystemLanguage.English)
		{
			message.text = "Loading, please keep the network flow ...";
		}
	}

	void Start () 
	{
		Sequence run = DOTween.Sequence();
		Tween rot = waitIcon.transform.DORotate(new Vector3(0, 0, -360), 5, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		run.Append(rot).SetLoops(-1);
	}


	public void closeCallBack()
	{
		Debug.Log ("closeCallBack");
		transform.parent.gameObject.SetActive(false);
	}
}
