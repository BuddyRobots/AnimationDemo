using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//public class Manager : AllSceneSinglton<Manager>
//{
//	public static bool isMusicOn=true;
//	[HideInInspector]
//	public AudioSource bgAudio;
////	[HideInInspector]
////	public GameObject manager;
//
//	[HideInInspector]
//	public  Texture2D texture;
//
//	void OnEnable() 
//	{
////		manager=this.gameObject;
//		Manager.isMusicOn=true;
//		bgAudio=GameObject.Find("Manager").GetComponent<AudioSource>();
//
//	}
//
//
//	void Update () 
//	{
//		if (Manager.isMusicOn)
//		{
//			
//			if (bgAudio && !bgAudio.isPlaying) 
//			{
//				bgAudio.Play ();
//			}
//		}
//		if (!Manager.isMusicOn )
//		{
//
//			//关闭音乐 
//			if (bgAudio && bgAudio.isPlaying) 
//			{
//				Debug.Log("pause");
//				bgAudio.Pause ();
//			}
//		}
//
//	}
//
//
//}



public class Manager : MonoBehaviour
{
	public static Manager Instance=null;

	public static bool isMusicOn=true;
	[HideInInspector]
	public AudioSource bgAudio;
	//	[HideInInspector]
	//	public GameObject manager;

	[HideInInspector]
	public  Texture2D texture;



	void Awake()
	{

		if (Instance==null) {
			Instance=this;
		}
		else if (Instance!=this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}


	void Start() 
	{
		Manager.isMusicOn=true;
		bgAudio=GameObject.Find("Manager").GetComponent<AudioSource>();

	}


	void Update () 
	{
		if (Manager.isMusicOn)
		{

			if (bgAudio && !bgAudio.isPlaying) 
			{
				bgAudio.Play ();
			}
		}
		if (!Manager.isMusicOn )
		{

			//关闭音乐 
			if (bgAudio && bgAudio.isPlaying) 
			{
				Debug.Log("pause");
				bgAudio.Pause ();
			}
		}

	}


}