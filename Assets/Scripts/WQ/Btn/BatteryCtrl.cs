using UnityEngine;
using System.Collections;

public class BatteryCtrl : MonoBehaviour 
{

	[HideInInspector]
	public bool isSemiTrans = false;
	private UISprite batterySprite;


	void Start()
	{

		batterySprite = GetComponent<UISprite> ();
	}


	void Update () 
	{
		if (isSemiTrans && batterySprite.spriteName !="semiTransBattery") //如果是半透明状态
		{
			batterySprite.spriteName="semiTransBattery";
		
		} 
		else if(!isSemiTrans && batterySprite.spriteName!="battery")//如果是正常状态 
		{
			batterySprite.spriteName="battery";
		
		}
	}
		
	void OnClick()
	{
		isSemiTrans = !isSemiTrans;
	}
}
