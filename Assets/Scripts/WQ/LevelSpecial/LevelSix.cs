using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCircuit;

//level 6-----并联电路，1个电池+2个开关+1个灯泡+1个音响（两个开关分别控制灯泡和音响）
public class LevelSix : MonoBehaviour 
{
	[HideInInspector]
	public bool isParrallelCircuit = false;
	private List<GameObject> switchList = null; 

	void OnEnable ()
	{
		isParrallelCircuit = false;
	}
	

	void Update () 
	{
		if (isParrallelCircuit) 
		{
//			switchList = PhotoRecognizingPanel._instance.switchList;	
//			for (int i = 0; i < switchList.Count; i++) //点击开关，调用方法，circuitItems更新powered属性
//			{
//				GetImage._instance.cf.switchOnOff (int.Parse(switchList [i].tag), switchList [i].GetComponent<SwitchCtrl> ().isSwitchOn ? false : true);
//
//				CommonFuncManager._instance.CircuitItemRefresh (GetImage._instance.itemList);//使用新的circuititems
//
//			}


			CommonFuncManager._instance.ArrowsRefresh(GetImage._instance.itemList);
		}
	}
}






