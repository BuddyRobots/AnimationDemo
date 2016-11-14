using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCircuit;

//level 7-----并联电路，2个电池+2个开关+1个灯泡+1个音响（两个开关分别控制灯泡和音响），电路通电流后有一个电池可以被点击（在透明和半透明之间切换）
public class LevelSeven : MonoBehaviour 
{
	[HideInInspector]
	public bool isParallelCircuitWithTwoBattery=false;
	private bool isCircuitAnimationPlayed=false;
	private bool isBatteryClick=false;
	private List<GameObject> batteryList = null;
//	private List<GameObject> switchList = null;
	private GameObject clickBattery =null;
	private bool isBatteryAddComponent = false;
	/// <summary>
	/// 标记小手最多出现的次数，小于0时销毁
	/// </summary>
	private int singnal = 2;
	private bool preClickBatterySemiStatus = false;

	void OnEnable () 
	{
		isBatteryClick=false;
		isParallelCircuitWithTwoBattery=false;
		isCircuitAnimationPlayed=false;
		singnal = 2;
		isBatteryAddComponent = false;
		preClickBatterySemiStatus = false;
		clickBattery =null;
	}

	void Update () 
	{
		if (isParallelCircuitWithTwoBattery) 
		{
			batteryList = PhotoRecognizingPanel._instance.batteryList;
//			switchList = PhotoRecognizingPanel._instance.switchList;
//			for (int i = 0; i < switchList.Count; i++) 
//			{
//				GetImage._instance.cf.switchOnOff (int.Parse(switchList [i].tag), switchList [i].GetComponent<SwitchCtrl> ().isSwitchOn ? false : true);
//				CommonFuncManager._instance.CircuitItemRefreshWithTwoBattery (GetImage._instance.itemList);//使用新的circuititems
//			}


			CommonFuncManager._instance.ArrowsRefresh(GetImage._instance.itemList);



			isCircuitAnimationPlayed=CircuitPowerdOrNot();


			if (isCircuitAnimationPlayed) //电池可以被点击
			{
				clickBattery = batteryList[1];//识别部分设定是ID为0的不能点击，为1的可以点击
				if (!isBatteryAddComponent)//保证只给battery加一次脚本
				{
					clickBattery.AddComponent<BoxCollider> ();
					clickBattery.GetComponent<BoxCollider>().size= new Vector3(114f,74f,0); 
					clickBattery.GetComponent<BoxCollider>().center= new Vector3(0,-2.6f,0);


					clickBattery.AddComponent<UIButton> ();//给随机的电池添加button组件和BatteryCtrl组件来实现点击事件
					clickBattery.AddComponent<BatteryCtrl> ();
					isBatteryAddComponent = true;
				}
				if (clickBattery.GetComponent<BatteryCtrl> ().isSemiTrans) //点击了电池，电池变成半透明，1个电池工作
				{ 
					foreach (var item in GetImage._instance.itemList) 
					{
						switch (item.type) 
						{
						case ItemType.Bulb:
							transform.Find("bulb").GetComponent<UISprite>().spriteName=item.powered ? "bulbOn":"bulbOff";
							break;
						case ItemType.Loudspeaker:
							AudioSource audio = transform.Find ("loudspeaker").GetComponent<AudioSource> ();
							if (item.powered) 
							{
								if (!audio .isPlaying)
								{
									audio.Play ();

								}
								audio.volume = 0.5f;
							} 
							else 
							{
								if (audio .isPlaying)
								{
									audio.Stop ();
								}
							}
							break;
						default:
							break;
						}
					}
					clickBattery.GetComponent<UISprite>().depth=1;
					isBatteryClick=true;
				} 
				if (isBatteryClick && !clickBattery.GetComponent<BatteryCtrl> ().isSemiTrans) //电池回归到正常
				{
					CommonFuncManager._instance.CircuitItemRefreshWithTwoBattery (GetImage._instance.itemList);
					batteryList [1].GetComponent<UISprite> ().depth = 4;
				}
			}

			if (singnal <=0) 
			{
				if (PhotoRecognizingPanel._instance.finger) 
				{
					Destroy (PhotoRecognizingPanel._instance.finger);
				}
			} 
			else 
			{
				if (clickBattery) 
				{
					GetComponent<PhotoRecognizingPanel> ().ShowFinger (clickBattery.transform.localPosition);//show finger
					if (preClickBatterySemiStatus != clickBattery.GetComponent<BatteryCtrl> ().isSemiTrans) 
					{
						singnal--;
						preClickBatterySemiStatus = clickBattery.GetComponent<BatteryCtrl> ().isSemiTrans;
					}
				}
			}
		}
	}
		
	/// <summary>
	/// if the circuit is powerd and show animation
	/// </summary>
	/// <returns><c>true</c>, if powerd or not was circuited, <c>false</c> otherwise.</returns>
	public bool CircuitPowerdOrNot()
	{

		foreach (var item in GetImage._instance.itemList) 
		{
			if (item.powered && item.type==ItemType.Switch)
			{
				return true;
				//break;
			}

		}
		return false;
	}

}
