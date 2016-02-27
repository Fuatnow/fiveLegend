using UnityEngine;
using System.Collections;
using SimpleJSON;
using cn.bmob.api;
using cn.bmob.tools;
using cn.bmob.http;
using cn.bmob.response;
public class RankLayer : MonoBehaviour 
{
	private GameObject alertNameLayer = null;
	private GameObject nickNameLabel = null;
	private GameObject bestScoreLabel = null;
	public GameObject scorllViewControll = null;
	public GameObject waitDialog = null;
	public GameObject loadFailDialog = null;
	private int sendTimes = 0;
	// Use this for initialization
	void Awake () 
	{
		alertNameLayer = gameObject.transform.FindChild ("AlertNameDialog").gameObject;
		nickNameLabel = gameObject.transform.Find ("selfRank/up/dynamicLabel/nickName").gameObject;
		bestScoreLabel = gameObject.transform.Find ("selfRank/up/dynamicLabel/bestScore").gameObject;

//		var rLabel = transform.Find ("selfRank/up/infoLabel/rankNum").gameObject.GetComponent<tk2dTextMesh> ();
//		var nLabel = transform.Find ("selfRank/up/infoLabel/nickName").gameObject.GetComponent<tk2dTextMesh> ();
//		var mLabel = transform.Find ("selfRank/up/infoLabel/maxGirdNum").gameObject.GetComponent<tk2dTextMesh> ();
//		var sLabel = transform.Find ("selfRank/up/infoLabel/scoreNum").gameObject.GetComponent<tk2dTextMesh> ();
//
//		if(Application.systemLanguage == SystemLanguage.English)
//		{
//			rLabel.text = "RankNum";
//			nLabel.text = "NickName";
//			mLabel.text = "Single";
//			mLabel.text = "Score";
//		}
	}
	public void updateView()
	{
		var name = PlayerPrefs.GetString ("nickName","Player");
		nickNameLabel.GetComponent<tk2dTextMesh> ().text = name;
		var score = PlayerPrefs.GetInt ("bestScore", 0);
		bestScoreLabel.GetComponent<tk2dTextMesh> ().text = score+"";
		if (waitDialog)
			waitDialog.SetActive (true);
		getRankData ();
	}

	void getRankData()
	{
		Debug.Log ("getRankData");
		NotificationCenterExtra.DefaultCenter ().RemoveObserver ("sendTimes"+sendTimes.ToString(), rank_callBack);
		sendTimes++;
		NotificationCenterExtra.DefaultCenter ().AddObserver ("sendTimes"+sendTimes.ToString(), rank_callBack);
		var rankCallBack = new BmobCallback<EndPointCallbackData<Hashtable>> (
			(resp, exception) =>{
			if(waitDialog != null)
			{
				waitDialog.SetActive(false);
			}
			if (exception != null) 
			{
				print ("调用失败, 失败原因为： " + exception.Message);
				if(loadFailDialog != null)
				{
					loadFailDialog.GetComponent<LoadFail>().reLoadFailAct();
				}
				return;
			}
			NotificationCenterExtra.DefaultCenter ().PostNotification("sendTimes"+sendTimes.ToString(),0,resp.data);
		});
		var objectId = BombTool.instance.ObjectId;
		if(objectId.Length > 0)
		{
			BombTool.instance.getSelfRank(objectId, true, rankCallBack);
		}
		else
		{
			BombTool.instance.signUp();
		}
	}

	void rank_callBack(object test , object data)
	{
		Hashtable hashTab = (Hashtable)data;
		// 遍历哈希表
		foreach (DictionaryEntry de in hashTab)
		{
			Debug.Log( de.Key + " " +de.Value);
		}

		testVertical verticalAreaObj = scorllViewControll.GetComponent<testVertical> ();

		var res = (bool)hashTab ["res"];
		if (res == false) 
		{
			//
			Debug.Log("error");
			return;
		}

		string selfInfo = hashTab ["selfInfo"].ToString();

		var selfNode = JSONNode.Parse (selfInfo);
		var selfName = selfNode ["name"].ToString ();
		if(selfName.Length>2) selfName = selfName.Substring(1,selfName.Length-2);
		var selfBestScore = selfNode ["bestScore"].AsInt;
		var winPer = selfNode ["winPer"].ToString ();
		if(winPer.Length>2) winPer = winPer.Substring(1,winPer.Length-2);
		PlayerPrefs.SetString ("nickName", selfName);
		if(verticalAreaObj)
		{
			var nickNameLabel = gameObject.transform.Find ("selfRank/up/dynamicLabel/nickName").gameObject;
			nickNameLabel.GetComponent<tk2dTextMesh> ().text = selfName;

			var bestScoreLabel = gameObject.transform.Find ("selfRank/up/dynamicLabel/bestScore").gameObject;
			bestScoreLabel.GetComponent<tk2dTextMesh> ().text = selfBestScore+"";

			var winPerLabel = gameObject.transform.Find ("selfRank/up/dynamicLabel/winPer").gameObject;
			winPerLabel.GetComponent<tk2dTextMesh> ().text = winPer;
		}


		string otherInfo = hashTab ["otherInfo"].ToString();
		var infoNode = JSONNode.Parse(otherInfo);
		verticalAreaObj.clearAllItem ();
		for(var i=0;i<infoNode.Count;i++)
		{
			var obj = infoNode[i];
			var name = obj["name"].ToString();
			var bestScore = obj["bestScore"].AsInt;
			var objectId = obj["objectId"].ToString();
			var girdNum = obj["maxGirdNum"].AsInt;
			if(name.Length>2) name = name.Substring(1,name.Length-2);
			if(objectId.Length>2) objectId = objectId.Substring(1,objectId.Length-2);
//			foreach(string e in shsh)
//			{
//				Debug.Log(e);
//			}
//			Debug.Log(i+"->"+"name:"+name+" bestScore:"+bestScore+" objectId:"+objectId);

			testVertical.ItemDef item = new testVertical.ItemDef();
			item.name = name;
			item.objectId = objectId;
			item.score = bestScore;
			item.girdNum = girdNum;
			item.serial = i+1;

			if(verticalAreaObj)verticalAreaObj.addItem(item);
		}
		if(verticalAreaObj)verticalAreaObj.UpdateListGraphics ();
	}

	void alter_callBack(Hashtable hashTab)
	{
		// 遍历哈希表
		foreach (DictionaryEntry de in hashTab)
		{
			Debug.Log( de.Key + " " +de.Value);
		}

		var res = (bool)hashTab ["res"];
		if (res == false) 
		{
			//
			Debug.Log("error");
			return;
		}
//		string alertName = hashTab ["name"].ToString ();
//		var nickName = gameObject.transform.Find("selfRank/dynamicLabel/nickName").gameObject;
//		nickName.GetComponent<tk2dTextMesh> ().text = alertName;
		getRankData ();
	}

	
	
	void alterNickName()
	{
		Debug.Log ("alterNickName");
		if(alertNameLayer)
		{
			alertNameLayer.SetActive(true);
		}
	}
	
	void alterOk()
	{
		Debug.Log("alertOk");
		if(alertNameLayer)
		{
			var TextInput = alertNameLayer.transform.FindChild("TextInput").gameObject;
			string inStr = TextInput.GetComponent<tk2dUITextInput>().Text;
			TextInput.GetComponent<tk2dUITextInput>().Text = "";
			alertNameLayer.SetActive(false);
			var alertCallBack = new BmobCallback<EndPointCallbackData<Hashtable>> (
				(resp, exception) =>{
				if (exception != null) 
				{
					print ("调用失败, 失败原因为： " + exception.Message);
					return;
				}
				alter_callBack(resp.data);
			});
			var objectId = BombTool.instance.ObjectId;
			if(objectId.Length > 0)
			{
				if(inStr.Length>0)BombTool.instance.updateUserName(objectId, inStr,alertCallBack);
			}
			else
			{
				BombTool.instance.signUp();
			}
		}
	}

	void alterCancel()
	{
		if(alertNameLayer)
		{
			var TextInput = alertNameLayer.transform.FindChild("TextInput").gameObject;
			TextInput.GetComponent<tk2dUITextInput>().Text = "";
			alertNameLayer.SetActive(false);
		}
	}

	void close_callBack()
	{
		gameObject.SetActive (false);
	}


	void OnDestroy()
	{
		waitDialog = null;
		loadFailDialog = null;
		NotificationCenterExtra.DefaultCenter ().RemoveObserver ("sendTimes"+sendTimes.ToString(), rank_callBack);
	}
}
