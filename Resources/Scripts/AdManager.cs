#define USE_ADMOB
//#undef USE_ADMOB
#define USE_GDT
using UnityEngine;
using System.Collections;
public class AdManager
{
	private static AdManager _instance = null;
//	private static bool useAdmob = false;
	private AdManager()
	{

	}

	public static void init(bool isUseAdmob)
	{
//		useAdmob = isUseAdmob;
	}

	public static AdManager getInstance()
	{
		if(_instance == null) 
		{
			_instance = new AdManager();
			_instance.initData ();
		}
		return _instance;
	}

	void initData()
	{
		#if USE_ADMOB
		AdmobManager.getInstance ().requestBanner ();
		AdmobManager.getInstance ().requestInterstitial ();
//		#else

		#endif
	}


	public void showBanner()
	{
		#if USE_ADMOB
		AdmobManager.getInstance ().showBanner();
		#else
		Platform.getInstance().doShowBanner();
		#endif
	}

	public void showInterstitial()
	{
		#if USE_ADMOB
		AdmobManager.getInstance ().showInterstitial ();
		#else
		Platform.getInstance().doShowInterstital();
		#endif
	}

	public void hideBanner()
	{
		#if USE_ADMOB
		AdmobManager.getInstance ().hideBanner ();
		#else
		Platform.getInstance().doHideBanner();
		#endif
	}
}
