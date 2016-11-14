using UnityEngine;
using System.Collections;

public delegate void VoidDelegate();//delegate with a void return value & a void parameter


/// <summary>
/// singlton in a scene of the game
/// </summary>
public class SceneSinglton<T>:MonoBehaviour
	where T:Component
{
	private static T _instance;
	public static T Instance
	{
		get {
			if (_instance == null) 
			{
				GameObject go = new GameObject ();
				go.hideFlags = HideFlags.HideAndDontSave;
				_instance = (T)go.AddComponent (typeof(T));
				return _instance;
			} 
			else 
			{
				return _instance;
			}
		}
	}
}

/// <summary>
/// singlton in a game
/// </summary>
public class AllSceneSinglton<T>:MonoBehaviour
	where T:Component
{
	private static T _instance;
	public static T Instance
	{
		get {
			if (_instance == null) 
			{
				GameObject go = new GameObject ();
				go.hideFlags = HideFlags.HideAndDontSave;
				DontDestroyOnLoad (go);
				_instance = (T)go.AddComponent(typeof(T));
				return _instance;
			} 
			else 
			{
				return _instance;
			}
		}
	}
}
