using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AnimationTest : MonoBehaviour 
{
	public Transform body;
	public Transform leftWing;
	public Transform rightWing;
	public Transform leftLeg;
	public Transform rightLeg;




	private Vector2 leftWing_offSet=new Vector2(-214,-61);
	private Vector2 rightWing_offSet=new Vector2(188,-55);
	private Vector2 leftLeg_offSet=new Vector2(-67,-232);
	private Vector2 rightLeg_offSet=new Vector2(44,-230);

	public float intervalTime = 0.04f;//1秒24帧


	private  List<Vector2> body_vectorDataList=new List<Vector2>();//该集合用来存储已经转换了坐标轴（但是没有转化原点）的数据
	private List<Vector2> body_offset_dataList=new List<Vector2>();//该集合用来存储偏移量数据
	private List<Vector2> body_zeroOrigin_dataList=new List<Vector2>();//该集合用来存储转化了原点的数据，可以直接用



	private List<Vector2> leftWing_vectorDataList=new List<Vector2>();
	private List<Vector2> rightWing_vectorDataList=new List<Vector2>();
	private List<Vector2> leftLeg_vectorDataList=new List<Vector2>();
	private List<Vector2> rightLeg_vectorDataList=new List<Vector2>();


	private bool isPlaying=false;
	private int index;
	private float time;
	private Vector2 toOriginPoint_offset;//假设图片初始位置是（0，0），该变量是数据的起始位置到原点的差向量

	void Start () 
	{
		
		isPlaying=false;
		GetData_Test._instance.ReadInfo();

//		temp_body.GetComponent<UITexture>().width=GetData_Test._instance.widthAndHeightList[0];
//		temp_body.GetComponent<UITexture>().height=GetData_Test._instance.widthAndHeightList[1];

		#region 根据数据进行坐标的相应转化
		foreach (var vec in GetData_Test._instance.body_vectorDataList) 
		{
			Vector2 vector=new Vector2();

			//转换坐标，坐标轴转换
			vector.x=vec.y;
			vector.y=vec.x;
			vector.y*=-1;
			body_vectorDataList.Add(vector);
		}
		#endregion
		//get leftWing vectors
//		foreach (var item in GetData_Test._instance.leftWing_vectorDataList)
//		{
//
//			Vector2 vector=new Vector2();
//			vector.x=item.x+leftWing_offSet.x;
//			vector.y=item.y+leftWing_offSet.y;
//		
//			leftWing_vectorDataList.Add(vector);
//		}



		#region 平移坐标
		toOriginPoint_offset=Vector2.zero-body_vectorDataList[0];

		for (int i = 0; i < body_vectorDataList.Count; i++) 
		{
			Vector2 zeroOrigin_vec=body_vectorDataList[i]+toOriginPoint_offset;
			body_zeroOrigin_dataList.Add(zeroOrigin_vec);
		}
		#endregion

		//左翅膀数据
		for (int i = 0; i < GetData_Test._instance.leftWing_vectorDataList.Count; i++) 
		{
			Vector2 vector=new Vector2();
			vector.x=GetData_Test._instance.leftWing_vectorDataList[i].x+leftWing_offSet.x;
			vector.y=GetData_Test._instance.leftWing_vectorDataList[i].y+leftWing_offSet.y;
			leftWing_vectorDataList.Add(vector+body_zeroOrigin_dataList[i]);

		}

		//右翅膀数据
		for (int i = 0; i < GetData_Test._instance.rightWing_vectorDataList.Count; i++) 
		{
			Vector2 vector=new Vector2();
			vector.x=GetData_Test._instance.rightWing_vectorDataList[i].x+rightWing_offSet.x;
			vector.y=GetData_Test._instance.rightWing_vectorDataList[i].y+rightWing_offSet.y;
			rightWing_vectorDataList.Add(vector+body_zeroOrigin_dataList[i]);

		}

		//左脚数据
		for (int i = 0; i < GetData_Test._instance.leftWing_vectorDataList.Count; i++) 
		{
			Vector2 vector=new Vector2();
			vector.x=GetData_Test._instance.leftLeg_vectorDataList[i].x+leftLeg_offSet.x;
			vector.y=GetData_Test._instance.leftLeg_vectorDataList[i].y+leftLeg_offSet.y;
			leftLeg_vectorDataList.Add(vector+body_zeroOrigin_dataList[i]);

		}
		// 右脚数据
		for (int i = 0; i < GetData_Test._instance.rightWing_vectorDataList.Count; i++) 
		{
			Vector2 vector=new Vector2();
			vector.x=GetData_Test._instance.rightLeg_vectorDataList[i].x+rightLeg_offSet.x;
			vector.y=GetData_Test._instance.rightLeg_vectorDataList[i].y+rightLeg_offSet.y;
			rightLeg_vectorDataList.Add(vector+body_zeroOrigin_dataList[i]);

		}




		#region 计算偏移量
		for (int i = 0; i < body_vectorDataList.Count-1; i++) 
		{
			Vector2 offset=body_vectorDataList[i+1]-body_vectorDataList[i];
//			Debug.Log("vectorDataList["+i+"]-vectorDataList[0]="+offset);

			Vector2 modify_offset=new Vector2(offset.x,offset.y);
//			offset_dataList.Add(offset);
			body_offset_dataList.Add(modify_offset);
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
//				Debug.Log("%%%%%%% "+Time.time);
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
//		}
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
		body.localPosition+=new Vector3(body_offset_dataList[index%(body_offset_dataList.Count)].x, body_offset_dataList[index%(body_offset_dataList.Count)].y, 0);
		body.localRotation=Quaternion.identity;
		body.localScale=Vector3.one;
		index++;

	}
	void ChangePositionByVector()
	{
//		Debug.Log("******* "+Time.time);
		body.localPosition=body_zeroOrigin_dataList[index%body_zeroOrigin_dataList.Count];
		body.localRotation=Quaternion.identity;
		body.localScale=Vector3.one;

		leftWing.localPosition=leftWing_vectorDataList[index%leftWing_vectorDataList.Count];
//		leftWing.localRotation=GetData_Test._instance.leftWing_angleList[index%GetData_Test._instance.leftWing_angleList.Count];
		leftWing.localRotation=Quaternion.AngleAxis((float)GetData_Test._instance.leftWing_angleList[index%GetData_Test._instance.leftWing_angleList.Count],Vector3.forward);
		leftWing.localScale=Vector3.one;


		rightWing.localPosition=rightWing_vectorDataList[index%rightWing_vectorDataList.Count];
		rightWing.localRotation=Quaternion.AngleAxis((float)GetData_Test._instance.rightWing_angleList[index%GetData_Test._instance.rightWing_angleList.Count],Vector3.forward);
		rightWing.localScale=Vector3.one;

		leftLeg.localPosition=leftLeg_vectorDataList[index%leftLeg_vectorDataList.Count];
		leftLeg.localRotation=Quaternion.AngleAxis((float)GetData_Test._instance.leftLeg_angleList[index%GetData_Test._instance.leftLeg_angleList.Count],Vector3.forward);
		leftLeg.localScale=Vector3.one;

		rightLeg.localPosition=rightLeg_vectorDataList[index%rightLeg_vectorDataList.Count];
		rightLeg.localRotation=Quaternion.AngleAxis((float)GetData_Test._instance.rightLeg_angleList[index%GetData_Test._instance.rightLeg_angleList.Count],Vector3.forward);
		rightLeg.localScale=Vector3.one;








		index++;

	}

	public void Play()
	{
		isPlaying=true;
		time=Time.time;
	}
}
