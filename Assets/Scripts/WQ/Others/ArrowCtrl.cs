using UnityEngine;
using System.Collections;
/// <summary>
/// 点到点的移动控制
/// </summary>
public class ArrowCtrl : MonoBehaviour 
{

	/// <summary>
	/// 箭头移动的速度
	/// </summary>
	public float speed;

	/// <summary>
	/// 箭头是否需要移动的标志
	/// </summary>
	private bool isNeedMove;

	/// <summary>
	/// 是否到达目的地的标志
	/// </summary>
	private  bool isAtDest;

	/// <summary>
	/// 目标点
	/// </summary>
	//[HideInInspector]
	public Vector3 dest;


	private Vector3 direction;


	void Start()
	{
		isAtDest = false;
	}


	void Update()
	{
		if (isNeedMove) //如果需要移动，就慢慢移动
		{
			transform.localPosition += direction * speed;
			if (Vector3.Distance( transform.localPosition,dest)< 3) 
			{
				isAtDest = true;
				isNeedMove = false;
			}
			else
			{
				isAtDest = false;
			}
		}
	}

	/// <summary>
	/// 设定目标点
	/// </summary>
	/// <param name="pos">Position.</param>
	public void SetDestination(Vector3 pos ,int speed = 3)
	{
		this.speed = speed;
		dest = pos;//记录目标点，方便后面判断是不是移动到了这个点
		direction= (pos - transform.localPosition).normalized;//箭头的方向
		transform.up = direction;
		isNeedMove = true;
	}
		
	public bool IsAtDest()
	{
		return isAtDest;
	}

	/// <summary>
	/// 箭头停止移动
	/// </summary>
	public void Stop()
	{
		isNeedMove = false;
	}
	/// <summary>
	/// 箭头继续移动
	/// </summary>
	public void ContinueStart()
	{
		isNeedMove = true;
	}
}
