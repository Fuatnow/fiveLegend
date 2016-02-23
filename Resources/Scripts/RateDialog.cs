using UnityEngine;
using System.Collections;
using Umeng;
using DG.Tweening;
public class RateDialog : MonoBehaviour {

	private tk2dTextMesh title = null;
	private tk2dTextMesh message = null;
	private tk2dTextMesh okLabel = null;
	private tk2dSlicedSprite bg = null;
	private float dialogWidth = 5f;
	private float dialogHeight = 3f;
	//	private float pixel2Meters = 100f;
	
	
	public delegate void OkCallBack<T>(T data);
	private OkCallBack<string> _okCallBack = null;
	private OkCallBack<string> _cancelCallBack = null;
	
	
	// Use this for initialization
	void Awake () 
	{
		message = transform.Find ("message").gameObject.GetComponent<tk2dTextMesh>();
		title = transform.Find ("title/Text").gameObject.GetComponent<tk2dTextMesh>();
		okLabel = transform.Find ("okBtn/ButtonGraphic/Text").gameObject.GetComponent<tk2dTextMesh>();
		bg = transform.Find ("bg").gameObject.GetComponent<tk2dSlicedSprite>();
		
		if(Application.systemLanguage == SystemLanguage.English)
		{
			title.text = "Hi"; 
			message.text = "To develop this game, developers workhard day and night, would you like to take 10 seconds to support the developers, make more friends to join us?";
		}
	}
	
	void Start () 
	{
		//		float messageWidth = message.GetComponent<tk2dTextMesh> ().wordWrapWidth;
		float messageWidth = message.GetComponent<Renderer>().bounds.size.x;
		float posX = - messageWidth / 2;
		var messagePos = message.transform.localPosition;
		messagePos.x = posX;
		message.transform.localPosition = messagePos;

		gameObject.transform.localScale = Vector3.zero;
		var seq = DOTween.Sequence ();
		var move = gameObject.transform.DOScale (1.0f, 0.2f).SetEase(Ease.OutQuad);
		seq.AppendInterval (1.0f).Append (move);
	}
	
	public void setOkCallBack(OkCallBack<string> callBack)
	{
		_okCallBack = callBack;
	}
	
	public void setCancelCallBack(OkCallBack<string> callBack)
	{
		_cancelCallBack = callBack;
	}
	
	void okCallBack()
	{
		if (_okCallBack != null) 
		{
			_okCallBack ("okCallBack");
		}
		Destroy(gameObject);
		GA.Event ("btn_rate_ok", null, 0);
	}
	
	void cancelkCallBack()
	{
		if (_cancelCallBack != null) 
		{
			_cancelCallBack ("cancelCallBack");
		}
		Destroy(gameObject);
		GA.Event ("btn_rate_cancel", null, 0);
	}
}
