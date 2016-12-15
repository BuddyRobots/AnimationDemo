using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Manager : AllSceneSinglton<Manager>
{
	public static bool isMusicOn=true;
	[HideInInspector]
	public AudioSource bgAudio;
	[HideInInspector]
	public GameObject manager;

	void Start () 
	{
		manager=this.gameObject;
		Manager.isMusicOn=true;
		bgAudio=GameObject.Find("Manager").GetComponent<AudioSource>();
	}


	void Update () 
	{
		if (Manager.isMusicOn)
		{

			if (!bgAudio.isPlaying) 
			{
				bgAudio.Play ();
			}
		}
		if (!Manager.isMusicOn )
		{

			//关闭音乐 
			if (bgAudio.isPlaying) 
			{
				Debug.Log("pause");
				bgAudio.Pause ();
			}
		}

	}


}
