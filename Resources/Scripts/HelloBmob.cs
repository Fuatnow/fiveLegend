using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using cn.bmob.api;
using cn.bmob.tools;
public class HelloBmob : MonoBehaviour
{
	
	private BmobUnity Bmob;
	void Start()
	{
		//注册调试打印对象
		BmobDebug.Register(print);
		//获取Bmob的服务组件
		Bmob = gameObject.GetComponent<BmobUnity>();



//		//创建数据对象
//		var data = new BmobGameObject();
//		//设置值    
//		System.Random rnd = new System.Random();
//		data.score = rnd.Next(-50, 170);
//		data.playerName = "123";
//		data.cheatMode = false;

//		getSelfRank ();
	}

	void signUp()
	{
		signUp ("tourist");
	}

	void signUp(string userName)
	{
		signUp (userName, 0);
	}

	void signUp(string userName ,int bestScore)
	{
		var dic = new Dictionary<string, object> ();
		dic.Add ("bestScore", bestScore);
		dic.Add ("userName", userName);
		Bmob.Endpoint<Hashtable>("signUp", dic,(resp, exception) => 
		                         {
			if (exception != null)
			{
				print("调用失败, 失败原因为： " + exception.Message);
				return;
			}
			
			//print("返回对象为： " + resp);
		});
	}

	void updateUserName(string objectId , string userName)
	{
		var dic = new Dictionary<string, object> ();
		dic.Add ("objectId", objectId);
		dic.Add ("userName", userName);
		Bmob.Endpoint<Hashtable>("updateUserName",dic,(resp, exception) => 
		                         {
			if (exception != null)
			{
				print("调用失败, 失败原因为： " + exception.Message);
				return;
			}
			
			//print("返回对象为： " + resp);
		});		
	}

	void submitScore(string objectId,int bestScore)
	{
		var dic = new Dictionary<string, object> ();
		dic.Add ("objectId", objectId);
		dic.Add ("bestScore", bestScore);
		Bmob.Endpoint<Hashtable>("submitScore",dic,(resp, exception) => 
		                         {
			if (exception != null)
			{
				print("调用失败, 失败原因为： " + exception.Message);
				return;
			}
			
			//print("返回对象为： " + resp);
		});
	}

	void getSelfRank(string objectId , bool getOtherInfo)
	{
		var dic = new Dictionary<string, object> ();
		dic.Add ("objectId", objectId);
		dic.Add ("getOtherInfo", getOtherInfo);
		Bmob.Endpoint<Hashtable>("getSelfRank",dic,(resp, exception) => 
		                         {
			if (exception != null)
			{
				print("调用失败, 失败原因为： " + exception.Message);
				return;
			}
			
			//print("返回对象为： " + resp);
		});
	}
}