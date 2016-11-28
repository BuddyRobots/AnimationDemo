using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AnimationTest : MonoBehaviour 
{
	public Transform temp;
	public float intervalTime = 0.04f;//1秒24帧


	private  List<Vector2> vectorDataList=new List<Vector2>();//该集合用来存储已经转换了坐标轴（但是没有转化原点）的数据
	private List<Vector2> offset_dataList=new List<Vector2>();//该集合用来存储偏移量数据
	private List<Vector2> zeroOrigin_dataList=new List<Vector2>();//该集合用来存储转化了原点的数据，可以直接用

	private bool isPlaying=false;
	private int index;
	private float time;
	private Vector2 toOriginPoint_offset;//假设图片初始位置是（0，0），该变量是数据的起始位置到原点的差向量

	void Start () 
	{
		//Debug.Log("fps===="+Application.targetFrameRate);
		//Application.targetFrameRate=60;
		isPlaying=false;
		GetData_Test._instance.ReadInfo();

		temp.GetComponent<UITexture>().width=GetData_Test._instance.widthAndHeightList[0];
		temp.GetComponent<UITexture>().height=GetData_Test._instance.widthAndHeightList[1];




		#region 根据数据进行坐标的相应转化
		foreach (var vec in GetData_Test._instance.vectorDataList) 
		{
			Vector2 vector=new Vector2();

			//转换坐标，坐标轴转换
			vector.x=vec.y;
			vector.y=vec.x;
			vector.y*=-1;
			vectorDataList.Add(vector);
		}
		#endregion

		#region 平移坐标
		toOriginPoint_offset=Vector2.zero-vectorDataList[0];

		for (int i = 0; i < vectorDataList.Count; i++) 
		{
			Vector2 zeroOrigin_vec=vectorDataList[i]+toOriginPoint_offset;
			zeroOrigin_dataList.Add(zeroOrigin_vec);
		}
		#endregion

		#region 计算偏移量
		for (int i = 0; i < vectorDataList.Count-1; i++) 
		{
			Vector2 offset=vectorDataList[i+1]-vectorDataList[i];
//			Debug.Log("vectorDataList["+i+"]-vectorDataList[0]="+offset);

			Vector2 modify_offset=new Vector2(offset.x,offset.y);
//			offset_dataList.Add(offset);
			offset_dataList.Add(modify_offset);
		}
//		Debug.Log(".........");
//		Debug.Log("offset_dataList_count==="+offset_dataList.Count);
//		foreach (var item in offset_dataList) 
//		{
//			Debug.Log(item);
//		}
		#endregion

		Play();
//		StartCoroutine (Test());
	}
	
	#region 根据偏移量 //如果用偏移量，数据的起始帧和末尾帧坐标得保持一致，可以在数据中手动添加一帧，或者记录开始位置，重新播放按照开始位置重新来;而且要调整偏移量的比率，大图偏移量大，小图偏移量小
	/*
	void Update () 
	{
		if (isPlaying) 
		{
			if (Time.time >= time + intervalTime) 
			{
				time=Time.time;
				ChangePositionByOffset();
			}
		}

	}
	*/
	#endregion


	#region 根据坐标
	///*
	void Update () 
	{
		if (isPlaying) 
		{
			if (Time.time >= time + intervalTime) 
			{
				Debug.Log("%%%%%%% "+Time.time);
				time=Time.time;
				ChangePositionByVector();
			}
		}

	}
	//*/
	#endregion


	#region 开协同
//	void Update ()
//	{
//		if (isPlaying) 
//		{
//			isPlaying = false;
//			StartCoroutine (WaitForAwhile ());
//
//		}
//
//	}
	#endregion

	IEnumerator WaitForAwhile()
	{
		
		yield return new WaitForSeconds(intervalTime);

		ChangePositionByVector();

		isPlaying=true;

	}

	IEnumerator Test()
	{
		while (true)
		{
			if (isPlaying)
			{
				ChangePositionByVector();
				yield return new WaitForSeconds(intervalTime);
				Debug.Log("^^^^^^ "+Time.time);
			}
			else
			{
				yield return new WaitForFixedUpdate();
			}
		}
	}

	void ChangePositionByOffset()                                                                                              
	{
		temp.localPosition+=new Vector3(offset_dataList[index%(offset_dataList.Count)].x, offset_dataList[index%(offset_dataList.Count)].y, 0);
		temp.localRotation=Quaternion.identity;
		temp.localScale=Vector3.one;
		index++;

	}
	void ChangePositionByVector()
	{
//		Debug.Log("******* "+Time.time);
		temp.localPosition=zeroOrigin_dataList[index%zeroOrigin_dataList.Count];
		temp.localRotation=Quaternion.identity;
		temp.localScale=Vector3.one;
		index++;

	}

	public void Play()
	{
		isPlaying=true;
		time=Time.time;
	}
}
