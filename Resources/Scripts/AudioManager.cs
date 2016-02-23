using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class AudioManager : MonoBehaviour {

	public AudioClip clear = null;
	public AudioClip down = null;
	public AudioClip gameover = null;
	public AudioClip move = null;
	public AudioClip pause = null;
	public AudioClip sou = null;
	public AudioClip tap = null;
	public AudioClip win = null;


	private bool canPlayAudio = true;
	private Dictionary<string,AudioClip> audioMap = new Dictionary<string,AudioClip> ();

	public bool CanPlayAudio
	{
		get 
		{ 
			return canPlayAudio;
		}
		set 
		{ 
			canPlayAudio = value;
			if(canPlayAudio)
				PlayerPrefs.SetInt ("canPlayAudio",1);
			else
				PlayerPrefs.SetInt ("canPlayAudio",0);
		}
	}

	void Awake()
	{
		canPlayAudio = true;
		int v = PlayerPrefs.GetInt ("canPlayAudio",1);
		if (v == 0) 
		{
			canPlayAudio = false;
		}

		audioMap ["clear"] = clear;
		audioMap ["down"] = down;
		audioMap ["gameover"] = gameover;
		audioMap ["move"] = move;
		audioMap ["pause"] = pause;
		audioMap ["sou"] = sou;
		audioMap ["tap"] = tap;
		audioMap ["win"] = win;
	}

	// Use this for initialization
	void Start () 
	{
	}


	public void playAudio(string audioName)
	{
		if(canPlayAudio)
		{
			var audioClip = audioMap [audioName];
			if(audioClip)
			{		
				GetComponent<AudioSource>().PlayOneShot (audioClip);
			}
		}
	}

}
