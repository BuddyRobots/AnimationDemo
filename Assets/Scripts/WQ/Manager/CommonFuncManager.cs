using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCircuit;

public class CommonFuncManager : MonoBehaviour 
{

	public static CommonFuncManager _instance;
	const int SOUND_CRITERION = 1;//音量大小标准，可以调整以满足具体需求

	void Awake()
	{
		_instance = this;
	}


	/// <summary>
	/// compute  the angle according to two points
	/// </summary>
	/// <returns>The angle.</returns>
	/// <param name="from">vector2 first</param>
	/// <param name="to">vector2 second</param>
	public float TanAngle(Vector2 from, Vector2 to)
	{
		float xdis = to.x - from.x;//计算临边长度  
		float ydis = to.y - from.y;//计算对边长度  
		float tanValue = Mathf.Atan2(ydis, xdis);//反正切得到弧度  
		float angle = tanValue * Mathf.Rad2Deg;//弧度转换为角度  
		return angle;  
	}

}
