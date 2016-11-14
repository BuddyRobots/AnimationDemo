using UnityEngine;
using System.Collections;

/// <summary>
/// 确认按钮，点击按钮关闭父对象
/// </summary>
public class ConfirmBtnCtrl : MonoBehaviour 
{
	void OnClick()
	{
		transform.parent.gameObject.SetActive (false);
		System.GC.Collect();
//		Resources.UnloadUnusedAssets();
	}
}
