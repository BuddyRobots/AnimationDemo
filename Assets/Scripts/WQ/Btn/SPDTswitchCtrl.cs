using UnityEngine;
using System.Collections;

public class SPDTswitchCtrl : MonoBehaviour 
{
	/// <summary>
	/// 记录开关当前的状态
	/// </summary>
	[HideInInspector]
	public bool isRightOn = true;//默认右闭合
	/// <summary>
	/// 记录点击开关前的状态，
	/// </summary>
	[HideInInspector]
	public bool preStatus=true;
	private UISprite SPDTsprite;


	void Start () 
	{
		SPDTsprite = GetComponent<UISprite> ();
		preStatus=true;
	}

	void Update () 
	{
		if (isRightOn && SPDTsprite.spriteName != "SPDTRight" ) 
		{
			SPDTsprite.spriteName = "SPDTRight";
		}
		if (!isRightOn && SPDTsprite.spriteName != "SPDTLeft") 
		{
			SPDTsprite.spriteName = "SPDTLeft";
		
		}
	}

	void OnClick()
	{
		preStatus = isRightOn;
		isRightOn = !isRightOn;

		GetImage._instance.cf_SPDT.switchOnOff (int.Parse (gameObject.tag), isRightOn);
		CommonFuncManager._instance.CircuitItemRefresh (GetImage._instance.itemList);

		for (var i = 0; i < GetImage._instance.itemList.Count; i++)
			Debug.Log("SPDTSwitchCtrl.cs OnClick : GetImage._instance.itemList["+i+"].list[0] = " + GetImage._instance.itemList[i].list[0]);

	}
}
