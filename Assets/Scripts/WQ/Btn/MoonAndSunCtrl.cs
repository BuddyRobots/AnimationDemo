using UnityEngine;
using System.Collections;

public class MoonAndSunCtrl : MonoBehaviour 
{
	public static MoonAndSunCtrl _instance;

	[HideInInspector]
	public  bool isDaytime = true;

	private GameObject sunBtn;
	private GameObject moonBtn;



	void Awake()
	{
		_instance = this;

	}

	void OnEnable()
	{
		isDaytime = true;
		sunBtn = transform.Find ("SunBtn").gameObject;
		moonBtn = transform.Find ("MoonBtn").gameObject;
		sunBtn.SetActive (true);
		moonBtn.SetActive (false);

	}

	void Start () 
	{
		UIEventListener.Get (sunBtn).onClick = OnSunBtnClick;
		UIEventListener.Get (moonBtn).onClick = OnMoonBtnClick;

	}
	

	void Update () 
	{
		if (isDaytime && moonBtn.activeSelf) 
		{
			moonBtn.SetActive (false);
			sunBtn.SetActive (true);
		}
		if (!isDaytime && sunBtn.activeSelf) 
		{
			moonBtn.SetActive (true);
			sunBtn.SetActive (false);
		}
	
	}

	/// <summary>
	/// click sunBtn to switch to night mode
	/// </summary>
	/// <param name="btn">Button.</param>
	void OnSunBtnClick(GameObject btn)
	{
		isDaytime = false;

	}

	/// <summary>
	/// click moonBtn to switch to day mode
	/// </summary>
	/// <param name="btn">Button.</param>
	void OnMoonBtnClick(GameObject btn)
	{
		isDaytime = true;
	}
}
