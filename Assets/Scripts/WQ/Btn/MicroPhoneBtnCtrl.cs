using UnityEngine;
using System.Collections;

public class MicroPhoneBtnCtrl : MonoBehaviour 
{
	[HideInInspector]
	public bool isCollectVoice = false;

	void OnEnable () 
	{
		isCollectVoice = false;
		//13 ,14关的小话筒按钮必须等其他开关闭合后才能点击有效
		if (LevelManager.currentLevelData.LevelID==13 || LevelManager.currentLevelData.LevelID==14) 
		{
			transform.GetComponent<BoxCollider> ().enabled = false;
		}

	}
		
	void OnClick()
	{
		isCollectVoice = true;
	}
}
