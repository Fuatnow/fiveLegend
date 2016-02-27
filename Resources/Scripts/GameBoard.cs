using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Umeng;
public class GameBoard : MonoBehaviour
{
	public GameObject[] girdBlocks;
	public GameObject[] spriteBlocks;
	private GameObject [,] girdArray = new GameObject[10,5];
	private int [,] preGirdInfo = new int[10,5];//record pre girdArray info
	private int preScoreNum = 0;
	private int preMaxGirdNum = 0;//pre best maxgird num the left-bottom gird of the bgBoard
	private int preCreateNum = 0;//pre curGird create Num
	private int preNextGirdNum = 0;// pre nextGird create Num
	private GameObject curMaxGird = null;
	private float downSpeed = 2.0f; 
	private float downDelTime = 0.0f;
	private GameObject nextGird = null;
	private GameObject curGird = null;
	private static float backgroundPosY = -0.3f;
	private Vector3 nextGirdPos = new Vector3(2.7f,backgroundPosY+4.05f,0);
	private Vector3 curScorePos = new Vector3(2.75f,7.0f,-0.1f);
	private float girdWidth = 0.9f;
	private float girdHeight = 0.9f;
	private bool canDropDown = false;
	private Vector3 touchPos;
	private bool isCurGirdMoving = false;
	private bool isGameOver = false;
	private int[,] searchDir = new int[,]{{-1,0},{0,-1}};
	private int[] remainderNum = new int[]{1,2,3,5};
	private Dictionary<int, int> girdNumMap=new Dictionary<int,int>();
	public bool isPuaseGame = false;
	public bool isExchangingBlock = false;
	private ItemLayer itemLayer = null;
	private GameObject box_sel_first = null;
	private GameObject box_sel_second = null;
	private GameObject block_exchange_first = null;
	private GameObject block_exchange_second = null;
	private GameObject block_exchange_cur_copy = null;
	private float exchange_addZoder = 0.0001f;
	public bool CanDropDown
	{
		get 
		{ 
			return canDropDown;
		}
		set 
		{ 
			canDropDown = value;
		}
	}

	void Awake()
	{
		box_sel_first = transform.Find ("box_sel_first").gameObject;
		box_sel_second = transform.Find ("box_sel_second").gameObject;
		itemLayer = GameObject.Find("itemLayer").GetComponent<ItemLayer>();

		for(int i=0;i<Constant.ROW;i++)
		{
			for(int j=0;j<Constant.COL;j++)
			{
				girdArray[i,j] = null;
				preGirdInfo[i,j] = 0;
			}
		}

		int index = 0;
		girdNumMap [0] = 0;
		girdNumMap [1] = index++;
		girdNumMap [2] = index++;
		girdNumMap [3] = index++;
		girdNumMap [5] = index++;

		girdNumMap [10] = index++;
		girdNumMap [20] = index++;
		girdNumMap [30] = index++;
		girdNumMap [50] = index++;

		girdNumMap [100] = index++;
		girdNumMap [200] = index++;
		girdNumMap [300] = index++;
		girdNumMap [500] = index++;

		girdNumMap [1000] = index++;
		girdNumMap [2000] = index++;
		girdNumMap [3000] = index++;
		girdNumMap [5000] = index++;

		girdNumMap [10000] = index++;
		girdNumMap [20000] = index++;
		girdNumMap [30000] = index++;
		girdNumMap [50000] = index++;

		girdNumMap [100000] = index++;
		girdNumMap [200000] = index++;
		girdNumMap [300000] = index++;
		girdNumMap [500000] = index++;

		girdNumMap [1000000] = index++;
		//search two directions first->bottom(-1,0) second->left(0,-1) 

		GameData.getInstance ().CurScoreNum = 0;
		int needResume = PlayerPrefs.GetInt ("needResume",0);
		if(needResume > 0)
		{
			copyToPreBoardInfo();
			toPreStep();
		}
		else
		{		
			createNextGird ();
		}

		thirdInit ();
	}

	void Start ()
	{
		thirdInit ();
	}

	void thirdInit()
	{

		GA.EventEnd ("game_play");
		GA.EventBegin("game_play");
		
		string objectId = BombTool.instance.ObjectId;
		if(objectId != "")
		{
			GA.ProfileSignOff();
			GA.ProfileSignIn(objectId);
		}

		GameData.getInstance ().PlayTimes++;
		BombTool.instance.submitAttribute ();
		AdManager.getInstance ();
		if (GameData.getInstance ().CanLoadFullAd == true) 
		{
			AdManager.getInstance ().showInterstitial ();
			GameData.getInstance ().CanLoadFullAd = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		Platform.getInstance ().keyBack ();

		if(isPuaseGame == true || isExchangingBlock == true || isGameOver == true) 
		{
			return;
		}

		if(curGird && CanDropDown && downDelTime > downSpeed)
		{
			int row = curGird.GetComponent<Gird> ().ROW;
			int col = curGird.GetComponent<Gird> ().COL;
			CanDropDown = canDropCurGird(row,col);
			if(CanDropDown)
			{
				dropCurGird();
			}

			if(CanDropDown == false)
			{	
				if(col < Constant.COL)
				{
					girdArray[row,col] = curGird;
				}
				updatePreBoardInfo();
				addScore(curGird.transform.position,curGird.GetComponent<Gird>().Num);
				mergeGirds();
			}
			downDelTime = 0;
		}
		if(CanDropDown)
		{
			downDelTime += Time.deltaTime;
		}
	}
	

	
	void OnMouseDown()
	{        
		touchPos = Input.mousePosition;
		if(isExchangingBlock)
		{
			selExchangeBlock(touchPos);
		}
	}
	
	void OnMouseUp() 
	{
		if(isCurGirdMoving == false && isPuaseGame == false && isExchangingBlock == false && isGameOver == false)
		{
			moveCurGird(touchPos,Input.mousePosition);
		}
	}

	void selExchangeBlock(Vector3 position)
	{
		var pos = Camera.main.ScreenToWorldPoint (position);
		pos.z = 0;
		//Debug.Log ("position: " + position + "  pos: " + pos);
		for(int i=0;i<Constant.ROW;i++)
		{
			for(int j=0;j<Constant.COL;j++)
			{
				var blockObj = girdArray[i,j];
				if(blockObj)
				{
					var block = blockObj.GetComponent<tk2dSprite>();
					var bound = block.GetBounds();
					bound.center = blockObj.transform.position;

					if(bound.Contains(pos))
					{
						//Debug.Log("bound:"+bound.ToString());
						var blockPos = block.transform.position;
						//change block zOreder if not when change them maybe hide bottom othes
						Debug.Log("Row: "+i+" Col:"+j);
						if(box_sel_first.activeSelf == false)
						{
							blockPos.z = blockPos.z - exchange_addZoder;
							box_sel_first.SetActive(true);
							box_sel_first.transform.position = blockPos;
							block_exchange_first = blockObj;
							addZorder(ref block_exchange_first , true);
						}
						else
						{
							blockPos.z = blockPos.z - exchange_addZoder;
							box_sel_second.SetActive(true);
							box_sel_second.transform.position = blockPos;
							block_exchange_second = blockObj;
							addZorder(ref block_exchange_second , true);
							StartCoroutine(exchangeBlocks());
						}
						return;
					}
				}
			}
		}
	}

	IEnumerator exchangeBlocks()
	{
		updatePreBoardInfo();
		yield return new WaitForSeconds(0.5f);
		block_exchange_cur_copy = curGird;
		var sel1Pos = block_exchange_first.transform.position;
		var sel2Pos = block_exchange_second.transform.position;
		box_sel_first.SetActive (false);
		box_sel_second.SetActive (false);

		//change girdArray
		var gird1 = block_exchange_first.GetComponent<Gird>();
		var gird2 = block_exchange_second.GetComponent<Gird>();

		gird1.swap_rowAndcol (ref gird2);

		girdArray[gird1.ROW,gird1.COL] = block_exchange_first;
		girdArray[gird2.ROW,gird2.COL] = block_exchange_second;



		var dis = Vector3.Distance (sel1Pos, sel2Pos);
		var moveTime = dis / 10;
		var seq1 = DOTween.Sequence ();
		var move1 = block_exchange_first.transform.DOMove (sel2Pos, moveTime);
		var callBack1 = new TweenCallback(()=>{
			addZorder(ref block_exchange_first,false);
			StartCoroutine(exchange_callBack());
		});
		seq1.Append (move1).AppendCallback (callBack1);

		var seq2 = DOTween.Sequence ();
		var move2 = block_exchange_second.transform.DOMove (sel1Pos, moveTime);
		var callBack2 = new TweenCallback(()=>{
			addZorder(ref block_exchange_second,false);
		});
		seq2.Append (move2).AppendCallback (callBack2);

		//var itemLayer = GameObject.Find("itemLayer").GetComponent<ItemLayer>();
	}

	IEnumerator exchange_callBack()
	{

		//var itemLayer = GameObject.Find("itemLayer").GetComponent<ItemLayer>();
		itemLayer.subDiamondNum (2);
		itemLayer.updateInfo();
		itemLayer.addPropTimes (2);
		storeCurBoardInfo();
		yield return new WaitForSeconds(0.05f);
		if(isExchangingBlock)
		{
			curGird = block_exchange_first;
			block_exchange_first=null;
			mergeGirds();
		}
	}

	public void resumeExchangeStatus()
	{
		if(box_sel_first)
		{
			box_sel_first.SetActive(false);
		}

		if(box_sel_second)
		{
			box_sel_second.SetActive(false);
		}

		addZorder(ref block_exchange_first,false);
		addZorder(ref block_exchange_second,false);
		block_exchange_first = null;
		block_exchange_second = null;
		isExchangingBlock = false;
		block_exchange_cur_copy = null;
	}

	void addZorder(ref GameObject block , bool isUp)
	{
		if (block == null)
			return;
		int toUp = isUp ? -1 : 1;
		var blockPos = block.transform.position;
		blockPos.z = blockPos.z + exchange_addZoder * toUp;
		block.transform.position = blockPos;
	}

	void createNextGird()
	{
		int girdIndex = Random.Range (0, 5);
		createNextGird (girdIndex);
	}

	void createNextGird(int girdIndex)
	{
		createNextGird (girdIndex, -1);
	}

	void createNextGird(int girdIndex,int nextGirdIndex)
	{
		//when need crateNextGird set diwnDekTime to be 0
		downDelTime = 0;
		int toIndex = getMoveToIndex();
		if(toIndex==Constant.COL)
		{
//			if(testCanMerge(Constant.ROW-1,Constant.COL-1))
//			{
//				mergeGirds();
//			}
//			else
//			{
//				gameOver();
//			}
//			return;
		}
		
		int num = getGirdNum(girdIndex);
		if(nextGird)
		{
			curGird = nextGird;
			int nextGirdNum = nextGird.GetComponent<Gird>().Num;
			curGird.GetComponent<Gird>().setProperty(nextGirdNum,Constant.ROW-1,toIndex);
			
			var spriteRender = curGird.GetComponent<tk2dSprite>();
			spriteRender.spriteId = nextGird.GetComponent<tk2dSprite>().spriteId;
			
			float toX = (toIndex - 2) * girdWidth;
			float moveTime = 0.05f * (5-toIndex);
			isCurGirdMoving = true;

			var curGirdPos = curGird.transform.position;
			curGirdPos.z = 0;
			curGird.transform.position = curGirdPos;
			var moveTo1 = curGird.transform.DOMoveX (toX-0.05f, moveTime).SetEase (Ease.OutQuad);
			var moveTo2 = curGird.transform.DOMoveX(toX,0.05f).SetEase(Ease.InQuad);
			var seq = DOTween.Sequence();
			seq.Append(moveTo1).Append(moveTo2).AppendInterval(0.1f).AppendCallback(()=>{
				CanDropDown = true;
				isCurGirdMoving = false;
				testGameOver();
			});
			nextGirdPos.z = exchange_addZoder;
			nextGird = GameObject.Instantiate (girdBlocks[girdIndex], nextGirdPos, Quaternion.identity) as GameObject;
			nextGird.GetComponent<Gird>().Num = num;
			//nextGird = Resources.Load("Prefab/block0.prefab") as GameObject;
			var scale  =  new Vector3(0.2f,0.2f,1);
			nextGird.transform.localScale = scale;
			nextGird.transform.DOScale(new Vector3(1,1,1),0.5f).SetDelay(0.1f);
			nextGird.transform.parent = gameObject.transform;
			var audioManager = Singleton.getInstance ("audioManager").GetComponent<AudioManager> ();
			audioManager.playAudio("sou");
		}
		else
		{
			nextGirdPos.z = exchange_addZoder;
			nextGird = GameObject.Instantiate (girdBlocks[girdIndex], nextGirdPos, Quaternion.identity) as GameObject;
			//nextGird = Resources.Load("Prefab/block0") as GameObject;
			nextGird.GetComponent<Gird>().Num = num;
			nextGird.transform.parent = gameObject.transform;
			if(nextGirdIndex < 0)
			{
				createNextGird();
			}
			else
			{
				createNextGird(nextGirdIndex,-1);
			}
		}
		
		storeCurBoardInfo();
	}


	int getMoveToIndex()
	{
		int i = Constant.COL - 1;
		while(i>=2)
		{
			GameObject obj = girdArray[Constant.ROW-1,i];
			if(obj)
			{
				break;
			}
			i--;
		}
		return i+1;
	}


	int getGirdNum(int createIndex)
	{
		int exp = createIndex/4;
		int remainder = createIndex % 4;
		int num = (int)(System.Math.Pow (10, exp) * remainderNum[remainder]);
		return num;
	}

	bool canDropCurGird(int row , int col)
	{
		if(row-1 < 0 || col+1 > Constant.COL)	return false;
		if(girdArray[row-1,col])return false;
		return true;
	}

	void dropCurGird()
	{
		int row = curGird.GetComponent<Gird> ().ROW;
		//int col = curGird.GetComponent<Gird> ().COL;
		var pos = curGird.transform.position;
		pos.y -= girdHeight;
		curGird.transform.position = pos;
		curGird.GetComponent<Gird> ().ROW = row - 1;
	}

	void moveCurGird(Vector3 benganPos , Vector3 endPos)
	{
		float winWidth = Screen.width;
		float winHeight = Screen.height;
		Vector3 disPos = endPos - benganPos;
		float distance = Vector3.Distance (Vector3.zero, disPos);		
		if(distance < 0.01f * winWidth)
		{
			if(benganPos.x < winWidth/2)
			{
				moveToLeft();
			}
			else
			{
				moveToRight();
			}
		}
		else
		{
//			float degree = Mathf.Atan2(endPos.y-benganPos.y,endPos.x-benganPos.x) * Mathf.Rad2Deg;
//			degree = ((int)(degree + 360)) % 180;
//			Debug.Log ("degree= " + degree);
			var norPos = disPos.normalized;
			float k = norPos.y / norPos.x;
			if(disPos.x < 0) 
			{
				if(k > -1 && k < 1 && Mathf.Abs(disPos.x) > 0.1f*winWidth) moveToLeft(true);
				else if(k > 1 && Mathf.Abs(disPos.y) > 0.03f*winHeight) moveToButtom();
			}
			else
			{
				if(k > -1 && k < 1 && Mathf.Abs(disPos.x) > 0.1f*winWidth)moveToRight(true);
				else if(k < -1 && Mathf.Abs(disPos.y) > 0.03f*winHeight) moveToButtom();
			}
		}
	}

	void moveToLeft(bool toEdge = false)
	{
		Debug.Log ("moveToLeft:" + toEdge);
		int row = curGird.GetComponent<Gird> ().ROW;
		int col = curGird.GetComponent<Gird> ().COL;
		int toCol = col;
		
		var audioManager = Singleton.getInstance ("audioManager").GetComponent<AudioManager> ();
		if(toEdge == false)
		{
			int tempCol = System.Math.Max (col-1,0);
			if(girdArray[row,tempCol] == null)
			{
				toCol = tempCol;
			}
			audioManager.playAudio("move");
		}
		else
		{
			int i=col-1;
			for(;i>=0;i--)
			{
				if(girdArray[row,i])
				{
					toCol = i+1;
					break;
				}
			}
			if(i == -1) toCol = 0;
			audioManager.playAudio("sou");
		}

		//if(toCol != col)
		{
			float moveToX = curGird.transform.position.x + (toCol - col) * girdWidth;
			CanDropDown = false;
			isCurGirdMoving = true;
			float moveTime = System.Math.Abs(toCol - col)*0.02f;
			moveTime = 0;
			curGird.GetComponent<Gird>().COL = toCol;
			curGird.transform.DOMoveX(moveToX,moveTime).OnComplete(delegate(){
				CanDropDown = true;
				isCurGirdMoving = false;
			});
//			var curPos = curGird.transform.position;
//			curPos.x = moveToX;
//			curGird.transform.position = curPos;
//			curGird.GetComponent<Gird>().COL = toCol;
		}
	}

	void moveToRight(bool toEdge = false)
	{
		Debug.Log ("moveToRight:" + toEdge);
		var audioManager = Singleton.getInstance ("audioManager").GetComponent<AudioManager> ();
		int row = curGird.GetComponent<Gird> ().ROW;
		int col = curGird.GetComponent<Gird> ().COL;
		int toCol = col;
		if(toEdge == false)
		{
			int tempCol = System.Math.Min (col+1, Constant.COL-1);
			if(girdArray[row,tempCol] == null)
			{
				toCol = tempCol;
			}
			audioManager.playAudio("move");
		}
		else
		{
			int i = col+1;
			for(;i<Constant.COL;i++)
			{
				if(girdArray[row,i])
				{
					toCol = i-1;
					break;
				}
			}
			if(i == Constant.COL) toCol = Constant.COL-1;
			audioManager.playAudio("sou");
		}
		
		//if(toCol != col)
		{
			float moveToX = curGird.transform.position.x + (toCol - col) * girdWidth;
//			var curPos = curGird.transform.position ;
//			curPos.x = moveToX;
//			curGird.transform.position = curPos;
//			curGird.GetComponent<Gird>().COL = toCol;

			CanDropDown = false;
			isCurGirdMoving = true;
			curGird.GetComponent<Gird>().COL = toCol;
			float moveTime = System.Math.Abs(toCol - col)*0.02f;
			moveTime = 0;
			curGird.transform.DOMoveX(moveToX,moveTime).OnComplete(delegate(){
				CanDropDown = true;
				isCurGirdMoving = false;
			});
		}
	}

	void moveToButtom()
	{
		Debug.Log ("moveToButtom:");
		int row = curGird.GetComponent<Gird> ().ROW;
		int col = curGird.GetComponent<Gird> ().COL;
		int toRow = row;
		int i = row - 1;
		for(;i>=0;i--)
		{
			if(girdArray[i,col])
			{
				toRow = i+1;
				break;
			}
		}
		if(i==-1) toRow = 0;
		//if(toRow !=row)
		{
			float moveToY = curGird.transform.position.y + (toRow - row) * girdHeight;
//			var curPos = curGird.transform.position ;
//			curPos.y = moveToY;
//			curGird.transform.position = curPos;
//			curGird.GetComponent<Gird>().ROW = toRow;
//			girdArray[toRow,col] = curGird;
//			mergeGirds();

			CanDropDown = false;
			isCurGirdMoving = true;
			curGird.GetComponent<Gird>().ROW = toRow;
			girdArray[toRow,col] = curGird;
			float moveTime = System.Math.Abs(toRow - row)*0.005f;
			moveTime = 0;
			curGird.transform.DOMoveY(moveToY,moveTime).OnComplete(delegate(){
				isCurGirdMoving = false;
				updatePreBoardInfo();
				addScore(curGird.transform.position,curGird.GetComponent<Gird>().Num);
				mergeGirds();
			});
		}
		var audioManager = Singleton.getInstance ("audioManager").GetComponent<AudioManager> ();
		audioManager.playAudio("down");
	}

	void updatePreBoardInfo()
	{
		int curScoreNum = GameData.getInstance ().CurScoreNum;
		preScoreNum = curScoreNum;
		preCreateNum = curGird.GetComponent<Gird> ().Num;

		if(curMaxGird)
		{
			preMaxGirdNum = curMaxGird.GetComponent<Gird>().Num;
		}

		var girdInfo = "";
		for(int i=0;i<Constant.ROW;i++)
		{
			for(int j=0;j<Constant.COL;j++)
			{
				var gird = girdArray[i,j];
				int girdNum = 0;
				if(gird && gird != curGird)
				{
					girdNum = gird.GetComponent<Gird> ().Num;
				}
				preGirdInfo[i,j] = girdNum;
				girdInfo += girdNum+";";
			}
		}
	}

	public void storeCurBoardInfo()
	{
		var girdInfo = "";
		for(int i=0;i<Constant.ROW;i++)
		{
			for(int j=0;j<Constant.COL;j++)
			{
				var gird = girdArray[i,j];
				int girdNum = 0;
				if(gird && gird != curGird)
				{
					girdNum = gird.GetComponent<Gird> ().Num;
				}
				girdInfo += girdNum+";";
			}
		}

		int curMaxGirdNum = 0;
		if(curMaxGird)
		{
			curMaxGirdNum = curMaxGird.GetComponent<Gird>().Num;
		}

		//store curgird info
//		Debug.Log ("girdInfo:" + girdInfo);
		PlayerPrefs.SetString ("curGirdInfo", girdInfo);
		int curScoreNum = GameData.getInstance ().CurScoreNum;
		int curGirdNum = curGird.GetComponent<Gird> ().Num;
		int nextGirdNum = nextGird.GetComponent<Gird> ().Num;
		int prop1_useTimes = itemLayer.getPropUseTimes (1);
		int prop2_useTimes = itemLayer.getPropUseTimes (2);

		PlayerPrefs.SetInt ("needResume", 1);

//		PlayerPrefs.SetInt("curScoreNum",curScoreNum);
//		PlayerPrefs.SetInt("curCreateNum",curGirdNum);
//		PlayerPrefs.SetInt ("nextGirdNum", nextGirdNum);
//		PlayerPrefs.SetInt("curMaxGirdNum",curMaxGirdNum);
//		PlayerPrefs.SetInt ("prop1_useTimes", prop1_useTimes);
//		PlayerPrefs.SetInt ("prop2_useTimes", prop2_useTimes);
		var dataInfo = curScoreNum+";"+curGirdNum+";"+nextGirdNum+";"+curMaxGirdNum+";"+prop1_useTimes+";"+prop2_useTimes+";";
		PlayerPrefs.SetString ("dataInfo", dataInfo);
	}

	void copyToPreBoardInfo()
	{
//		preScoreNum = PlayerPrefs.GetInt ("curScoreNum", 0);
//		preCreateNum = PlayerPrefs.GetInt ("curCreateNum", 0);
//		preNextGirdNum = PlayerPrefs.GetInt ("nextGirdNum", 0);
//		preMaxGirdNum = PlayerPrefs.GetInt ("curMaxGirdNum", 0);
//		int prop1_useTimes = PlayerPrefs.GetInt ("prop1_useTimes", 0);
//		int prop2_useTimes = PlayerPrefs.GetInt ("prop2_useTimes", 0);
		char[] p = new char[1]{';'};
		string dataInfoStr = PlayerPrefs.GetString ("dataInfo", "");
		var dataVec = dataInfoStr.Split(p);
		if(dataVec.GetLength(0)>=6)
		{
			var numStr = dataVec[0];
			int dataNum = int.Parse(numStr);
			preScoreNum = dataNum;

			numStr = dataVec[1];
			dataNum = int.Parse(numStr);
			preCreateNum = dataNum;

			numStr = dataVec[2];
			dataNum = int.Parse(numStr);
			preNextGirdNum = dataNum;

			numStr = dataVec[3];
			dataNum = int.Parse(numStr);
			preMaxGirdNum = dataNum;

			numStr = dataVec[4];
			dataNum = int.Parse(numStr);
			int prop1_useTimes = dataNum;

			numStr = dataVec[5];
			dataNum = int.Parse(numStr);
			int prop2_useTimes = dataNum;

			itemLayer.setPropUseTimes (1, prop1_useTimes);
			itemLayer.setPropUseTimes (2, prop2_useTimes);
		}

		string preGirdInfoStr = PlayerPrefs.GetString ("curGirdInfo", "");
		//Debug.Log ("curGirdInfo:" + preGirdInfoStr);
		var stringVec = preGirdInfoStr.Split(p);
		if(stringVec.GetLength(0)>= Constant.ROW * Constant.COL)
		{
			for(int i=0;i<Constant.ROW;i++)
			{
				for(int j=0;j<Constant.COL;j++)
				{
					var girdNumStr = stringVec[i*Constant.COL+j];
					int girdNum = int.Parse(girdNumStr);
					preGirdInfo[i,j] = girdNum;
				}
			}
		}
	}

	Vector3 getPosByRowCol(int row , int col)
	{
		float posX = (col-2) * girdWidth;
		float posY = -4.5f*girdHeight + row * girdHeight+backgroundPosY;
		return new Vector3 (posX, posY, 0);
	}

	public int getSprite(int girdNum)
	{
		int spriteIndex = girdNumMap[girdNum];
		return spriteBlocks[spriteIndex].GetComponent<tk2dSprite>().spriteId;
	}

	public bool canResumePreStep()
	{
		return !(isCurGirdMoving || preCreateNum == 0 || isPuaseGame || isExchangingBlock);
	}

	public void toPreStep()
	{
//		if(isCurGirdMoving || preCreateNum == 0) return;
//		var itemLayer = GameObject.Find("itemLayer").GetComponent<ItemLayer>();
//		itemLayer.setPreBtnkActive (false);
		if(canResumePreStep() == false) return;
		
		for(int i=0;i<Constant.ROW;i++)
		{
			for(int j=0;j<Constant.COL;j++)
			{
				int girdNum = preGirdInfo[i,j];
				var gird = girdArray[i,j];
				if(girdNum == 0)
				{
					if(gird)
					{
						Destroy(gird);
						girdArray[i,j] = null;
					}
				}
				else
				{
					if(gird == null)
					{
						var pos = getPosByRowCol(i,j);
						gird = GameObject.Instantiate (girdBlocks[0], pos, Quaternion.identity) as GameObject;
						gird.GetComponent<Gird>().ROW = i;
						gird.GetComponent<Gird>().COL = j;
						gird.transform.parent = gameObject.transform;
						girdArray[i,j] = gird;
					}
					gird.GetComponent<Gird>().Num = girdNum;
					var spriteRender = gird.GetComponent<tk2dSprite>();
					int spriteIndex = girdNumMap[girdNum];
					spriteRender.spriteId = spriteBlocks[spriteIndex].GetComponent<tk2dSprite>().spriteId;
				}
			}
		}

		Destroy (curGird);

		GameData.getInstance ().CurScoreNum = preScoreNum;
		//var itemLayer = GameObject.Find("itemLayer").GetComponent<ItemLayer>();
		itemLayer.updateInfo();

		int nextGirdNum = 0;
		if(nextGird)
		{
			nextGirdNum = curGird.GetComponent<Gird> ().Num;
			nextGird.GetComponent<Gird>().Num = preCreateNum;
			var spriteRender = nextGird.GetComponent<tk2dSprite>();
			int spriteIndex = girdNumMap[preCreateNum];
			spriteRender.spriteId = spriteBlocks[spriteIndex].GetComponent<tk2dSprite>().spriteId;
		}
		else
		{
			nextGirdNum = preCreateNum;
		}
		if(nextGirdNum >= 0)
		{
			createNextGird (girdNumMap[nextGirdNum],girdNumMap[preNextGirdNum]);
		}
		else
		{
			createNextGird();
		}


		//return;
		if(curMaxGird)
		{
			if(preMaxGirdNum ==0) 
			{
				Destroy(curMaxGird);
			}
			else
			{
				var spriteRender = curMaxGird.GetComponent<tk2dSprite>();
				int spriteIndex = girdNumMap[preMaxGirdNum];
				spriteRender.spriteId = spriteBlocks[spriteIndex].GetComponent<tk2dSprite>().spriteId;
				curMaxGird.GetComponent<Gird>().Num = preMaxGirdNum;
			}
		}
		else
		{
			if(preMaxGirdNum > 0)
			{
				var pos = getPosByRowCol(0,-1);
				curMaxGird = GameObject.Instantiate (girdBlocks[0], pos, Quaternion.identity) as GameObject;
				curMaxGird.transform.parent = gameObject.transform;
				curMaxGird.GetComponent<Gird>().Num = preMaxGirdNum;
				var spriteRender = curMaxGird.GetComponent<tk2dSprite>();
				int spriteIndex = girdNumMap[preMaxGirdNum];
				spriteRender.spriteId = spriteBlocks[spriteIndex].GetComponent<tk2dSprite>().spriteId;
			}
		}

		storeCurBoardInfo ();
	}
	
	void mergeGirds()
	{
		Debug.Log ("mergeGirds");
		int curRow = curGird.GetComponent<Gird> ().ROW;
		int curCol = curGird.GetComponent<Gird> ().COL;
		bool canMerge = false;

		for(int i=0;i<searchDir.GetLength(0);i++)
		{
			int dRow = searchDir[i,0];
			int dCol = searchDir[i,1];
			if(testCanMerge(curRow+dRow,curCol+dCol))
			{
				canMerge = true;
				executeMerge(curRow+dRow,curCol+dCol);
				break;
			}
		}

		//test [col,Constant.COL-1]
		if(canMerge == false)
		{
			int fromeCol = System.Math.Max(curCol,0);
			for(int j= fromeCol;j<=Constant.COL-1;j++)
			{
				for(int i= Constant.ROW-1 ; i>=0;i--)
				{
					var gird = girdArray[i,j];
					if(gird)
					{
						for(int k=0;k<searchDir.GetLength(0);k++)
						{
							int dRow = searchDir[k,0];
							int dCol = searchDir[k,1];
							var curGirdCopy = curGird;
							curGird = gird;
							if(testCanMerge(i+dRow,j+dCol))
							{
								canMerge = true;
								executeMerge(i+dRow,j+dCol);
								return;
							}
							else
							{
								curGird = curGirdCopy;
							}
						}
					}
				}
			}
		}

		if(canMerge == false)
		{
			if(isExchangingBlock == false)
			{
				createNextGird();
				//var itemLayer = GameObject.Find("itemLayer").GetComponent<ItemLayer>();
				itemLayer.setPreBtnkActive(true);   
			}
			else
			{
				if(block_exchange_first == null && block_exchange_second == null)
				{
					//resume pre curGird 
					curGird = block_exchange_cur_copy;
					this.resumeExchangeStatus();
					//var itemLayer = GameObject.Find("itemLayer").GetComponent<ItemLayer>();
					itemLayer.resumeExchangeStatus ();
				}
				else if(block_exchange_second)
				{
					curGird = block_exchange_second;
					block_exchange_second = null;
					mergeGirds();
				}
			}
		}
	}

	void executeMerge(int aimRow , int aimCol)
	{
		var audioManager = Singleton.getInstance ("audioManager").GetComponent<AudioManager> ();
		audioManager.playAudio("clear");
		int curRow = curGird.GetComponent<Gird> ().ROW;
		int curCol = curGird.GetComponent<Gird> ().COL;
		int dRow = aimRow - curRow;
		int dCol = aimCol - curCol;
		isCurGirdMoving = true;
		float moveTime = 0.07f;
		Vector3 toPos = curGird.transform.position + new Vector3 ( dCol * girdWidth,dRow * girdHeight,0);
		var seq = DOTween.Sequence ();
		var moveTo = curGird.transform.DOMove (toPos, moveTime);
		seq.Append (moveTo).AppendInterval(0.03f).AppendCallback (()=>{
			Debug.Log("Merge OK");
			isCurGirdMoving = false;
			//left bottom gird
			GameObject aimGird = null;
			if(aimRow == 0 && aimCol == -1)
			{
				aimGird = curMaxGird;
			}
			else
			{
				aimGird = girdArray[aimRow,aimCol];
				girdArray[aimRow,aimCol] = curGird;
			}
			int sum1 = curGird.GetComponent<Gird>().Num;
			int sum2 = 0;
			if(aimGird) 
			{
				sum2 = aimGird.GetComponent<Gird>().Num;
			}
			var sum = sum1 + sum2;
			curGird.GetComponent<Gird>().Num = sum;
			curGird.GetComponent<Gird>().ROW = aimRow;
			curGird.GetComponent<Gird>().COL = aimCol;
			var spriteRender = curGird.GetComponent<tk2dSprite>();
			int spriteIndex = girdNumMap[sum];
			var aimSpriteRender = spriteBlocks[spriteIndex].GetComponent<tk2dSprite>();
			spriteRender.spriteId = aimSpriteRender.spriteId;
			Debug.Log("Del Block");
			addScore(curGird.transform.position,sum);
			//del the aim's ref 
			if(block_exchange_second == aimGird)
			{
				block_exchange_second = null;
			}
			if(block_exchange_first == aimGird) 
			{
				block_exchange_first = null;
			}
			Destroy(aimGird);
			if(aimRow == 0 && aimCol == -1)
			{
				curMaxGird = curGird;
				int maxGirdNum = GameData.getInstance().BestGirdNum;
				if(sum > maxGirdNum)
				{
					GameData.getInstance().BestGirdNum = sum;
					audioManager.playAudio("win");
					GA.SetUserLevel(sum);
				}
			}
			mergeGirds();
		});

		//curGird.transform.localScale = new Vector3 (1.5f, 1.5f, 1);
		//curGird.transform.position = curGird.transform.position + Vector3.back;
		var scaleTo = curGird.transform.DOScale (new Vector3 (1.2f, 1.2f,1), moveTime*0.7f);
		var seq2 = DOTween.Sequence ();
		seq2.Append(scaleTo).AppendCallback(()=>{
			curGird.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			//curGird.transform.position = curGird.transform.position + Vector3.forward;
		});

		// Insert a scale tween for the whole duration of the Sequence
		seq.Insert(0, seq2);

		GameObject aimGird2 = null;
		if(aimRow == 0 && aimCol == -1)
		{
			aimGird2 = curMaxGird;
		}
		else
		{
			aimGird2 = girdArray[aimRow,aimCol];
		}
		if(aimGird2)
		{
			aimGird2.transform.DOScale(new Vector3(0,0,1),moveTime*0.7f);
		}

		for(int i= curRow;i<Constant.ROW-1;i++)
		{
			var upGird = girdArray[i+1,curCol];
			if(upGird)
			{
				girdArray[i,curCol] = upGird;
				var upPos = upGird.transform.position;
				upPos += new Vector3(0,-girdHeight,0);
				upGird.transform.DOMove(upPos,moveTime);
				upGird.GetComponent<Gird>().ROW = i;
				upGird.GetComponent<Gird>().COL = curCol;
			}
			else
			{
				girdArray[i,curCol] = null;
			}
		}
		if(curCol<Constant.COL)
		{
			girdArray[Constant.ROW-1,curCol] = null;
		}
	}

	void addScore(Vector3 pos , int addNum)
	{
		var label = Instantiate(Resources.Load ("Prefab/textMesh")) as GameObject;
		//var itemLayer = GameObject.Find("itemLayer").GetComponent<ItemLayer>();
		label.transform.position = pos;
		label.GetComponent<tk2dTextMesh> ().text = "+"+addNum;
		var seq = DOTween.Sequence ();
		var moveTo = label.transform.DOMove (curScorePos, 0.6f).SetEase (Ease.InQuad);
		GameData.getInstance().addScore(addNum);
		seq.Append (moveTo).AppendCallback (() => {
			itemLayer.updateInfo();
			Destroy(label);
		});
		itemLayer.addDiamondByScore ();
		label.transform.localScale = new Vector3 (1.5f, 1.5f, 1.0f);
		var scaleTo = label.transform.DOScale (0.2f,0.6f).SetEase (Ease.InQuint);
		seq.Insert (0, scaleTo);
	}

	bool testCanMerge(int nextRow , int nextCol)
	{
		if(nextRow<0 || nextCol >= Constant.COL) return false;

		GameObject tempGird = null;
		if(nextRow==0 && nextCol == -1)
		{
			if(curMaxGird == null)
			{
				return true;
			}
			else
			{
				tempGird = curMaxGird;
			}

		}
		else if(nextCol >=0)
		{
			tempGird = girdArray [nextRow, nextCol];
		}

		if (tempGird == null) return false;
		int curNum = curGird.GetComponent<Gird> ().Num;
		int nextNum = tempGird.GetComponent<Gird> ().Num;
		int sum = curNum + nextNum;
		int [] testArray = new int[]{1,2,3,5};
		int digit = getDigit (sum);
		int divisor = (int)System.Math.Pow (10, digit - 1);
		foreach(var i in testArray)
		{
			if(sum == i*divisor) 
			{
				return true;
			}
		}

		return false;
	}

	int getDigit(int num)
	{
		int i=0;
		while(num !=0)
		{
			i++;
			num /= 10;
		}
		return i;
	}

	void testGameOver()
	{
		if(curGird)
		{
			var gird = curGird.GetComponent<Gird>();
			int col = gird.COL;
			if(col == Constant.COL)
			{
				if(testCanMerge(Constant.ROW-1, Constant.COL-1) == false)
				{
					gameOver();
					CanDropDown = false;
				}
				else
				{
					downDelTime = downSpeed;
				}
			}
		}
	}

	public void gameOver()
	{
		Debug.Log ("gameOver");
		//var itemLayer = GameObject.Find("itemLayer").GetComponent<ItemLayer>();
		isGameOver = true;
		itemLayer.gameOver ();
	}
}
