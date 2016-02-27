#define USE_ADGDT

#if USE_ADGDT
using UnityEngine;
using System.Collections;

public class AdGdtManager
{

	private static AdGdtManager _instance = null;

	private AdGdtManager()
	{

	}

	public static AdGdtManager getInstance()
	{
		if(_instance == null) 
		{
			_instance = new AdGdtManager();
		}
		return _instance;
	}


}
#endif