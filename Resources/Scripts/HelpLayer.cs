using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HelpLayer : MonoBehaviour {

	private float winWidth;
	private float winHeight;
	private float offsetX;
	private Vector3 startTouchPos;
	private Vector2 prePos;
	private int curPage = 1;
	private GameObject help = null;

	private Sequence seq = null;
	void Awake()
	{
		var gameCamera = GameObject.Find ("camera").GetComponent<GameCamera> ();
		winWidth = gameCamera.winWidth;
		winHeight = gameCamera.winHeight;
		help = gameObject.transform.Find ("help").gameObject;
	}

	// Use this for initialization
	void Start () 
	{
	
	}

	public void resetChildPos()
	{
		var gameCollider = gameObject.GetComponent<BoxCollider2D> ();
		gameCollider.size = new Vector2 (winWidth / 100 , winHeight / 100);

		var swallowBg = gameObject.transform.Find ("swallowBg").gameObject;var sColor = swallowBg.GetComponent<tk2dSlicedSprite> ().color;
		sColor.a = 0.8f;
		swallowBg.GetComponent<tk2dSlicedSprite> ().color = sColor;
		swallowBg.GetComponent<tk2dSlicedSprite> ().dimensions = new Vector2 (winWidth,winHeight);
		swallowBg.SetActive (true);
		var swallowBtn = swallowBg.transform.Find ("swallowBtn").gameObject;
		var swallowBounds = swallowBtn.GetComponent<tk2dSprite> ().GetBounds();
		float swallowBtnWidth = swallowBounds.size.x * 100;
		float swallowBtnHeight = swallowBounds.size.y * 100;
		swallowBtn.GetComponent<tk2dSprite>().scale = new Vector3 (winWidth / swallowBtnWidth, winHeight / swallowBtnHeight,1);

		
		var help1 = help.transform.Find ("help1").gameObject;
		var help1Pos = help1.transform.position;
		help1Pos.x = 0;
		help1Pos.y = 0;
		help1.transform.localPosition = help1Pos;
		
		var help2 = help.transform.Find ("help2").gameObject;
		var help2Pos = help2.transform.position;
		help2Pos.x = winWidth / 100;
		help2Pos.y = 0;
		help2.transform.localPosition = help2Pos;
	}


	
	
	
	void OnMouseDown() 
	{  
		seq = null;
		startTouchPos = Input.mousePosition;
		prePos = help.transform.position;
		offsetX = -startTouchPos.x/100.0f + prePos.x;
	}
	
	void OnMouseDrag() 
	{
		var touchPos = Input.mousePosition;
		var selfPos = help.transform.position;
		selfPos.x = offsetX + touchPos.x / 100;
		help.transform.position = selfPos;
	}

	
	void OnMouseUp() 
	{
		var selfPos = help.transform.position;
		float disX = selfPos.x - prePos.x;
		float moveTime = 7;
		if(curPage == 1)
		{
			if(disX > -winWidth/100*0.03f)
			{
				moveTime = System.Math.Abs(disX/winWidth)*0.5f;
				var moveTo = help.transform.DOMoveX(0,0.5f).SetEase(Ease.OutQuad);
				seq = DOTween.Sequence();
				seq.Append(moveTo);
			}
			else
			{
				moveTime = (winWidth+disX)/winWidth*0.5f;
				var moveTo = help.transform.DOMoveX(-winWidth/100,moveTime).SetEase(Ease.OutQuad);
				seq = DOTween.Sequence();
				seq.Append(moveTo).AppendCallback(()=>{
					curPage = 2;
				});
			}
		}
		else if(curPage == 2)
		{
			if(disX > winWidth/100*0.03f)
			{
				moveTime = (winWidth-disX)/winWidth*0.5f;
				var moveTo = help.transform.DOMoveX(0,moveTime).SetEase(Ease.OutQuad);
				seq = DOTween.Sequence();
				seq.Append(moveTo).AppendCallback(()=>{
					curPage = 1;
				});
			}
			else if(disX < -winWidth/100*0.1f)
			{
				moveTime = (winWidth+disX)/winWidth*0.5f;
				var moveTo = help.transform.DOMoveX(-2*winWidth/100,moveTime).SetEase(Ease.OutQuad);
				seq = DOTween.Sequence();
				seq.Append(moveTo).AppendCallback(()=>{
					var curPos = help.transform.position;
					curPos.x =0;
					help.transform.position = curPos;
					curPage = 1;
					var gameBoard = GameObject.FindGameObjectWithTag (Constant.gameBoard);
					gameBoard.GetComponent<GameBoard>().isPuaseGame = false;
					gameObject.SetActive(false);
				});
			}
			else
			{
				moveTime = System.Math.Abs(disX/winWidth)*0.5f;
				var moveTo = help.transform.DOMoveX(-winWidth/100,moveTime).SetEase(Ease.OutQuad);
				seq = DOTween.Sequence();
				seq.Append(moveTo);
			}

		}
		if (seq != null)
		{
			seq.SetId ("seq");
		}
	}
}
