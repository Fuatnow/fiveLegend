using UnityEngine;
using System.Collections;
using DG.Tweening;
using Umeng;
public class PauseLayer : MonoBehaviour {

	public bool isMoving = false;
	public GameObject rankLayer = null;
	// Use this for initialization
	void Start () 
	{

	}


	public void downFromeTop()
	{
		var gameCamera = GameObject.Find ("camera").GetComponent<GameCamera> ();
		float winWidth = gameCamera.winWidth;
		float winHeight = gameCamera.winHeight;
		
		var swallowBg = gameObject.transform.Find ("swallowBg").gameObject;
		swallowBg.SetActive (true);
		var sColor = swallowBg.GetComponent<tk2dSlicedSprite> ().color;
		sColor.a = 0.6f;
		swallowBg.GetComponent<tk2dSlicedSprite> ().color = sColor;
		swallowBg.GetComponent<tk2dSlicedSprite> ().dimensions = new Vector2 (winWidth,winHeight);
		
		var swallowBtn = swallowBg.transform.Find ("swallowBtn").gameObject;
		var swallowBounds = swallowBtn.GetComponent<tk2dSprite> ().GetBounds();
		float swallowBtnWidth = swallowBounds.size.x * 100;
		float swallowBtnHeight = swallowBounds.size.y * 100;
		swallowBtn.GetComponent<tk2dSprite>().scale = new Vector3 (winWidth / swallowBtnWidth, winHeight / swallowBtnHeight,1);
		swallowBtn.transform.FindChild("collider").gameObject.GetComponent<BoxCollider2D>().size = new Vector2 (winWidth / 100, winHeight / 100);
		
		bool canPlay = Singleton.getInstance("audioManager").GetComponent<AudioManager>().CanPlayAudio;
		var audioBtn_on = gameObject.transform.Find ("pauseBg/audioBtn_on").gameObject;
		var audioBtn_off =  gameObject.transform.Find ("pauseBg/audioBtn_off").gameObject;
		audioBtn_on.SetActive (canPlay);
		audioBtn_off.SetActive (!canPlay);
		
		var pauseBg = gameObject.transform.Find ("pauseBg").gameObject;
		var pBgDeimensions = pauseBg.GetComponent<tk2dSlicedSprite> ().dimensions;
		pBgDeimensions.x = winWidth;
		pauseBg.GetComponent<tk2dSlicedSprite> ().dimensions = pBgDeimensions;
		var pBgHeight = pBgDeimensions.y;
		var pBgPos = pauseBg.transform.position;
		float pY = -winHeight/200;
		pBgPos.y = pY - pBgHeight/100;
		pauseBg.transform.position = pBgPos;

		isMoving = true;
		float moveTime = 1.0f;
		pauseBg.SetActive (false);
		var moveTo = pauseBg.transform.DOLocalMoveY (pY, 0.8f).SetEase (Ease.OutExpo);
		var seq = DOTween.Sequence ();
		seq.AppendInterval (moveTime).Append(moveTo).AppendCallback(()=>{
			isMoving = false;
		});
		
		
		var selfPos = gameObject.transform.position;
		selfPos.y = (winHeight+pBgHeight)/100;
		gameObject.transform.position = selfPos;
		
		var gMoveTo = gameObject.transform.DOMoveY (0, moveTime).SetEase (Ease.OutExpo);
		var gSeq = DOTween.Sequence ();
		gSeq.Append (gMoveTo).AppendCallback (() => {
			pauseBg.SetActive(true);
		});
	}

	void rank_callBack()
	{
		Debug.Log ("rank_callBack");
		Singleton.getInstance("audioManager").GetComponent<AudioManager>().playAudio ("tap");
		if(rankLayer)
		{
			rankLayer.SetActive(true);
			rankLayer.GetComponent<RankLayer>().updateView();
		}
		GA.Event ("btn_rank", null, 0);
	}

	void audio_callBack()
	{
		Debug.Log ("audio_callBack");
		bool canPlay = Singleton.getInstance("audioManager").GetComponent<AudioManager>().CanPlayAudio;
		var audioBtn_on = gameObject.transform.Find ("pauseBg/audioBtn_on").gameObject;
		var audioBtn_off =  gameObject.transform.Find ("pauseBg/audioBtn_off").gameObject;
		audioBtn_on.SetActive (!canPlay);
		audioBtn_off.SetActive (canPlay);
		Singleton.getInstance("audioManager").GetComponent<AudioManager>().CanPlayAudio = !canPlay;
		Singleton.getInstance("audioManager").GetComponent<AudioManager>().playAudio ("tap");
		GA.Event ("btn_sound", null, 0);
	}

	void rate_callBack()
	{
		Debug.Log ("rate_callBack");
		Platform.getInstance().doRate();
		Singleton.getInstance("audioManager").GetComponent<AudioManager>().playAudio ("tap");
		GA.Event ("btn_rate", null, 0);
	}

	void share_callBack()
	{
		Debug.Log ("share_callBack");
		Singleton.getInstance("audioManager").GetComponent<AudioManager>().playAudio ("tap");
	}

	void restart_callBack()
	{
		Debug.Log ("restart_callBack");
		AdManager.getInstance ().showBanner ();
		PlayerPrefs.SetInt ("needResume", 0);
		Application.LoadLevel (0);


		//如果超过四盘且当前分数大于1000
		var playTimes = GameData.getInstance ().PlayTimes;
		if (playTimes > 1 && playTimes %5 == 0 ) 
		{
			GameData.getInstance ().CanLoadFullAd = true;
		}
	}



	void resume_callBack()
	{
		Debug.Log ("resume_callBack");
		AdManager.getInstance ().showBanner ();
		gameObject.SetActive (false);
		var gameBoard = GameObject.FindGameObjectWithTag (Constant.gameBoard);
		gameBoard.GetComponent<GameBoard> ().isPuaseGame = false;
	}
}
