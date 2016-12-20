using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AnimationDemo;


public class PhotoRecognizingPanel : MonoBehaviour 
{

	public static PhotoRecognizingPanel _instance;

	private GameObject replayBtn;
	private UITexture photoImage;//拍摄截取的图像

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

		UIEventListener.Get(replayBtn).onClick = OnReplayBtnClick;
		Init();

	}

	void Init()
	{
		mask.alpha=0;
		photoImage.mainTexture = GetImage._instance.texture;
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


	void OnReplayBtnClick(GameObject btn)
	{
		GetImage._instance.isStartUpdate=true;
		PanelTranslate.Instance.GetPanel(Panels.PhotoTakingPanel );
		PanelTranslate.Instance.DestoryAllPanel();
	}

}



