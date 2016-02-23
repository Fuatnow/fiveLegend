using UnityEngine;
using System.Collections;

public class TipDialog : MonoBehaviour 
{

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
			title.text = "Tip"; 
			message.text = "Diamond is unenough, every 100 points can get one diamond.The longer you play the faster you get.";
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
	}

	void cancelkCallBack()
	{
		if (_cancelCallBack != null) 
		{
			_cancelCallBack ("cancelCallBack");
		}
		Destroy(gameObject);
	}
}
