using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PhotoTakingPanel : MonoBehaviour 
{
	public static PhotoTakingPanel _instance;
	private GameObject confirmBtn;
	private UISprite noticeImg;
	private UILabel countDown;


	void Awake () 
	{	
		_instance = this;

	}

	void Start()
	{
		confirmBtn = transform.Find ("ConfirmBtn").gameObject;
		noticeImg=transform.Find("Notice").GetComponent<UISprite>();
		countDown = transform.Find ("CountDown").GetComponent<UILabel> ();
		noticeImg.gameObject.SetActive (false);
		countDown.gameObject.SetActive (false);

		UIEventListener.Get(confirmBtn).onClick = OnConfirmBtnClick;
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

		yield return new WaitForSeconds(1);
		countDown.text = "2";
		yield return new WaitForSeconds(1);
		countDown.text = "1";
		yield return new WaitForSeconds(1);


		PanelTranslate.Instance.GetPanel(Panels.PhotoRecognizedPanel , false);//识别界面需要从 拍摄界面Quad上的GetImage获取itemlist数据，所以这里暂时不能销毁拍摄界面
		PanelOff();
		//		PanelTranslate.Instance.DestoryThisPanel();
	}

	public void PanelOff()
	{
		countDown.text = "3";
		countDown.gameObject.SetActive (false);
		noticeImg.gameObject.SetActive (false);

		GetImage._instance.isStartUpdate=false;
		PanelTranslate.Instance.DestoryThisPanel();
	}
}