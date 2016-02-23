#define USE_ADMOB

#if USE_ADMOB
using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
public class AdmobManager
{
	private static AdmobManager _instance = null;

	private AdmobManager()
	{

	}

	public static AdmobManager getInstance()
	{
		if(_instance == null) 
		{
			_instance = new AdmobManager();
		}
		return _instance;
	}

	BannerView bannerView = null;
	public void requestBanner()
	{
		Debug.Log ("RequestBanner");
		#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-1039662318264516/6951694785";
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().
			AddTestDevice ("09D07C56257EEFEC78794EE077776E1A").
			AddTestDevice ("2D7E07D8B5296C18C770CB403CE2261D").
			AddTestDevice("BEBDE3B8905AB36B7E2ED39FEACCAB6D").
			Build();
		// Called when an ad request has successfully loaded.
		bannerView.AdLoaded += HandleOnBannerLoaded;
		bannerView.AdClosed += handleOnBannerClosed;
		// Load the banner with the request.
		bannerView.LoadAd(request);
	}

	public void HandleOnBannerLoaded(object sender, EventArgs args)
	{
		Debug.Log ("HandleOnBannerLoaded");
//		hideBanner ();
	}

	public void handleOnBannerClosed(object sender, EventArgs args)
	{
		Debug.Log ("handleOnBannerClosed");
		bannerView.Destroy ();
		bannerView = null;
		requestBanner ();
	}
	

	InterstitialAd interstitial = null;
	public void requestInterstitial()
	{
		Debug.Log ("RequestInterstitial");
		#if UNITY_ANDROID

		string adUnitId = "ca-app-pub-1039662318264516/8428427986";
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(adUnitId);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().
			AddTestDevice ("09D07C56257EEFEC78794EE077776E1A").
			AddTestDevice ("2D7E07D8B5296C18C770CB403CE2261D").
			AddTestDevice("BEBDE3B8905AB36B7E2ED39FEACCAB6D").
			Build();
		// Load the interstitial with the request.
		interstitial.AdClosed += handleOnInterstitialClosed;
		interstitial.LoadAd(request);
	}

	public void handleOnInterstitialClosed(object sender, EventArgs args)
	{
		Debug.Log ("HandleOnInterstitialClosed");
		interstitial.Destroy ();
		interstitial = null;
		requestInterstitial ();
	}

	public void showBanner()
	{
		if (bannerView != null) 
		{
			bannerView.Show();
		}
	}

	public void hideBanner()
	{
		if (bannerView != null) 
		{
			bannerView.Hide();
		}
	}

	public void showInterstitial()
	{
		if (interstitial != null && interstitial.IsLoaded()) 
		{
			interstitial.Show();
		}
	}
}
#endif