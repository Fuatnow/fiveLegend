using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class NotificationCenterExtra : MonoBehaviour
{
	private static NotificationCenterExtra defaultCenter = null;
	public delegate void  NotificationEventHandler(object sender,object data);
	public static NotificationCenterExtra DefaultCenter ()
	{
		// If the defaultCenter doesn't already exist, we need to create it
		if (!defaultCenter) 
		{
			// Because the NotificationCenter is a component, we have to create a GameObject to attach it to.
			GameObject notificationObject = new GameObject("Default Notification Center");
			// Add the NotificationCenter component, and set it as the defaultCenter
			defaultCenter = notificationObject.AddComponent<NotificationCenterExtra>();
			DontDestroyOnLoad(defaultCenter);
		}

		return defaultCenter;
	}

	// Our hashtable containing all the notifications.  Each notification in the hash table is an ArrayList that contains all the observers for that notification.
	Hashtable notifications = new Hashtable();

	// AddObserver includes a version where the observer can request to only receive notifications from a specific object.  We haven't implemented that yet, so the sender value is ignored for now.
	public void AddObserver (String name,NotificationEventHandler eHandler ) 
	{
		AddObserver(name, eHandler, null); 
	}
	public void AddObserver (String name,NotificationEventHandler eHandler, object sender)
	{
		// If the name isn't good, then throw an error and return.
		if (name == null || name == "")
		{ 
			Debug.Log("Null name specified for notification in AddObserver."); 
			return;
		}
		// If this specific notification doens't exist yet, then create it.
		if (!notifications.ContainsKey (name))
		{
			var tmpHandler = new NotificationEventHandler (eHandler);
			notifications [name] = tmpHandler;
		}
		else 
		{
			NotificationEventHandler eventHandler = (NotificationEventHandler)notifications[name];
			eventHandler += eHandler;
			notifications [name] = eventHandler;
		}

	}

	// RemoveObserver removes the observer from the notification list for the specified notification type
	public void RemoveObserver (String name,NotificationEventHandler eHandler ) {
		NotificationEventHandler eventHandler= (NotificationEventHandler)notifications[name]; //change from original
		if (eventHandler != null) {
			eventHandler -= eHandler;
		} else 
		{
			Debug.Log("Null name specified for notification in RemoveObserver."); 
		}		
	}

	// PostNotification sends a notification object to all objects that have requested to receive this type of notification.
	// A notification can either be posted with a notification object or by just sending the individual components.
	public void PostNotification (String aName,object aSender) 
	{
		PostNotification(aName, aSender, null);
	}
	public void PostNotification (String aName,object aSender, object aData)
	{ 
		PostNotification(new Notification(aName,aSender, aData)); 
	}
	public void PostNotification (Notification aNotification)
	{
		// First make sure that the name of the notification is valid.
		if (aNotification.name == null || aNotification.name == "") 
		{ 
			Debug.Log("Null name sent to PostNotification."); 
			return;
		}
		// Obtain the notification list, and make sure that it is valid as well
		NotificationEventHandler eHandler = (NotificationEventHandler)notifications[aNotification.name]; //change from original
		if (eHandler == null) 
		{ 
			Debug.Log("Notify list not found in PostNotification."); 
			return; 
		}
		eHandler (aNotification.sender, aNotification.data);
	}
}

// The Notification class is the object that is sent to receiving objects of a notification type.
// This class contains the sending GameObject, the name of the notification, and optionally a hashtable containing data.
public class Notification 
{
	public String name;
	public object sender;
	public object data;

	public Notification (String aName,object aSender)
	{ 
		name = aName; 
		sender = aSender; 
		data = null; 
	}
	public Notification (String aName,object aSender, object aData) 
	{ 
		name = aName; 
		sender = aSender; 
		data = aData;
	}
}
