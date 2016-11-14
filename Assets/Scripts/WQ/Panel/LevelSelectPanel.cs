using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class LevelSelectPanel : MonoBehaviour {

	//用来控制关卡列表，包含很多个LevelItemUI---用列表来存储
	public static LevelSelectPanel _instance;

	private GameObject levelDescriptionPanel;
	private GameObject helpBtn;


	public List<UIButton> uiBtnListT = new List<UIButton>();
	public List<UIButton> uiBtnListL = new List<UIButton>();

	// twinkle button
	private UIButton btnT01;
	private UIButton btnT02;
	private UIButton btnT03;
	private UIButton btnT04;
	private UIButton btnT05;
	private UIButton btnT06;
	private UIButton btnT07;
	private UIButton btnT08;
	private UIButton btnT09;
	private UIButton btnT10;
	private UIButton btnT11;
	private UIButton btnT12;
	private UIButton btnT13;
	private UIButton btnT14;
	private UIButton btnT15;

	// light button
	private UIButton btnL01;
	private UIButton btnL02;
	private UIButton btnL03;
	private UIButton btnL04;
	private UIButton btnL05;
	private UIButton btnL06;
	private UIButton btnL07;
	private UIButton btnL08;
	private UIButton btnL09;
	private UIButton btnL10;
	private UIButton btnL11;
	private UIButton btnL12;
	private UIButton btnL13;
	private UIButton btnL14;
	private UIButton btnL15;


	void Awake()
	{
		_instance = this;

		btnT01 = transform.Find ("Bg/LevelD01/LevelT01").GetComponent<UIButton> ();
		btnL01 = transform.Find ("Bg/LevelD01/LevelL01").GetComponent<UIButton> ();

		btnT02 = transform.Find ("Bg/LevelD02/LevelT02").GetComponent<UIButton> ();
		btnL02 = transform.Find ("Bg/LevelD02/LevelL02").GetComponent<UIButton> ();

		btnT03 = transform.Find ("Bg/LevelD03/LevelT03").GetComponent<UIButton> ();
		btnL03 = transform.Find ("Bg/LevelD03/LevelL03").GetComponent<UIButton> ();

		btnT04 = transform.Find ("Bg/LevelD04/LevelT04").GetComponent<UIButton> ();
		btnL04 = transform.Find ("Bg/LevelD04/LevelL04").GetComponent<UIButton> ();

		btnT05 = transform.Find ("Bg/LevelD05/LevelT05").GetComponent<UIButton> ();
		btnL05 = transform.Find ("Bg/LevelD05/LevelL05").GetComponent<UIButton> ();

		btnT06 = transform.Find ("Bg/LevelD06/LevelT06").GetComponent<UIButton> ();
		btnL06 = transform.Find ("Bg/LevelD06/LevelL06").GetComponent<UIButton> ();

		btnT07 = transform.Find ("Bg/LevelD07/LevelT07").GetComponent<UIButton> ();
		btnL07 = transform.Find ("Bg/LevelD07/LevelL07").GetComponent<UIButton> ();

		btnT08 = transform.Find ("Bg/LevelD08/LevelT08").GetComponent<UIButton> ();
		btnL08 = transform.Find ("Bg/LevelD08/LevelL08").GetComponent<UIButton> ();

		btnT09 = transform.Find ("Bg/LevelD09/LevelT09").GetComponent<UIButton> ();
		btnL09 = transform.Find ("Bg/LevelD09/LevelL09").GetComponent<UIButton> ();

		btnT10 = transform.Find ("Bg/LevelD10/LevelT10").GetComponent<UIButton> ();
		btnL10 = transform.Find ("Bg/LevelD10/LevelL10").GetComponent<UIButton> ();

		btnT11 = transform.Find ("Bg/LevelD11/LevelT11").GetComponent<UIButton> ();
		btnL11 = transform.Find ("Bg/LevelD11/LevelL11").GetComponent<UIButton> ();

		btnT12 = transform.Find ("Bg/LevelD12/LevelT12").GetComponent<UIButton> ();
		btnL12 = transform.Find ("Bg/LevelD12/LevelL12").GetComponent<UIButton> ();

		btnT13 = transform.Find ("Bg/LevelD13/LevelT13").GetComponent<UIButton> ();
		btnL13 = transform.Find ("Bg/LevelD13/LevelL13").GetComponent<UIButton> ();

		btnT14 = transform.Find ("Bg/LevelD14/LevelT14").GetComponent<UIButton> ();
		btnL14 = transform.Find ("Bg/LevelD14/LevelL14").GetComponent<UIButton> ();

		btnT15 = transform.Find ("Bg/LevelD15/LevelT15").GetComponent<UIButton> ();
		btnL15 = transform.Find ("Bg/LevelD15/LevelL15").GetComponent<UIButton> ();

		uiBtnListT.Add (btnT01);
		uiBtnListT.Add (btnT02);
		uiBtnListT.Add (btnT03);
		uiBtnListT.Add (btnT04);
		uiBtnListT.Add (btnT05);
		uiBtnListT.Add (btnT06);
		uiBtnListT.Add (btnT07);
		uiBtnListT.Add (btnT08);
		uiBtnListT.Add (btnT09);
		uiBtnListT.Add (btnT10);
		uiBtnListT.Add (btnT11);
		uiBtnListT.Add (btnT12);
		uiBtnListT.Add (btnT13);
		uiBtnListT.Add (btnT14);
		uiBtnListT.Add (btnT15);

		uiBtnListL.Add (btnL01);
		uiBtnListL.Add (btnL02);
		uiBtnListL.Add (btnL03);
		uiBtnListL.Add (btnL04);
		uiBtnListL.Add (btnL05);
		uiBtnListL.Add (btnL06);
		uiBtnListL.Add (btnL07);
		uiBtnListL.Add (btnL08);
		uiBtnListL.Add (btnL09);
		uiBtnListL.Add (btnL10);
		uiBtnListL.Add (btnL11);
		uiBtnListL.Add (btnL12);
		uiBtnListL.Add (btnL13);
		uiBtnListL.Add (btnL14);
		uiBtnListL.Add (btnL15);
	}

	void OnEnable()
	{
		helpBtn=transform.Find("HelpBtn").gameObject;
		UIEventListener.Get(helpBtn).onClick = OnHelpBtnClick;
		RefreshLevelUI ();//界面刷新
//		HomeBtn.Instance.panelOff = PanelOff;
	}

	//refresh UI
	public void RefreshLevelUI()
	{
		for (int i = 0; i < LevelManager._instance.levelItemDataList.Count; ++i)
		{
			
			LevelItemData data = LevelManager._instance.levelItemDataList [i];
			UIButton btnT = uiBtnListT [i];
			UIButton btnL = uiBtnListL [i];
			int levelId =btnT.GetComponent<LevelItemUI>().GetLevel(btnT.name);
			if(data.LevelID==levelId)
			{
				switch(data.Progress)
				{
					case LevelProgress.Todo:						
						btnT.gameObject.SetActive (false);
						btnL.gameObject.SetActive (false);
						break;
					case LevelProgress.Doing:
						btnT.gameObject.SetActive (true);
						btnL.gameObject.SetActive (false);
						break;
					case LevelProgress.Done:	
						btnT.gameObject.SetActive (false);
						btnL.gameObject.SetActive (true);
						break;
					default:
						break;
				}
			}
		}
	}

//	public void PanelOff()
//	{
//		gameObject.SetActive (false);
//	}


	void OnHelpBtnClick(GameObject btn)
	{
//		//code for unlock all level items test
//		PlayerPrefs.SetInt ("LevelID",15);
//		PlayerPrefs.SetInt ("LevelProgress",2);
//		LevelManager._instance.LoadLocalLevelProgressData();
//		RefreshLevelUI();
//


//		real code
		PlayerPrefs.SetInt("toDemoPanelFromPanel",2);
		PanelTranslate.Instance.GetPanel(Panels.DemoShowPanel);
		PanelTranslate.Instance.DestoryAllPanel();
		GameObject.Find("UI Root/DemoShowPanel(Clone)/DemoPic").GetComponent<HelpDataShow>().InitFromStart();
	}
}