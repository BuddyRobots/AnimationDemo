using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AnimationDemo;
using UnityEngine.SceneManagement;
	
public class PhotoRecognizingPanel : SceneSinglton<PhotoRecognizingPanel> 
{
	private GameObject manager;
	private GameObject replayBtn;
	private UITexture photoImage;//拍摄截取的图像
	private bool isPhotoImageShowDone = false;


	void Start()
	{
		replayBtn=transform.Find("ReplayBtn").gameObject;
		manager=GameObject.Find("Manager(Clone)");
		photoImage =transform.Find ("Bg/PhotoImage").GetComponent<UITexture> ();//real code 
		photoImage.gameObject.SetActive (false);
		UIEventListener.Get(replayBtn).onClick = OnReplayBtnClick;
		isPhotoImageShowDone = false;
		StartCoroutine (ShowPhotoImage ());//进入识别界面的第一步是显示拍摄的照片

	}
		
		
	IEnumerator ShowPhotoImage()// 显示拍摄的图片
	{
		photoImage.gameObject.SetActive (true);
		photoImage.mainTexture = GetImage._instance.texture;
		yield return new WaitForSeconds (1f);
		isPhotoImageShowDone = true;
	}
		
	void OnReplayBtnClick(GameObject btn)
	{
		SceneManager.LoadSceneAsync("scene_PhotoTaking");
<<<<<<< HEAD
//		GameObject.DontDestroyOnLoad(manager);
=======
		GameObject.DontDestroyOnLoad(manager);
		Debug.Log("--------------LoadSceneAsync to scene_PhotoTaking");
>>>>>>> b6b9369dae7cd2796aa5c112079a3287efff5f99
	}
		
}



