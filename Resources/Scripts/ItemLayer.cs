using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Umeng;
public class ItemLayer : MonoBehaviour {
	private GameObject bestScoreLabel = null;
	private GameObject curScoreLabel = null;
	private GameObject diamondLabel = null;
	private GameObject maxGird = null;
	private GameObject gameBoard = null;
	private GameObject diamondSp = null;
	private GameObject preBtn = null;
	public GameObject pauseLayer = null;
	public GameObject helpLayer = null;
	public GameObject overLayer = null;

	private int curAddDiamondNum = 0;
	private int addDiamondBaseScore = 100;
	private int prop1UseTimes = 0;
	private int prop2UseTimes = 0;
	private tk2dTextMesh undo_diamondLabel = null;
	private tk2dTextMesh exchange_diamondLabel = null;

	// Use this for initialization
	void Start () 
	{
		gameBoard = GameObject.Find (Constant.gameBoard);
		bestScoreLabel = gameObject.transform.Find ("up/stage/bestScoreLabel").gameObject;
		curScoreLabel = gameObject.transform.Find ("up/stage/curScoreLabel").gameObject;
		diamondLabel = gameObject.transform.Find ("up/diamond/diamondLabel").gameObject;
		maxGird = gameObject.transform.Find ("maxGird").gameObject;
		preBtn = gameObject.transform.Find ("preBtn").gameObject;
		diamondSp = transform.Find ("up/diamond/Sprite").gameObject;
		undo_diamondLabel = transform.Find ("undo_diamond/diamondLabel").gameObject.GetComponent<tk2dTextMesh> ();
		exchange_diamondLabel = transform.Find ("exchange_diamond/diamondLabel").gameObject.GetComponent<tk2dTextMesh> ();
		curAddDiamondNum = GameData.getInstance ().CurScoreNum / addDiamondBaseScore;
		setPreBtnkActive (false);
		updateInfo ();
		updatePropDiamondLabel ();
	}

	
	public void setPreBtnkActive(bool isActive)
	{
		preBtn.GetComponent<tk2dButton> ().enabled = isActive;
		if(isActive)
		{
			preBtn.GetComponent<tk2dSprite> ().color = Color.white;
		}
		else
		{
			preBtn.GetComponent<tk2dSprite> ().color = Color.gray;
		}
		//preBtn.SetActive (isActive);
	}

	void preStep_callBack()
	{
		Debug.Log ("preStep_callBack");
		var gameBoard = GameObject.FindGameObjectWithTag (Constant.gameBoard).GetComponent<GameBoard>();
		bool canResumePreStep = gameBoard.canResumePreStep ();
		if(canResumePreStep)
		{
			int needDiamondNum =  getNeedDiamondNum(1);
			int curDiamondNum = GameData.getInstance().DiamondNum;
			int remainDiamonNum =  curDiamondNum-needDiamondNum;
			if(remainDiamonNum >= 0)
			{
				GameData.getInstance().DiamondNum = remainDiamonNum;
				updateDiamondLabel();
				gameBoard.toPreStep ();
				addPropTimes(1);
				setPreBtnkActive(false);
				gameBoard.storeCurBoardInfo();
			}
			else
			{
				showDiamondNotEnough();
			}
		}
		Singleton.getInstance("audioManager").GetComponent<AudioManager>().playAudio ("tap");
		GA.Event ("btn_undo", null, 0);
	}

	void exchange_callBack()
	{
		Debug.Log ("exchange_callBack");
		int needDiamondNum = getNeedDiamondNum(2);
		int curDiamondNum = GameData.getInstance().DiamondNum;
		int remainDiamonNum =  curDiamondNum-needDiamondNum;
		if(remainDiamonNum >= 0)
		{
			//gameBoard.GetComponent<GameBoard> ().isPuaseGame = true;
			gameBoard.GetComponent<GameBoard> ().isExchangingBlock = true;
			var unexchangeBtn = transform.Find ("unexchangeBtn").gameObject;
			var exchangeBtn = transform.Find ("exchangeBtn").gameObject;
			if(unexchangeBtn)
			{
				unexchangeBtn.SetActive(true);
				exchangeBtn.SetActive(false);
			}
			//GameData.getInstance().DiamondNum = remainDiamonNum;
			//updateDiamondLabel();
		}
		else
		{
			showDiamondNotEnough();
		}
		Singleton.getInstance("audioManager").GetComponent<AudioManager>().playAudio ("tap");
		GA.Event ("btn_exchange", null, 0);
	}

	void unexchange_callBack()
	{
		Debug.Log ("exchange_callBack");
		if (gameBoard.GetComponent<GameBoard> ().isExchangingBlock == false) return;
		gameBoard.GetComponent<GameBoard> ().resumeExchangeStatus(); 
		resumeExchangeStatus ();
		GA.Event ("btn_unexchange", null, 0);
	}

	public void subDiamondNum(int propId)
	{
		int needDiamondNum = getNeedDiamondNum(propId);
		int curDiamondNum = GameData.getInstance().DiamondNum;
		int remainDiamonNum =  curDiamondNum-needDiamondNum;
		GameData.getInstance().DiamondNum = remainDiamonNum;
	}

	public void resumeExchangeStatus()
	{
		var unexchangeBtn = transform.Find ("unexchangeBtn").gameObject;
		var exchangeBtn = transform.Find ("exchangeBtn").gameObject;
		if(unexchangeBtn)
		{
			unexchangeBtn.SetActive(false);
			exchangeBtn.SetActive(true);
		}
	}

	public void updatePropDiamondLabel()
	{
		updateUndoDiamondLabel ();
		updateExchangeDiamondLabel ();
	}

	public void updateUndoDiamondLabel()
	{
		int prop1DiamondNum = getNeedDiamondNum (1);
		var prop1DiamondStr = getNumStr (prop1DiamondNum);
		undo_diamondLabel.text = prop1DiamondStr;
	}

	public void updateExchangeDiamondLabel()
	{
		int prop2DiamondNum = getNeedDiamondNum (2);
		var prop2DiamondStr = getNumStr (prop2DiamondNum);
		exchange_diamondLabel.text = prop2DiamondStr;
	}


	string getNumStr(int propDiamondNum)
	{
		var str = "" + propDiamondNum;
		if(propDiamondNum >= 1000000)
		{
			float k = propDiamondNum/1000000.0f;
			str = k.ToString("#.0")+"m";
		}
		else if(propDiamondNum >= 1000)
		{
			float k = propDiamondNum/1000.0f;
			str = k.ToString("#.0")+"k";
		}
		return str;
	}


	//add use times
	public void addPropTimes(int propId)
	{
		if(propId == 1)
		{
			prop1UseTimes+=1;
			//PlayerPrefs.SetInt("undo_UseTimes",prop1UseTimes);
			updateUndoDiamondLabel();
		}
		else if(propId == 2)
		{
			prop2UseTimes+=1;
			//PlayerPrefs.SetInt("exchange_UseTimes",prop2UseTimes);
			updateExchangeDiamondLabel();
		}
	}

	public void setPropUseTimes(int propId , int useTimes)
	{
		switch(propId)
		{
		case 1:
			prop1UseTimes = useTimes;
			break;
		case 2:
			prop2UseTimes = useTimes;
			break;
		default:
			break;
		}
	}

	
	public int getPropUseTimes(int propId)
	{
		int useTimes = 0;
		switch(propId)
		{
		case 1:
			useTimes = prop1UseTimes;
			break;
		case 2:
			useTimes = prop2UseTimes;
			break;
		default:
			break;
		}
		return useTimes;
	}

	int getNeedDiamondNum(int propId)
	{
		int useTimes = getPropUseTimes (propId);
		int baseUseDiamond = GameData.getInstance ().getBaseDiamondNum (propId);
		return (int)(baseUseDiamond* Mathf.Pow(2,useTimes));
	}
	
	void pause_callBack()
	{
		Debug.Log ("pause_callBack");
		AdManager.getInstance ().hideBanner ();
		submitScore ();
		var audioManager = Singleton.getInstance ("audioManager").GetComponent<AudioManager> ();
		audioManager.playAudio("pause");	
		pauseLayer.SetActive (true);
		var layer = pauseLayer.GetComponent<PauseLayer> ();
		if(layer.isMoving == false)
		{
			layer.downFromeTop();
		}
		gameBoard.GetComponent<GameBoard> ().isPuaseGame = true;
		GA.Event ("btn_pause", null, 0);
	}

	void help_callBack()
	{
		Debug.Log ("help_callBack");
		Singleton.getInstance("audioManager").GetComponent<AudioManager>().playAudio ("tap");
		helpLayer.SetActive (true);
		var layer = helpLayer.GetComponent<HelpLayer> ();
		layer.resetChildPos ();
		gameBoard.GetComponent<GameBoard> ().isPuaseGame = true;
		GA.Event ("btn_help", null, 0);
	}

	public void updateInfo()
	{
		updateBestScoreLabel ();
		updateCurScoreLabel ();
		updateDiamondLabel ();
		updateMaxGird ();
	}

	void updateBestScoreLabel()
	{
		if(bestScoreLabel != null)
		{
			int bestNum = GameData.getInstance ().BestScoreNum;
			bestScoreLabel.GetComponent<tk2dTextMesh> ().text = bestNum + "";
		}
	}

	void updateCurScoreLabel()
	{
		if(curScoreLabel != null)
		{
			int curNum = GameData.getInstance ().CurScoreNum;
			curScoreLabel.GetComponent<tk2dTextMesh> ().text = curNum + "";
		}

	}

	void updateMaxGird()
	{
		if(maxGird != null)
		{
			int maxGirdNum = GameData.getInstance ().BestGirdNum;
			maxGird.GetComponent<tk2dSprite> ().spriteId = gameBoard.GetComponent<GameBoard> ().getSprite (maxGirdNum);
		}
	}

	void updateDiamondLabel()
	{
		if(diamondLabel != null)
		{
			int diamondNum = GameData.getInstance ().DiamondNum;
			diamondLabel.GetComponent<tk2dTextMesh>().text = ""+diamondNum;
		}
	}

	public void addDiamondByScore()
	{
		int curNum = GameData.getInstance ().CurScoreNum;
		int aimAddDiamondNum = curNum / addDiamondBaseScore;
		if(curAddDiamondNum != aimAddDiamondNum)
		{
			int needAddNum = aimAddDiamondNum - curAddDiamondNum;
			curAddDiamondNum = aimAddDiamondNum;
			updateDiamondLabel();
			var pos = diamondSp.transform.position;
			pos.z += -0.00001f;
			var scale = Random.Range(2.0f,2.5f);
			Quaternion rotation = Quaternion.identity;
			//rotation.eulerAngles = new Vector3(0,0,rotate);
			var diamondCopy = Instantiate(diamondSp,pos,rotation) as GameObject;
			diamondCopy.transform.parent = gameObject.transform;
			var scaleTo1 =diamondCopy.transform.DOScale(scale,0.05f);
			var scaleTo2 =diamondCopy.transform.DOScale(1.0f,0.1f);
			var seq = DOTween.Sequence();
			seq.Append(scaleTo1).Append(scaleTo2).AppendCallback(()=>{
				GameData.getInstance().addDiamond(needAddNum);
				Destroy(diamondCopy);
			});
		}
	}


	public void gameOver()
	{
		Debug.Log ("gameOver");
		PlayerPrefs.SetInt ("needResume", 0);
		GA.EventEnd ("game_play");
		submitScore ();
		var audioManager = Singleton.getInstance ("audioManager").GetComponent<AudioManager> ();
		audioManager.playAudio("gameover");	
		overLayer.SetActive (true);
		var overCom = overLayer.GetComponent<OverLayer> ();
		if(overCom.isMoving == false)
		{
			overCom.downFromeTop();
		}

		bool isNeedShowRateDialog = false;
		if(GameData.getInstance().PlayTimes >= 5 && GameData.getInstance().BestScoreNum > 12000)
		{
			if(GameData.getInstance().PlayTimes % 5 == 0)
			{
				var tipDialog = Instantiate(Resources.Load ("Prefab/rateDialog")) as GameObject;
				tipDialog.transform.parent = transform;
				tipDialog.GetComponent<RateDialog> ().setOkCallBack (new RateDialog.OkCallBack<string>( (data)=>{
					Debug.Log("return date: "+data);
					Platform.getInstance().doRate();
					isNeedShowRateDialog = true;
				}));
			}
		}

		//如果超过四盘且当前分数大于1000
		if (isNeedShowRateDialog == false && GameData.getInstance ().PlayTimes > 4 && GameData.getInstance().CurScoreNum >  1000 ) 
		{
			GameData.getInstance ().CanLoadFullAd = false;
			AdManager.getInstance ().showInterstitial ();
		}
	}

	void submitScore()
	{
		int curScore = GameData.getInstance ().CurScoreNum;
		int bestScore = GameData.getInstance ().BestScoreNum;
		int maxGirdNum = GameData.getInstance ().BestGirdNum;
		BombTool.instance.submitScore (BombTool.instance.ObjectId, bestScore,maxGirdNum,null);
	}

	void showDiamondNotEnough()
	{
		gameBoard.GetComponent<GameBoard> ().isPuaseGame = true;
		var tipDialog = Instantiate(Resources.Load ("Prefab/tipDialog")) as GameObject;
		tipDialog.transform.parent = transform;
		tipDialog.GetComponent<TipDialog> ().setOkCallBack (new TipDialog.OkCallBack<string>( (data)=>{
			Debug.Log("return date: "+data);
			gameBoard.GetComponent<GameBoard> ().isPuaseGame = false;
		}));
	}
}
