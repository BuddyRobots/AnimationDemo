using UnityEngine;
using System.Collections;
//level 1
public class LevelOne : MonoBehaviour 
{

	/// <summary>
	///执行该脚本中update函数中的方法的标志
	/// </summary>
	[HideInInspector]
	public bool isPlayCircuitAnimation=false;

	void Update () 
	{
		
		if (isPlayCircuitAnimation)
		{
			//if (!GetComponent<PhotoRecognizingPanel> ().isArrowShowDone)//如果没有播放过电流
			{
				CommonFuncManager._instance.OpenCircuit ();

			}
		}
	}

}
