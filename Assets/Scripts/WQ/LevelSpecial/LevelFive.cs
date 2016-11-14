using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCircuit;


//level 5
public class LevelFive : MonoBehaviour 
{
	private List<GameObject> normalSwitchList =null;
//	private int animationPlayedTimes=0;

	[HideInInspector]
	public bool isTwoSwitchInSeriesCircuit = false;

	/// <summary>
	/// 电路是否接通的标志
	/// </summary>
//	private bool isCircuitOpen = false;

	void OnEnable () 
	{
		//animationPlayedTimes=0;
		isTwoSwitchInSeriesCircuit = false;
		//isCircuitOpen = false;
	}


	void Update () 
	{
		
		if (isTwoSwitchInSeriesCircuit) 
		{
//			normalSwitchList = GetComponent<PhotoRecognizingPanel> ().switchList;
//
//
//			for (int i = 0; i < normalSwitchList.Count; i++) 
//			{
//				GetImage._instance.cf.switchOnOff (int.Parse(normalSwitchList [i].tag), normalSwitchList [i].GetComponent<SwitchCtrl> ().isSwitchOn ? false : true);
//			
//				CommonFuncManager._instance.CircuitItemRefresh (GetImage._instance.itemList);
//			}


			CommonFuncManager._instance.ArrowsRefresh(GetImage._instance.itemList);

		}
	
	}
}
