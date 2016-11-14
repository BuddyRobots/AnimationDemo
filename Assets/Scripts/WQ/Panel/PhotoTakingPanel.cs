using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PhotoTakingPanel : MonoBehaviour 
{
	public static PhotoTakingPanel _instance;

	private GameObject helpBtn;
	private GameObject confirmBtn;

	private UISprite noticeImg;

	private UILabel countDown;
	private UILabel levelLabel;

	void Awake () 
	{	
		_instance = this;

		helpBtn = transform.Find ("HelpBtn").gameObject;
		confirmBtn = transform.Find ("ConfirmBtn").gameObject;
		noticeImg=transform.Find("Notice").GetComponent<UISprite>();
		countDown = transform.Find ("CountDown").GetComponent<UILabel> ();
		levelLabel = transform.Find ("LevelLabel").GetComponent<UILabel> ();

		UIEventListener.Get(helpBtn).onClick = OnHelpBtnClick;
		UIEventListener.Get(confirmBtn).onClick = OnConfirmBtnClick;
	}

	void Start()
	{
		noticeImg.gameObject.SetActive (false);
		countDown.gameObject.SetActive (false);
	}

	void OnEnable()
	{
		levelLabel.text = LevelManager.currentLevelData.LevelName+"拍摄中";
//		HomeBtn.Instance.panelOff = PanelOff;   
	}

	#region 计算按钮的点击事件

	void OnHelpBtnClick(GameObject btn)
	{
		PlayerPrefs.SetInt("toDemoPanelFromPanel",3);
		PanelTranslate.Instance.GetPanel(Panels.DemoShowPanel);

		GameObject.Find("UI Root/DemoShowPanel(Clone)/DemoPic").GetComponent<HelpDataShow>().InitFromLevel();
		PanelTranslate.Instance.DestoryAllPanel();
	}

	void OnConfirmBtnClick(GameObject btn)
	{
		if (!noticeImg.gameObject.activeSelf) 
		{
			noticeImg.gameObject.SetActive (true);
			StartCoroutine (CountDown());//图片出来后停留几秒，弹出倒计时数字
		}
	}


	IEnumerator CountDown()
	{
		yield return new WaitForSeconds(1f);//1f for rest, real time is 3f..

		countDown.gameObject.SetActive(true);


		GetImage._instance.isTakingPhoto = true;


		//倒计时，每个数字停留一秒后变化
		yield return new WaitForSeconds(1);
		countDown.text = "2";


		GetImage._instance.isTakingPhoto = false;

		GetImage._instance.Thread_Process_Start();
		//GetImage._instance.test_saveFullQuadPhotoToiPad();



		yield return new WaitForSeconds(1);
		countDown.text = "1";

		yield return new WaitForSeconds(1);



		PanelTranslate.Instance.GetPanel(Panels.PhotoRecognizedPanel , false);//识别界面需要从 拍摄界面Quad上的GetImage获取itemlist数据，所以这里暂时不能销毁拍摄界面
		PanelOff();
	}
	#endregion

	public void PanelOff()
	{
		countDown.text = "3";
		countDown.gameObject.SetActive (false);
		noticeImg.gameObject.SetActive (false);

		GetImage._instance.isStartUpdate=false;
		PanelTranslate.Instance.DestoryThisPanel();
	}
}