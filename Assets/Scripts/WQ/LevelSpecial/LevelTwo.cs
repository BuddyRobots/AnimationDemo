using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// level 2
public class LevelTwo : MonoBehaviour 
{
	[HideInInspector]
	public bool isRemoveLine=false;

	[Range(0,2)]
	public float radius;
	private bool isFingerShow = false;

	void OnEnable()
	{
		isRemoveLine=false;
		if (PhotoRecognizingPanel._instance) 
		{
			PhotoRecognizingPanel._instance.isNeedToCreateArrow = true;
		}
	}

	void Update () 
	{
		if (isRemoveLine) 
		{
			if (!GetComponent<PhotoRecognizingPanel> ().isArrowShowDone) 
			{
				CommonFuncManager._instance.OpenCircuit ();	

			}
			Vector3	randomPos = GetComponent<PhotoRecognizingPanel> ().ChooseRandomPointOnLine ();
			GetComponent<PhotoRecognizingPanel> ().ShowFingerOnLine(randomPos);//动画播放3秒后，在电线上的任意随机点位置出现小手
			StartCoroutine (TouchLineAfterAwhile ());
		}
	}
		
	IEnumerator TouchLineAfterAwhile()
	{
		yield return new WaitForSeconds (3f);
		TouchToDestroyLine ();
	}
		
	/// <summary>
	/// 触摸移动销毁线条
	/// </summary>
	void TouchToDestroyLine()
	{
		PhotoRecognizingPanel temp = GetComponent<PhotoRecognizingPanel> ();
		#if UNITY_EDITOR 
		if (Input.GetMouseButtonDown(0)) 
		{
			Ray ray = transform.parent.Find ("Camera").GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) 
			{
//				Debug.Log("hit.collider.name===="+hit.collider.gameObject.name);
				if (hit.collider.gameObject.name=="MaskBg" || hit.collider.gameObject.name=="Btn") 
				{
					return ;	
				}
				else
				{
					DestroyLine(hit.point,0.03f/*radius*/);
				}
			}
		}
		#elif UNITY_IPHONE 
		if (Input.touchCount>0 /*&& Input.GetTouch (0).phase == TouchPhase.Moved*/) //如果有移动触摸
		{
			Ray ray = transform.parent.Find ("Camera").GetComponent<Camera> ().ScreenPointToRay (Input.GetTouch(0).position);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) 
			{
				if (hit.collider.gameObject.name=="MaskBg" || hit.collider.gameObject.name=="Btn") 
				{
					return ;	
				}
				else
				{
					DestroyLine(hit.point,0.03f);
				}
			}
		}
		#endif

	}



	void DestroyLine(Vector3 center, float radius)
	{
		Collider[] hitColliders=Physics.OverlapSphere(center,radius);
		for (int k = 0; k < hitColliders.Length; k++) 
		{
			GameObject tempGo=hitColliders[k].gameObject;
			if (tempGo.tag!="mask" && tempGo.name.Contains("lineNew"))
			{
				int index=int.Parse(tempGo.tag);
				//擦除线是应该判断线有没有电流，如果有电流则消除线并且断开电流，如果没有则只消除线,不用断开电流
				if (GetImage._instance.itemList[index].powered) 
				{
					Destroy(hitColliders[k].gameObject);
					BreakCircuit();
				}
				else
				{

					Destroy(hitColliders[k].gameObject);

				}


			}
		}
	}

	void BreakCircuit()
	{
		PhotoRecognizingPanel temp = GetComponent<PhotoRecognizingPanel> ();
		if (temp.finger)
		{
			Destroy (temp.finger);
		}
		transform.Find ("bulb").GetComponent<UISprite> ().spriteName = "bulbOff";
		temp.StopCreateArrows();
		for (int i = 0; i < temp.arrowList.Count; i++) 
		{
			Destroy (temp.arrowList[i]);
		}

	}



}
