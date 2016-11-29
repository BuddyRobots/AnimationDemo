using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GetData_Test :MonoBehaviour
{
	public static GetData_Test _instance;

	public TextAsset body_vectorInfoText;
	public TextAsset leftWing_vectorInfoText;

	[HideInInspector]
	public  List<Vector2> body_vectorDataList;
	[HideInInspector]
	public  List<Vector2> leftWing_vectorDataList;
	[HideInInspector]
	public List<int> leftWing_angleList;

	[HideInInspector]
	public List<int> widthAndHeightList;

	void Awake()
	{
		_instance=this;
	}
		

	//解析Txt文本文件
	public void ReadInfo()
	{
		
		#region 解析身体数据
		string body_Text = body_vectorInfoText.text;//取到文本里面所有的字符串
		string[] bodyData_origin = body_Text.Split ('\n');//用换行来取

		//获取宽高
		/*
		string[] widthHeight = bodyData_origin [0].Split (',');
		string width = widthHeight [0].Split (':') [1];
		string height = widthHeight [1].Split (':') [1];
		int w, h;
		if (int.TryParse (width, out w)) {
			//			Debug.Log("width====" + w);

		}
		if (int.TryParse (height, out h)) {
			//			Debug.Log("height====" + h);
		}
		widthAndHeightList.Add (w);
		widthAndHeightList.Add (h);
		*/

		//获取身体坐标信息
		string[] bodyVectors = new string[bodyData_origin.Length - 1];
		for (int i = 1; i < bodyData_origin.Length; i++) 
		{
			bodyVectors [i - 1] = bodyData_origin [i];
		}
		foreach (string str in bodyVectors) { //遍力数组
			string[] proArray = str.Split (',');//根据,号来拆分文本里面的数据
			string tmp_x = proArray [0];
			string tmp_y = proArray [1];
			//int id=int.Parse(proArray[0]);//出错原因是最后有小数产生0.0， 转换会出异常，所以为了防止异常发生，需要用int.TryParse()
			//int y=int.Parse(proArray[1]);
			int x;
			if (int.TryParse (tmp_x, out x)) {
				//				Debug.Log("id====" + id);
			}
			int y;
			if (int.TryParse (tmp_y, out y)) {
				//				Debug.Log("y====" + y);
			}
			body_vectorDataList.Add (new Vector2 (x, y));
		}
		#endregion


		#region 解析左翅膀数据
		string leftWing_text = leftWing_vectorInfoText.text;
		string[] leftWingData_origin = leftWing_text.Split ('\n');
		string[] leftWingVectors = new string[leftWingData_origin.Length - 1]; 
		for (int i = 1; i < leftWingData_origin.Length; i++) 
		{
			leftWingVectors [i - 1] = leftWingData_origin [i];
		}
		foreach (var item in leftWingVectors)
		{
			string[] dataSplit = item.Split (',');
			string vec_x = dataSplit [0];
			string vec_y = dataSplit [1];
			string angle_temp = dataSplit [2];

			int x;
			int y;
			int angle;
			if (int.TryParse (vec_x, out x)) 
			{
//				Debug.Log ("x====" + x);
			}
			if (int.TryParse (vec_y, out y)) 
			{
//				Debug.Log ("y====" + y);
			}
			if (int.TryParse (angle_temp, out angle)) 
			{
//				Debug.Log ("angle===" + angle);
			}


			leftWing_vectorDataList.Add(new Vector2(x,y));
			leftWing_angleList.Add(angle);


		}
		#endregion









	}

}
