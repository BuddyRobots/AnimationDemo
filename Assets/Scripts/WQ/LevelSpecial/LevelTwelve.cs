using UnityEngine;
using System.Collections;
using MagicCircuit;

//第12关----光敏开关
public class LevelTwelve : MonoBehaviour 
{
	[HideInInspector]
	public bool isLAswitchOccur = false;


	private float changeTime = 3f;//渐变的总时间
	private  float changeTimer = 0;
//	private bool isCircuitWork = false;
	private bool isFingerShow = false;
	private bool isFingerDestroyed=false;
	private bool CurrLASwitchStatus=false;
	private bool PreLASwitchStatus=false;

	private Transform LAswitch;
	private UITexture nightBg=null;

	void OnEnable () 
	{
		changeTime = 3f;
		changeTimer = 0;
		isLAswitchOccur = false;
//		isCircuitWork = false;

		isFingerShow = false;
		isFingerDestroyed=false;



		LAswitch = transform.Find ("lightActSwitch");
		nightBg = PhotoRecognizingPanel._instance.nightMask;

	}



	void Update () 
	{
		if (isLAswitchOccur)
		{
//			Transform LAswitch = transform.Find ("lightActSwitch");
//			nightBg = PhotoRecognizingPanel._instance.nightMask;
			if (!isFingerShow) 
			{
				//在太阳月亮按钮位置出现小手，点击太阳，蒙版渐变暗，小手消失，光敏开关闭合，灯泡亮，电流走起
				GetComponent<PhotoRecognizingPanel> ().ShowFinger(transform.Find("SunAndMoonWidget").localPosition);
				isFingerShow = true;
			}
			if(!transform.Find("SunAndMoonWidget").GetComponent<MoonAndSunCtrl>().isDaytime)//如果是晚上
			{
				if (!isFingerDestroyed) 
				{
					Destroy (PhotoRecognizingPanel._instance.finger);	
					isFingerDestroyed = true;
				}
				changeTimer += Time.deltaTime;
				if (changeTimer >= changeTime) 
				{
					changeTimer = changeTime;
				}
				nightBg.alpha = Mathf.Lerp (0, 1f, changeTimer / changeTime);//蒙版渐变暗
				if(changeTimer>=changeTime*5/6)//背景渐变快完成时
				{
//					isCircuitWork = true;
					CurrLASwitchStatus=true;
					if (PreLASwitchStatus!=CurrLASwitchStatus) 
					{
						GetImage._instance.cf.switchOnOff (int.Parse (LAswitch.gameObject.tag), true);
						LAswitch.GetComponent<UISprite>().spriteName= "LAswitchOn";
						CommonFuncManager._instance.CircuitItemRefresh (GetImage._instance.itemList);
						PreLASwitchStatus=CurrLASwitchStatus;
					}
				}
			}
			else //如果是白天
			{
				changeTimer -= Time.deltaTime;
				if (changeTimer <= 0) 
				{
					changeTimer =0;
				}
				nightBg.alpha = Mathf.Lerp (0, 1f, changeTimer / changeTime);
				if (changeTimer <= changeTime / 6) 
				{
//					isCircuitWork = false;
					CurrLASwitchStatus = false;
					if (PreLASwitchStatus!=CurrLASwitchStatus) 
					{
						GetImage._instance.cf.switchOnOff (int.Parse (LAswitch.gameObject.tag), false);
						LAswitch.GetComponent<UISprite>().spriteName= "LAswitchOff";
						CommonFuncManager._instance.CircuitItemRefresh (GetImage._instance.itemList);
						PreLASwitchStatus=CurrLASwitchStatus;
					}
				}
			}	
			CommonFuncManager._instance.ArrowsRefresh(GetImage._instance.itemList);
		}
	}

}
