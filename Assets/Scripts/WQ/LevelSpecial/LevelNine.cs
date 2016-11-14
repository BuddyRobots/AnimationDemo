using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCircuit;

// level 9-----并联电路，2个电池+3个开关+2个灯泡+1个音响+1个电磁炉（2个开关分别控制2个灯泡，音响，电磁炉；电池不可点击）
public class LevelNine : MonoBehaviour 
{
	[HideInInspector]
	public bool isParallelCircuitWithTwoBulb = false;
	private List<GameObject> switchList = null; 
	private bool isBulbCircuitAniPlayed=false;
	private bool isBulbClicked=false;
	private GameObject clickBulb =null;
	private List<GameObject> bulbList = null;
	private bool isBulbAddComponent = false;
	/// <summary>
	/// 记录小手最多出现的次数，小于0时销毁小手
	/// </summary>
	private int singnal=2;
	/// <summary>
	/// 标记能被点击的灯泡是否是半透明，false是不透明，true为透明
	/// </summary>
	private bool preClickBulbSemiStatus=false;
	/// </summary>

	void OnEnable ()
	{
		isBulbClicked=false;
		isParallelCircuitWithTwoBulb = false;
		isBulbCircuitAniPlayed=false;
		isBulbAddComponent = false;
		singnal=2;
		preClickBulbSemiStatus=false;
		clickBulb = null;
	}

	//电流播放一会后，在灯泡的位置出现小手提示灯泡是可以点击的，（两个灯泡要确保有一个是在工作的，只能有一个能被点击变成半透明），
	//点击灯泡后，小手消失，灯泡变成半透明，另外一个灯泡变得更亮；再点击透明灯泡，灯泡复原成不透明，另外一个灯泡变正常亮
	void Update () 
	{
		if (isParallelCircuitWithTwoBulb) 
		{
			bulbList = PhotoRecognizingPanel._instance.bulbList;
			switchList = PhotoRecognizingPanel._instance.switchList;

			for (int i = 0; i < switchList.Count; i++) //点击开关，调用方法，circuitItems更新powered属性
			{
				GetImage._instance.cf.switchOnOff (int.Parse(switchList [i].tag), switchList [i].GetComponent<SwitchCtrl> ().isSwitchOn ? false : true);
				CommonFuncManager._instance.CircuitItemRefresh(GetImage._instance.itemList);//使用新的circuititems
			}



			CommonFuncManager._instance.ArrowsRefresh(GetImage._instance.itemList);



			if (!isBulbCircuitAniPlayed) 
			{
				isBulbCircuitAniPlayed = BulbCircuitPowerdOrNot ();
			}

			if (isBulbCircuitAniPlayed) //有灯泡的电路通了一次后灯泡才可以被点击
			{
				clickBulb = bulbList[1];//识别部分设定是ID为0的不能点击，为1的可以点击
		
				if (!isBulbAddComponent) //保证只给bulb加一次脚本
				{
					clickBulb.AddComponent<BoxCollider> ();
					clickBulb.GetComponent<BoxCollider>().center=new Vector3(0.4f,3.5f,0);
					clickBulb.GetComponent<BoxCollider>().size=new Vector3(72f,100f,0);
					
					clickBulb.AddComponent<UIButton> ();//给随机的灯泡添加button组件和BulbCtrl组件来实现点击事件
					clickBulb.AddComponent<BulbCtrl> ();
					isBulbAddComponent = true;
				}


				if (clickBulb.GetComponent<BulbCtrl> ().isSemiTrans) //1个灯泡变成半透明
				{ 
					clickBulb.GetComponent<UISprite>().spriteName="semiTransBulb";
					UISprite temp=bulbList[0].GetComponent<UISprite>();
					if (temp.spriteName=="bulbOn") 
					{
						temp.spriteName="bulbSpark";
					}
					clickBulb.GetComponent<UISprite> ().depth = 1;//透明灯泡在电线下面显示，不遮挡电线和箭头
					isBulbClicked = true; 
				} 
				//被点击为不透明
				if (isBulbClicked && !clickBulb.GetComponent<BulbCtrl> ().isSemiTrans) 
				{

					//如果灯泡有电，则应该是BulbOn，如果没电，则应该是BlubOFF  
					int index=int.Parse(clickBulb.tag);
					if (PhotoRecognizingPanel._instance.itemList[index].powered) 
					{
						clickBulb.GetComponent<UISprite>().spriteName="bulbOn";
						bulbList[0].GetComponent<UISprite>().spriteName="bulbOn";
					}
					else
					{
						clickBulb.GetComponent<UISprite>().spriteName="bulbOff";
					}
					bulbList [1].GetComponent<UISprite> ().depth = 4;
				}
			}




			//小手只出现两次的逻辑
			if (singnal <=0) 
			{
				if (PhotoRecognizingPanel._instance.finger) 
				{
					Destroy (PhotoRecognizingPanel._instance.finger);
				}

			} 
			else 
			{
				if (clickBulb) 
				{
					GetComponent<PhotoRecognizingPanel> ().ShowFinger (clickBulb.transform.localPosition);//show finger
					if (preClickBulbSemiStatus != clickBulb.GetComponent<BulbCtrl> ().isSemiTrans) 
					{
						singnal--;
						preClickBulbSemiStatus = clickBulb.GetComponent<BulbCtrl> ().isSemiTrans;
					}
				}
			}

			CommonFuncManager._instance.ArrowsRefresh(GetImage._instance.itemList);

		}
	}
	/// <summary>
	/// if the bulb circuit is powerd and show animation
	/// </summary>
	/// <returns><c>true</c>, if powerd or not was circuited, <c>false</c> otherwise.</returns>
	public bool BulbCircuitPowerdOrNot()
	{
		foreach (var item in GetImage._instance.itemList) 
		{
			if (item.powered && item.type==ItemType.Bulb)
			{
				return true;
			}
		}
		return false;
	}
}
