using UnityEngine;
using System.Collections;

public class StartPanel : MonoBehaviour 
{
	
	public static StartPanel _instance;

	private GameObject nextBtn;
	private GameObject helpBtn;
	private bool isMusicOn = true;

	void Awake()
	{
		_instance = this;
	}

	void Start () 
	{		
		nextBtn= transform.Find ("NextBtn").gameObject;
		helpBtn=transform.Find("HelpBtn").gameObject;
		UIEventListener.Get(nextBtn).onClick =OnNextBtnClick;
		UIEventListener.Get(helpBtn).onClick =OnHelpBtnClick;
	}

	/// <summary>
	/// enter into the next panel
	/// </summary>
	/// <param name="btn">参数是点击的按钮对象</param>
	void OnNextBtnClick(GameObject btn)
	{
		if (PlayerPrefs.HasKey("isEnterGameFirstTime") && PlayerPrefs.GetInt("isEnterGameFirstTime")==1) //不是第一次进入游戏
		{
			Debug.Log("-------is not the first time lauching game-------");

			//for test 
//
//			PlayerPrefs.SetInt("toDemoPanelFromPanel",1);
//			PanelTranslate.Instance.GetPanel(Panels.DemoShowPanel);
//			PanelTranslate.Instance.DestoryAllPanel();


			//real code 
			PanelTranslate.Instance.GetPanel(Panels.LevelSelectedPanel);
			PanelTranslate.Instance.DestoryThisPanel();
		}
		else//是第一次进入游戏
		{

			Debug.Log("*****is the first time lauching game****");

			PlayerPrefs.SetInt("isEnterGameFirstTime",1);

			PlayerPrefs.SetInt("toDemoPanelFromPanel",1);

			PlayerPrefs.SetInt("toDemoPanelFromBtn",1);

			PanelTranslate.Instance.GetPanel(Panels.DemoShowPanel);
			PanelTranslate.Instance.DestoryAllPanel();

		}
	}

	void OnHelpBtnClick(GameObject btn)
	{
		PlayerPrefs.SetInt("toDemoPanelFromPanel",1);//标记是从哪个界面进入帮助界面的
		PlayerPrefs.SetInt("toDemoPanelFromBtn",2);
		PanelTranslate.Instance.GetPanel(Panels.DemoShowPanel);
		//transform.parent.Find("DemoShowPanel/DemoPic").GetComponent<HelpDataShow>().InitFromStart();
		GameObject.Find("UI Root/DemoShowPanel(Clone)/DemoPic").GetComponent<HelpDataShow>().InitFromStart();
		PanelTranslate.Instance.DestoryAllPanel();

	}
}
