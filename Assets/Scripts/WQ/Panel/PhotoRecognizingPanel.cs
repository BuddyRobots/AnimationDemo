using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCircuit;

	
public class PhotoRecognizingPanel : MonoBehaviour 
{


	public static PhotoRecognizingPanel _instance;

	private GameObject replayBtn;
	private UITexture photoImage;//拍摄截取的图像
	private bool isPhotoImageShowDone = false;


	void Awake()
	{
		_instance = this;
	}


		
	void OnEnable()
	{
		replayBtn=transform.Find("ReplayBtn").gameObject;
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

	void Update () 
	{
		
	}
		
	void OnReplayBtnClick(GameObject btn)
	{
		PanelTranslate.Instance.GetPanel(Panels.PhotoTakingPanel );
		PanelTranslate.Instance.DestoryAllPanel();
	}
		
}



