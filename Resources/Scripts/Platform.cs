using UnityEngine;
using System.Collections;
public class Platform
{
	enum CMD_ID
	{
		KEY_BACK = 0,
		DO_RATE,
		DO_FEEDBACK,
		DO_SHOWBANNER,
		DO_HIDEBANNER,
		DO_SHOWINTERSTITAL
	};

	private static Platform _instance = null;
	private Platform()
	{
		
	}
	
	public static Platform getInstance()
	{
		if(_instance == null) _instance = new Platform();
		return _instance;
	}


	public void keyBack()
	{
		if ( Application.platform == RuntimePlatform.Android &&(Input.GetKeyDown(KeyCode.Escape)))
		{
			var exitDialog = GameObject.Instantiate(Resources.Load ("Prefab/exitDialog")) as GameObject;
//			NativeMethod().Call("jni_called",new object[]{(int)CMD_ID.KEY_BACK,0});
		}
	}

	public void doRate()
	{
		if ( Application.platform == RuntimePlatform.Android)
		{
			NativeMethod().Call("jni_called",new object[]{(int)CMD_ID.DO_RATE,0});
		}
	}

	public void doShowBanner()
	{
		if ( Application.platform == RuntimePlatform.Android)
		{
			NativeMethod().Call("jni_called",new object[]{(int)CMD_ID.DO_SHOWBANNER,0});
		}
	}

	public void doHideBanner()
	{
		if ( Application.platform == RuntimePlatform.Android)
		{
			NativeMethod().Call("jni_called",new object[]{(int)CMD_ID.DO_HIDEBANNER,0});
		}
	}

	public void doShowInterstital()
	{
		if ( Application.platform == RuntimePlatform.Android)
		{
			NativeMethod().Call("jni_called",new object[]{(int)CMD_ID.DO_SHOWINTERSTITAL,0});
		}
	}

	public void doFeedBack()
	{
		if ( Application.platform == RuntimePlatform.Android)
		{
			NativeMethod().Call("jni_called",new object[]{(int)CMD_ID.DO_FEEDBACK,0});
		}
	}


	//获取当前App的Activity  
	public  AndroidJavaObject Current()  
	{  
		if (Application.platform == RuntimePlatform.Android)  
			return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");  
		else  
			return null;  
	}

	public AndroidJavaObject NativeMethod()
	{
		if (Application.platform == RuntimePlatform.Android)  
		{
			return new AndroidJavaClass("com.fuatnow.game.tools.NativeMethod").CallStatic<AndroidJavaObject>("getInstance");  
		}
		return null;  
	}
	
	//获取指定包名的Activity  
	public  AndroidJavaObject GetActivity(string package_name,string activity_name)  
	{  
		return new AndroidJavaClass(package_name).GetStatic<AndroidJavaObject>(activity_name);  
	}

	// UI线程中运行  
	public void RunOnUIThread(AndroidJavaRunnable r)  
	{  
		Current().Call("runOnUiThread", r);  
	}  
	
	//获取包名  
	public  string getPackageName()  
	{  
		return Current().Call<string>("getPackageName");  
	}  
	//讲解：call<返回值类型>("方法名");  
	
	//设置 不自动锁屏  
	public  void DisableScreenLock()  
	{  
		Current().Call<AndroidJavaObject>("getWindow")  
			.Call("addFlags",128);  
	}  
	//讲解：call("方法名",参数1);  
	
	// 获取内置SD卡路径  
	public  string GetStoragePath()  
	{  
		if (Application.platform == RuntimePlatform.Android)  
			return new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getPath");  
		else  
			return "d:/movie";  
	}   
	//讲解：new AndroidJavaClass("全类名")  ---new一个Android原生类  
	//讲解：CallStatic<返回类型>("方法名")  ---静态方法获取一个Android原生类型

}