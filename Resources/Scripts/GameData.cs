using UnityEngine;
using System.Collections;

public class GameData
{
	private static GameData _instance = null;
	private int bestScoreNum = 0;
	private int curScoreNum = 0;
	private int bestGirdNum = 0;
	private int diamondNum = 0;
	public  int useDiamondNum_backPreStep = 50;
	public  int useDiamondNum_exchangeBlock = 100;
	public  int playTimes = 0;
	private bool canLoadFullAd = false;

	public int BestScoreNum
	{
		get 
		{ 
			return bestScoreNum;
		}
		set 
		{ 
			PlayerPrefs.SetInt("bestScoreNum",value);
			bestScoreNum = value;
		}
	}

	public int CurScoreNum
	{
		get 
		{ 
			return curScoreNum;
		}
		set 
		{ 
			curScoreNum = value;
		}
	}

	public int BestGirdNum
	{
		get 
		{ 
			return bestGirdNum;
		}
		set 
		{ 
			PlayerPrefs.SetInt("bestGirdNum",value);
			bestGirdNum = value;
		}
	}

	public int PlayTimes
	{
		get 
		{ 
			return playTimes;
		}
		set 
		{ 
			PlayerPrefs.SetInt("playTimes",value);
			playTimes = value;
		}
	}

	public int DiamondNum
	{
		get 
		{ 
			return diamondNum;
		}
		set 
		{ 
			PlayerPrefs.SetInt("diamondNum",value);
			diamondNum = value;
		}
	}

	public bool CanLoadFullAd
	{
		get 
		{ 
			return canLoadFullAd;
		}
		set 
		{ 
			canLoadFullAd = value;
		}
	}
	
	private GameData()
	{
		
	}

	public static GameData getInstance()
	{
		if(_instance == null) 
		{
			_instance = new GameData();
			_instance.initData ();
		}
		return _instance;
	}

	public void initData()
	{
		bestScoreNum = PlayerPrefs.GetInt ("bestScoreNum",0);
		bestGirdNum = PlayerPrefs.GetInt ("bestGirdNum");
		//DiamondNum = 30;
		//PlayerPrefs.SetInt ("diamondNum", 250000);
		diamondNum = PlayerPrefs.GetInt ("diamondNum", 500);
		playTimes = PlayerPrefs.GetInt ("playTimes", 0);
//		playTimes = 4;
	}

	public void addScore(int addNum)
	{
		CurScoreNum = CurScoreNum + addNum;
		if(CurScoreNum > BestScoreNum)
		{
			BestScoreNum = CurScoreNum;
		}
	}

	public void addDiamond(int addNum)
	{
		DiamondNum = DiamondNum + addNum;
	}

	public int getBaseDiamondNum(int propId)
	{
		int useDiamondNum = 0;
		switch(propId)
		{
		case 1:
			useDiamondNum = useDiamondNum_backPreStep;
			break;
		case 2:
			useDiamondNum = useDiamondNum_exchangeBlock;
			break;
		default:
			break;
		}
		return useDiamondNum;
	}
}