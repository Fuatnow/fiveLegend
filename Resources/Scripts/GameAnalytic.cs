using UnityEngine;
using System.Collections;
using Umeng;
public class GameAnalytic : MonoBehaviour 
{

	public string channel_id = "";
	// Use this for initialization
	void Start () 
	{
		if (channel_id.Length < 1)	channel_id = "360";
		//请到 http://www.umeng.com/analytics 获取app key
		GA.StartWithAppKeyAndChannelId ("5644bf5f67e58ecf350009ab" , channel_id);	
		//调试时开启日志 发布时设置为false
		GA.SetLogEnabled (true);
		var bombTool = Singleton.getInstance ("gameConfig").GetComponent<BombTool> ();
		if(bombTool.ObjectId == null)
		{
			GA.ProfileSignIn (bombTool.ObjectId);
		}
		else
		{
			GA.ProfileSignIn ("000000000");
		}
	}
}
