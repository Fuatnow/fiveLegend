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

	public GameObject[] grayStarArray ;
	public GameObject[] ligthStarArray ;
	public tk2dSprite evaluateSp = null;
	
	
	// Use this for initialization
	void Awake () 
	{
		message = transform.Find ("bg/message").gameObject.GetComponent<tk2dTextMesh>();
		title = transform.Find ("bg/title/Text").gameObject.GetComponent<tk2dTextMesh>();
		okLabel = transform.Find ("bg/okBtn/ButtonGraphic/Text").gameObject.GetComponent<tk2dTextMesh>();
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

		bg.transform.localScale = Vector3.zero;
		var seq = DOTween.Sequence ();
		var move = bg.transform.DOScale (1.0f, 0.2f).SetEase(Ease.OutQuad);
		seq.AppendInterval (0.3f).Append (move);

		int i = 0;
		foreach (var gStar in grayStarArray) 
		{
			var tk2dItem = gStar.gameObject.GetComponent<tk2dUIItem>();
			tk2dItem.OnClickUIItem += grayStar_callBack;
			gStar.name = (i++).ToString();
		}


		i = 0;
		foreach (var lStar in ligthStarArray) 
		{
			var tk2dItem = lStar.gameObject.GetComponent<tk2dUIItem>();
			tk2dItem.OnClickUIItem += lightStar_callBack;
			lStar.name = (i++).ToString();
			if (i == 5) 
			{
				lStar.SetActive (false);
			}
		}
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
		if (_okCallBack != null && getRate() >= 4) 
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

	void grayStar_callBack(tk2dUIItem item)
	{
		Debug.Log ("grayStar_callBack");
		int lightNum = int.Parse (item.name);
		for(int i=0;i<5;i++)
		{
			ligthStarArray[i].SetActive (i <= lightNum || i==0);
		}
		updateEvaluateText (lightNum+1);
	}

	void lightStar_callBack(tk2dUIItem item)
	{
		Debug.Log ("lightStar_callBack");
		int lightNum = int.Parse (item.name);
		for(int i=0;i<5;i++)
		{
			ligthStarArray[i].SetActive (i < lightNum || i==0);
		}
		updateEvaluateText (Mathf.Max(0,lightNum-1)+1);
	}

	void updateEvaluateText(int starNum)
	{
		string str = string.Format ("choose_{0:00}", starNum);
		evaluateSp.SetSprite (str);
	}

	int getRate()
	{
		int num = 0;
		foreach(var item in ligthStarArray)
		{
			if (item.activeSelf) 
			{
				num++;
			}
		}
		return num;
	}
}
