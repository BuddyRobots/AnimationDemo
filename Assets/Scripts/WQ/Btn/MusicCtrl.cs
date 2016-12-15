using UnityEngine;
using System.Collections;

public class MusicCtrl : MonoBehaviour {

	private GameObject musicOnBtn;
	private GameObject musicOffBtn;


	void Start () 
	{
		musicOnBtn = transform.Find ("MusicOnBtn").gameObject;
		musicOffBtn = transform.Find ("MusicOffBtn").gameObject;


		UIEventListener.Get(musicOnBtn).onClick = OnMusicOnBtnClick;
		UIEventListener.Get(musicOffBtn).onClick = OnMusicOffBtnClick;
	}


	void Update () 
	{
		if (Manager.isMusicOn && musicOffBtn.activeSelf)
		{
			//如果状态是开，而当前下面的“音乐关” 按钮开着的话，关闭它，并且打开 “音乐开” 的按钮
			musicOnBtn.SetActive(true);
			musicOffBtn.SetActive(false);

		}
		if (!Manager.isMusicOn && musicOnBtn.activeSelf)
		{
			//如果状态是关，而当前下面的 “音乐开” 按钮开着的话，关闭它，并且打开 “音乐关” 的按钮
			musicOnBtn.SetActive(false);
			musicOffBtn.SetActive(true);

		}
	}

	/// <summary>
	/// 点击音乐开按钮，关闭声音
	/// </summary>
	/// <param name="btn">Button.</param>
	void OnMusicOnBtnClick(GameObject btn)
	{

		Manager.isMusicOn = false;  
		Debug.Log("Manager.isMusicOn---"+Manager.isMusicOn);
	}

	/// <summary>
	/// 点击音乐关按钮，开启声音
	/// </summary>
	/// <param name="btn">Button.</param>
	void  OnMusicOffBtnClick(GameObject btn)  
	{

		Manager.isMusicOn = true;
		Debug.Log("Manager.isMusicOn---"+Manager.isMusicOn);

	}


}
