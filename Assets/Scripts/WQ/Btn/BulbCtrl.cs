using UnityEngine;
using System.Collections;

public class BulbCtrl : MonoBehaviour 
{

	[HideInInspector]
	public bool isSemiTrans = false;

	private UISprite bulbSprite;

	void OnEnable()
	{
		bulbSprite=this.gameObject.GetComponent<UISprite>();

	}

//	void Update () 
//	{
//		if (isSemiTrans) 
//		{//如果是半透明状态
//			gameObject.GetComponent<UISprite>().spriteName="semiTransBulb";
//
//		} 
//		else//如果是正常状态 
//		{
//			//如果有电则是bulbOn ...
//			gameObject.GetComponent<UISprite>().spriteName="bulbOn";
//			//没电则是bulboff  to do ..
//		}
//	}

	void OnClick()
	{
		isSemiTrans = !isSemiTrans;
	}










}
