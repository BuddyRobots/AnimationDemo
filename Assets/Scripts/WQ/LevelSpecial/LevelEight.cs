using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCircuit;

//level 8-----并联电路，2个电池+3个开关+1个灯泡+1个音响+1个电磁炉（3个开关分别控制灯泡，音响，电磁炉；电池不可点击）
public class LevelEight: MonoBehaviour 
{
	[HideInInspector]
	public bool isLevelEight = false;
	private List<GameObject> switchList = null; 

	void OnEnable () 
	{
		isLevelEight = false;
	}
	

	void Update () 
	{
		if (isLevelEight) 
		{
			switchList = PhotoRecognizingPanel._instance.switchList;	
//			for (int i = 0; i < switchList.Count; i++) //点击开关，调用方法，circuitItems更新powered属性
//			{
//				GetImage._instance.cf.switchOnOff (int.Parse(switchList [i].tag), switchList [i].GetComponent<SwitchCtrl> ().isSwitchOn ? false : true);
//				CommonFuncManager._instance.CircuitItemRefreshWithTwoBattery (GetImage._instance.itemList);//使用新的circuititems
//
//			}
			CommonFuncManager._instance.ArrowsRefresh(GetImage._instance.itemList);
		}
	
	}
}
