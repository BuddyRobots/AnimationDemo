using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GetData_Test :MonoBehaviour
{
	public static GetData_Test _instance;
	public TextAsset vectorInfoText;
	public  List<Vector2> vectorDataList;
	public List<int> widthAndHeightList;

	void Awake()
	{
		_instance=this;
	}
		

	//解析Txt文本文件
	public void ReadInfo()
	{
		string text=vectorInfoText.text;//取到文本里面所有的字符串
		string[] strArray_origin=text.Split('\n');//用换行来取

		//获取宽高
		string[] widthHeight=strArray_origin[0].Split(',');
		string width=widthHeight[0].Split(':')[1];
		string  height=widthHeight[1].Split(':')[1];
		int w, h;
		if (int.TryParse(width,out w)) {
//			Debug.Log("width====" + w);

		}
		if (int.TryParse (height,out h)) {
//			Debug.Log("height====" + h);
		}
		widthAndHeightList.Add(w);
		widthAndHeightList.Add(h);


		//获取坐标信息
		string[] strArray_data=new string[strArray_origin.Length-1];
		for (int i = 1; i <strArray_origin.Length; i++)
		{
			strArray_data[i-1]=strArray_origin[i];
		}
		foreach (string str in strArray_data) //遍力数组
		{
			string[] proArray=str.Split(',');//根据,号来拆分文本里面的数据
			string tmp_x = proArray[0];
			string tmp_y = proArray[1];
			//int id=int.Parse(proArray[0]);//出错原因是最后有小数产生0.0， 转换会出异常，所以为了防止异常发生，需要用int.TryParse()
			//int y=int.Parse(proArray[1]);
			int id;
			if (int.TryParse(tmp_x, out id))
			{
//				Debug.Log("id====" + id);
			}
			int y;
			if (int.TryParse(tmp_y, out y))
			{
//				Debug.Log("y====" + y);
			}
			vectorDataList.Add(new Vector2(id,y));
		}

	}

}
