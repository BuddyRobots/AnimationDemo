using UnityEngine;
using System.Collections;

public class MusicCtrl : MonoBehaviour {

	public static bool isMusicOn=true;
	private GameObject musicOnBtn;
	private GameObject musicOffBtn;
	private AudioSource bgAudio;

	void Start () 
	{
		musicOnBtn = transform.Find ("MusicOnBtn").gameObject;
		musicOffBtn = transform.Find ("MusicOffBtn").gameObject;

		UIEventListener.Get(musicOnBtn).onClick = OnMusicOnBtnClick;
		UIEventListener.Get(musicOffBtn).onClick = OnMusicOffBtnClick;
	}

	void OnEnable()
	{


		bgAudio=GameObject.Find("Main Camera").GetComponent<AudioSource>();

	}

	void Update () 
	{
		if (isMusicOn && musicOffBtn.activeSelf)
		{
			//如果状态是开，而当前下面的“音乐关” 按钮开着的话，关闭它，并且打开 “音乐开” 的按钮
			musicOnBtn.SetActive(true);
			musicOffBtn.SetActive(false);
			if (!bgAudio.isPlaying) 
			{
				bgAudio.Play ();
			}
		}
		if (!isMusicOn && musicOnBtn.activeSelf)
		{
			//如果状态是关，而当前下面的 “音乐开” 按钮开着的话，关闭它，并且打开 “音乐关” 的按钮
			musicOnBtn.SetActive(false);
			musicOffBtn.SetActive(true);
//			关闭音乐 
			if (bgAudio.isPlaying) 
			{
				bgAudio.Pause ();
			}
		}
	}

	/// <summary>
	/// 点击音乐开按钮，关闭声音
	/// </summary>
	/// <param name="btn">Button.</param>
	void OnMusicOnBtnClick(GameObject btn)
	{
		isMusicOn = false;    
	}

	/// <summary>
	/// 点击音乐关按钮，开启声音
	/// </summary>
	/// <param name="btn">Button.</param>
	void  OnMusicOffBtnClick(GameObject btn)  
	{
		isMusicOn = true;

	}
	

}
