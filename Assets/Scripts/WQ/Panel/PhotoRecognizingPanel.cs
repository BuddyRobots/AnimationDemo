using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AnimationDemo;
using UnityEngine.SceneManagement;
	
public class PhotoRecognizingPanel : SceneSinglton<PhotoRecognizingPanel> 
{


//	public static PhotoRecognizingPanel Instance;
	private GameObject manager;
	private GameObject replayBtn;
	private UITexture photoImage;//拍摄截取的图像
	private bool isPhotoImageShowDone = false;


	void Awake()
	{
//		Instance = this;
	}

	void Start()
	{
		replayBtn=transform.Find("ReplayBtn").gameObject;
		manager=GameObject.Find("Manager");
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
//		PanelTranslate.Instance.GetPanel(Panels.PhotoTakingPanel );
//		PanelTranslate.Instance.DestoryAllPanel();

		SceneManager.LoadSceneAsync("scene_PhotoTaking");
//		GameObject.DontDestroyOnLoad(manager);
	}
		
}



