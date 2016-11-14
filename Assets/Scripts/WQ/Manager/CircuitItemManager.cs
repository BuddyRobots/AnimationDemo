using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCircuit;
public class CircuitItemManager :MonoBehaviour
{
	//该脚本应该挂载在识别界面，因为图标是在拍摄界面后被识别的

	//坐标管理类

	public static CircuitItemManager _instance;

	//private CircuitItem item;

	public List<CircuitItem> itemList=new List<CircuitItem>();//所有图标的集合
	public List<CircuitItem> itemListTest=new List<CircuitItem>();//for test...

	void Awake()
	{
		_instance = this;

	}

	void Start () 
	{
		initCircuitItemListData();//这里应该是调用图像识别返回的 List<CircuitItem> list
		GetCircuitVec (itemListTest);
	}

	//初始化电路测试数据-----for test....
	void initCircuitItemListData()
	{
		CircuitItem item1 = new CircuitItem();
		item1.ID = 0;
		item1.name = "Battery";
		item1.type = ItemType.Battery;
		item1.theta = 30;
		//Vector3 vec1 = new Vector3 (0.5f,-0.1f,0);
		Vector3 vec1 = new Vector3 (188,-93,0);
		List<Vector3> vecs1 = new List<Vector3> ();
		vecs1.Add (vec1);
		item1.list=vecs1;
		item1.showOrder = 0;

		CircuitItem item7 = new CircuitItem();
		item7.ID = 1;
		item7.name = "Switch";
		item7.type = ItemType.Switch;
		item7.theta = 90;
		//Vector3 vec3 = new Vector3 (0.1f,0.4f,0);
		Vector3 vec7 = new Vector3 (0,-75,0);
		List<Vector3> vecs7 = new List<Vector3> ();
		vecs7.Add (vec7);
		item7.list=vecs7;
		item7.showOrder = 1;

		#region 声控光敏开关测试--声控开关
//		CircuitItem item7 = new CircuitItem();
//		item7.ID = 1;
//		item7.name = "VOswitch";
//		item7.type = ItemType.VoiceOperSwitch;
//		item7.theta = 90;
//		//Vector3 vec3 = new Vector3 (0.1f,0.4f,0);
//		Vector3 vec7 = new Vector3 (0,-75,0);
//		List<Vector3> vecs7 = new List<Vector3> ();
//		vecs7.Add (vec7);
//		item7.list=vecs7;
//		item7.showOrder = 1;
		#endregion

		#region 单刀双掷开关测试1
//		CircuitItem item7 = new CircuitItem();
//		item7.ID = 1;
//		item7.name = "SPTDswitch";
//		item7.type = ItemType.DoubleDirSwitch;
//		item7.theta = 90;
//		//Vector3 vec3 = new Vector3 (0.1f,0.4f,0);
//		Vector3 vec7 = new Vector3 (0,-75,0);
//		List<Vector3> vecs7 = new List<Vector3> ();
//		vecs7.Add (vec7);
//		item7.list=vecs7;
//		item7.showOrder = 1;
		#endregion


		CircuitItem item2 = new CircuitItem();
		item2.ID = 2;
		item2.name = "Bulb";
		item2.type = ItemType.Bulb;
		//Vector3 vec2 = new Vector3 (-0.4f,0,0);
		Vector3 vec2 = new Vector3 (-170,-69,0);
		List<Vector3> vecs2 = new List<Vector3> ();
		vecs2.Add (vec2);
		item2.list=vecs2;
		item2.showOrder = 2;

		#region 两个开关测试
//		CircuitItem item3 = new CircuitItem();
//		item3.ID = 3;
//		item3.name = "Switch";
//		item3.type = ItemType.Switch;
//		//Vector3 vec3 = new Vector3 (0.1f,0.4f,0);
//		Vector3 vec3 = new Vector3 (3,140,0);
//		List<Vector3> vecs3 = new List<Vector3> ();
//		vecs3.Add (vec3);
//		item3.list=vecs3;
//		item3.showOrder = 3;
		#endregion

		#region 小喇叭测试
		CircuitItem item3 = new CircuitItem();
		item3.ID = 3;
		item3.name = "loudSpeaker";
		item3.type = ItemType.Loudspeaker;//ItemType.Switch;
		//Vector3 vec3 = new Vector3 (0.1f,0.4f,0);
		Vector3 vec3 = new Vector3 (3,140,0);
		List<Vector3> vecs3 = new List<Vector3> ();
		vecs3.Add (vec3);
		item3.list=vecs3;
		item3.showOrder = 3;
		#endregion


		#region 光敏开关测试
//		CircuitItem item3 = new CircuitItem();
//		item3.ID = 3;
//		item3.name = "lightActSwitch";
//		item3.type = ItemType.LightActSwitch;
//		//Vector3 vec3 = new Vector3 (0.1f,0.4f,0);
//		Vector3 vec3 = new Vector3 (3,140,0);
//		List<Vector3> vecs3 = new List<Vector3> ();
//		vecs3.Add (vec3);
//		item3.list=vecs3;
//		item3.showOrder = 3;
		#endregion

		#region 声控开关测试
//		CircuitItem item3 = new CircuitItem();
//		item3.ID = 3;
//		item3.name = "VOswitch";
//		item3.type = ItemType.VoiceOperSwitch;
//		//Vector3 vec3 = new Vector3 (0.1f,0.4f,0);
//		Vector3 vec3 = new Vector3 (3,140,0);
//		List<Vector3> vecs3 = new List<Vector3> ();
//		vecs3.Add (vec3);
//		item3.list=vecs3;
//		item3.showOrder = 3;
		#endregion

		#region 单刀双掷开关测试
//		CircuitItem item3 = new CircuitItem();
//		item3.ID = 3;
//		item3.name = "SPDTswitch";
//		item3.type = ItemType.DoubleDirSwitch;
//		//Vector3 vec3 = new Vector3 (0.1f,0.4f,0);
//		Vector3 vec3 = new Vector3 (3,140,0);
//		List<Vector3> vecs3 = new List<Vector3> ();
//		vecs3.Add (vec3);
//		item3.list=vecs3;
//		item3.showOrder = 3;
		#endregion


		CircuitItem item5 = new CircuitItem();
		item5.ID = 4;
		item5.name = "CircuitLine";
		item5.type = ItemType.CircuitLine;
		item5.powered = false;
		Vector3 vec21 = new Vector3 (205, -46, 0);
		Vector3 vec22 = new Vector3 (197, -24, 0);
		Vector3 vec23 = new Vector3 (183, 1, 0);
		Vector3 vec24 = new Vector3 (169, 27, 0);
		Vector3 vec25 = new Vector3 (143, 51, 0);
		Vector3 vec26 = new Vector3 (113, 72, 0);
		Vector3 vec27 = new Vector3 (90,88, 0);
		List<Vector3> vecs5 = new List<Vector3> ();
		vecs5.Add (vec21);
		vecs5.Add (vec22);
		vecs5.Add (vec23);
		vecs5.Add (vec24);
		vecs5.Add (vec25);
		vecs5.Add (vec26);
		vecs5.Add (vec27);
		item5.list = vecs5;
		item5.showOrder = 4;


		CircuitItem item6 = new CircuitItem();
		item6.ID = 5;
		item6.name = "CircuitLine";
		item6.type = ItemType.CircuitLine;
		item6.powered = false;
		Vector3 vec31 = new Vector3 (-103, 92, 0);
		Vector3 vec32 = new Vector3 (-129, 66, 0);
		Vector3 vec33 = new Vector3 (-144, 48, 0);
		Vector3 vec34 = new Vector3 (-162, 28, 0);
		Vector3 vec35 = new Vector3 (-181, 10, 0);
		Vector3 vec36 = new Vector3 (-206, -5, 0);
		Vector3 vec37 = new Vector3 (-214,-24, 0);
		List<Vector3> vecs6 = new List<Vector3> ();
		vecs6.Add (vec31);
		vecs6.Add (vec32);
		vecs6.Add (vec33);
		vecs6.Add (vec34);
		vecs6.Add (vec35);
		vecs6.Add (vec36);
		vecs6.Add (vec37);
		item6.list = vecs6;
		item6.showOrder = 5;

		CircuitItem item4 = new CircuitItem();
		item4.ID = 6;
		item4.name = "CircuitLine";
		item4.type = ItemType.CircuitLine;
		item4.powered = false;
		Vector3 vec01 = new Vector3 (-126, -77, 0);
		Vector3 vec02 = new Vector3 (-103, -83, 0);
		Vector3 vec03 = new Vector3 (-75, -88, 0);
		Vector3 vec04 = new Vector3 (-50, -101, 0);
		Vector3 vec05 = new Vector3 (-21, -96, 0);
		Vector3 vec06 = new Vector3 (3, -111, 0);
		Vector3 vec07 = new Vector3 (30,-116, 0);
		Vector3 vec08 = new Vector3 (53, -112, 0);
		Vector3 vec09 = new Vector3 (74, -103, 0);
		Vector3 vec10 = new Vector3 (98, -97, 0);
		Vector3 vec11 = new Vector3 (119, -87, 0);
		List<Vector3> vecs4 = new List<Vector3> ();
		vecs4.Add (vec01);
		vecs4.Add (vec02);
		vecs4.Add (vec03);
		vecs4.Add (vec04);
		vecs4.Add (vec05);
		vecs4.Add (vec06);
		vecs4.Add (vec07);
		vecs4.Add (vec08);
		vecs4.Add (vec09);
		vecs4.Add (vec10);
		vecs4.Add (vec11);
		item4.list = vecs4;
		item4.showOrder = 6;


		itemListTest.Add(item1);
		itemListTest.Add(item7);
		itemListTest.Add(item2);
		itemListTest.Add(item3);
		itemListTest.Add(item5);
		itemListTest.Add(item6);
		itemListTest.Add(item4);

		//Debug.Log ("itemListTest.count====" + itemListTest.Count);

	}

	/// <summary>
	/// 获取图标数据，并保存------图像识别后返回给我一个List<CircuitItem> list,我这边去接收，用List<CircuitItem> itemList来保存。
	/// </summary>
	/// <param name="circuitItemList">图标的集合</param>
	public void GetCircuitVec(List<CircuitItem> circuitItemList)
	{
		
		//拷贝传进来的值
		for(int i=0;i<circuitItemList.Count;++i)
		{
			//判断传进来的list是否是按showOrder从小到大排序的
			if (circuitItemList [i].showOrder == i)
			{
				CircuitItem item= circuitItemList [i];
				itemList.Add(item);
			}

		}
			
	}

}
