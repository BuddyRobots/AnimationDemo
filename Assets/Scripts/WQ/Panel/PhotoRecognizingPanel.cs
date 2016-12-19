using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AnimationDemo;


public class PhotoRecognizingPanel : MonoBehaviour 
{


	public static PhotoRecognizingPanel _instance;

	private GameObject replayBtn;
	private UITexture photoImage;//拍摄截取的图像
	private bool isPhotoImageShowDone = false;

	private UISprite mask;
	private float maskTime=0;
	private  float maskTimer=2f;

	private bool isMaskTrans=true;

	void Awake()
	{
		_instance = this;
	}

	void OnEnable()
	{
		replayBtn=transform.Find("ReplayBtn").gameObject;
		photoImage =transform.Find ("Bg/PhotoImage").GetComponent<UITexture> ();//real code 
		mask=transform.Find("Bg/Mask").GetComponent<UISprite>();
		mask.alpha=0;



		photoImage.gameObject.SetActive (false);
		UIEventListener.Get(replayBtn).onClick = OnReplayBtnClick;

		isPhotoImageShowDone = false;


		photoImage.mainTexture = GetImage._instance.texture;




//		StartCoroutine (ShowPhotoImage ());//进入识别界面的第一步是显示拍摄的照片

	}


	void Update()
	{
		if (isMaskTrans)
		{
			maskTime+=Time.deltaTime;
			if (maskTime>=maskTimer) 
			{
				isMaskTrans=false;
				maskTime=maskTimer;
			}
			mask.alpha=Mathf.Lerp (0, 1f,maskTime/maskTimer);
		}
	}

//	IEnumerator ShowPhotoImage()// 显示拍摄的图片
//	{
//		photoImage.gameObject.SetActive (true);
//		photoImage.mainTexture = GetImage._instance.texture;
//		yield return new WaitForSeconds (1f);
//		isPhotoImageShowDone = true;
//	}



	void OnReplayBtnClick(GameObject btn)
	{
		GetImage._instance.isStartUpdate=true;
		PanelTranslate.Instance.GetPanel(Panels.PhotoTakingPanel );
		PanelTranslate.Instance.DestoryAllPanel();
	}

}



