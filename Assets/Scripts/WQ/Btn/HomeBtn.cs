using UnityEngine;
using System.Collections;


public class HomeBtn : SceneSinglton<HomeBtn>
{
	private GameObject startPanel;
//	public VoidDelegate panelOff;

	void Awake () 
	{
		UIEventListener.Get (gameObject).onClick += onClick1;
	}

	void onClick1(GameObject go)
	{
		//跳转到选关界面
		PanelTranslate.Instance.GetPanel(Panels.LevelSelectedPanel);
		PanelTranslate.Instance.DestoryAllPanel();
	}
}
	

