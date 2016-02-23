using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using cn.bmob.api;
using cn.bmob.tools;
using cn.bmob.response;
using cn.bmob.http;
public class BombTool : MonoBehaviour
{
	private BmobUnity Bmob;
	private string objectId;
	public static BombTool instance = null;
	//
	private bool isSigningUpAccount = false;
	public string ObjectId
	{
		get 
		{ 
			return objectId;
		}
		set 
		{ 
			objectId = value;
			PlayerPrefs.SetString("objectId",objectId);		
		}
	}

	void Awake()
	{
		//注册调试打印对象
		BmobDebug.Register(print);
		//获取Bmob的服务组件
		Bmob = gameObject.GetComponent<BmobUnity>();
		instance = this;
		//
		objectId = PlayerPrefs.GetString ("objectId", "");
		if(objectId.Length == 0)
		{
			signUp();
		}
	}



	void Start()
	{
		
	}

	public void submitAttribute()
	{
		int diamondNum = GameData.getInstance ().DiamondNum;
		int playTimes = GameData.getInstance ().PlayTimes;
		var dic = new Dictionary<string, object> ();
		Debug.Log (objectId + diamondNum + playTimes);
		dic.Add ("objectId", objectId);
		dic.Add ("diamondNum", diamondNum);
		dic.Add ("playTimes", playTimes);
		var callback = new BmobCallback<EndPointCallbackData<Hashtable>>((resp, exception) =>{});
		Bmob.Endpoint<Hashtable> ("submitAttribute", dic, callback);
	}
	
	public void signUp()
	{
		Debug.Log ("sign Up");
		if(isSigningUpAccount) return;
		isSigningUpAccount = true;
		var userName = "游客";
		if(Application.systemLanguage == SystemLanguage.English)
		{
			userName = "tourist";
		}
		signUp (userName,0,(resp, exception) =>{
			isSigningUpAccount = false;
			if (exception != null) 
			{
				print ("调用失败, 失败原因为： " + exception.Message);
				return;
			}
			var hashTab = resp.data;
			if(hashTab.ContainsKey("objectId") == true)
			{
				ObjectId = hashTab["objectId"].ToString();
			}
		});
	}
	
	public void signUp(string userName,BmobCallback<EndPointCallbackData<Hashtable>> callback)
	{
		signUp (userName, 0,callback);
	}
	
	public void signUp(string userName ,int bestScore,BmobCallback<EndPointCallbackData<Hashtable>> callback)
	{
		var dic = new Dictionary<string, object> ();
		dic.Add ("bestScore", bestScore);
		dic.Add ("userName", userName);
		if(callback == null) 
		{
			callback = new BmobCallback<EndPointCallbackData<Hashtable>>((resp, exception) =>{});
		}
		Bmob.Endpoint<Hashtable> ("signUp", dic, callback);
	}
	
	public void updateUserName(string objectId , string userName,BmobCallback<EndPointCallbackData<Hashtable>> callback)
	{
		Debug.Log ("updateUserName");
		var dic = new Dictionary<string, object> ();
		dic.Add ("objectId", objectId);
		dic.Add ("userName", userName);
		if(callback == null) 
		{
			callback = new BmobCallback<EndPointCallbackData<Hashtable>>((resp, exception) =>{});
		}
		Bmob.Endpoint<Hashtable> ("updateUserName", dic, callback);
	}
	
	public void submitScore(string objectId,int bestScore,int maxGirdNum, BmobCallback<EndPointCallbackData<Hashtable>> callback)
	{
		Debug.Log ("submitScore");
		var dic = new Dictionary<string, object> ();
		dic.Add ("objectId", objectId);
		dic.Add ("bestScore", bestScore);
		dic.Add ("maxGirdNum", maxGirdNum);
		if(callback == null) 
		{
			callback = new BmobCallback<EndPointCallbackData<Hashtable>>((resp, exception) =>{});
		}
		Bmob.Endpoint<Hashtable> ("submitScore", dic, callback);
	}
	
	public void getSelfRank(string objectId , bool getOtherInfo , BmobCallback<EndPointCallbackData<Hashtable>> callback)
	{
		Debug.Log ("getSelfRank");
		var dic = new Dictionary<string, object> ();
		dic.Add ("objectId", objectId);
		dic.Add ("getOtherInfo", getOtherInfo);
		if(callback == null) 
		{
			callback = new BmobCallback<EndPointCallbackData<Hashtable>>((resp, exception) =>{});
		}
		Bmob.Endpoint<Hashtable> ("getSelfRank", dic, callback);
	}
}