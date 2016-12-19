﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class PhotoTakingPanel : SceneSinglton<PhotoTakingPanel> 
{
	private GameObject confirmBtn;
	private GameObject manager;

	private Transform notice;
	private Transform countDown;

	private UILabel countDownLabel;

	private Vector3 noticePos=new Vector3(0,63);
	private Vector3 countDownPos=new Vector3(0,-150);

	void Start()
	{
		Debug.Log("------phototaking --start()");
		confirmBtn = transform.Find ("ConfirmBtn").gameObject;
		manager=GameObject.Find("Manager");
		notice=transform.Find("Notice");
		countDown=transform.Find ("CountDown");
		countDownLabel = countDown.GetComponent<UILabel> ();

		UIEventListener.Get(confirmBtn).onClick = OnConfirmBtnClick;
		Debug.Log("------phototaking --start()--over");
	}

	void OnConfirmBtnClick(GameObject btn)
	{
		notice.localPosition=noticePos;
		StartCoroutine (CountDown());//图片出来后停留几秒，弹出倒计时数字
	
	}

	IEnumerator CountDown()
	{
		yield return new WaitForSeconds(1f);//1f for rest, real time is 3f..
		countDown.localPosition=countDownPos;

		yield return new WaitForSeconds(1);
		countDownLabel.text = "2";
		yield return new WaitForSeconds(1);
		countDownLabel.text = "1";
		yield return new WaitForSeconds(1);
	
		countDownLabel.text = " ";

		ChangeScene();
	}

	void ChangeScene()
	{
		Manager.Instance.texture=GetImage._instance.texture;
//		SceneManager.LoadSceneAsync("scene_PhotoRecognize");
		SceneManager.LoadScene("scene_PhotoRecognize");
		GameObject.DontDestroyOnLoad(manager);
	}
}