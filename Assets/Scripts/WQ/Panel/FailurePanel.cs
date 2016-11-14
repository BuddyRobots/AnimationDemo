using UnityEngine;
using System.Collections;

public class FailurePanel : MonoBehaviour {


	//有一个提示失败的图片
	//private UISprite failNotice;
	private string levelName;//记录当前的关卡名

	private GameObject replayBtn;
	private GameObject photoTakingPanel;
	void Start () 
	{
		
		replayBtn = transform.Find ("ReplayBtn").GetComponent<UIButton> ().gameObject;
		photoTakingPanel = transform.parent.parent.Find ("PhotoTakingPanel").gameObject;
		UIEventListener.Get (replayBtn).onClick = OnReplayBtnClick;

	}
	void OnEnable()
	{
		levelName= LevelManager.currentLevelData.LevelName;

	}

	void OnReplayBtnClick(GameObject btn)
	{
		//返回拍摄界面
		photoTakingPanel.SetActive (true);
		PanelOff();
	}

	public void PanelOff()
	{
		this.gameObject.SetActive(false);
		transform.parent.gameObject.SetActive (false);
	}
		
}
