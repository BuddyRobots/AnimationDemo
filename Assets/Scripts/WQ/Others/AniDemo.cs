using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AnimationDemo;

public class AniDemo : MonoBehaviour 
{

	private List<GameObject> goList=new List<GameObject>();//UI上显示的对象的集合
	private List<Texture> texList=new List<Texture>();

	// 各个部位对应的角度------double
	List<double> body_angles_double=new List<double>();
	List<double> leftWing_angles_double=new List<double>();
	List<double> rightWing_angles_double=new List<double>();
	List<double> leftLeg_angles_double=new List<double>();
	List<double> rightLeg_angles_double=new List<double>();
	private List<List<double>> total_angleList_double=new List<List<double>>();

	private List<Vector2> body_vectorDataList = new List<Vector2> ();//该集合用来存储已经转换了坐标轴（但是没有转化原点）的数据
	private List<Vector2> leftWing_vectorDataList = new List<Vector2> ();
	private List<Vector2> rightWing_vectorDataList = new List<Vector2> ();
	private List<Vector2> leftLeg_vectorDataList = new List<Vector2> ();
	private List<Vector2> rightLeg_vectorDataList = new List<Vector2> ();
	private List<List<Vector2>> total_posList=new List<List<Vector2>>();

	private List<int> widthList=new List<int>();//texture的宽度信息
	private List<int> heightList=new List<int>(); 

	private bool isPlaying = false;
	private int index;
	private int length;
	private float time;

	private Texture2D tex;//the whole texture , parts are sliced according to it
	private List<Texture2D> partTexList=new List<Texture2D>();

//	private string pngPath="Pictures/Photos/1479694037";
	private const float heightRate=1f;

	private List<string> jsonPaths=new List<string>();

	private Owl owl;

	void Start()
	{
		tex=GetImage._instance.texture;

		GetPartTexures();
		GetPartData();


		index=0;
		length=0;
		length=owl.body.position.Count;
		isPlaying=false;
		Init();
		Play();
	}

	void GetPartTexures()
	{
		//init jsonpath
		jsonPaths.Add(@"Json/body");
		jsonPaths.Add(@"Json/leftwing");
		jsonPaths.Add(@"Json/rightwing");
		jsonPaths.Add(@"Json/leftleg");
		jsonPaths.Add(@"Json/rightleg");

		owl=new Owl(tex, jsonPaths);

		//init partTexList
		partTexList.Add(owl.body.texture);
		partTexList.Add(owl.leftWing.texture);
		partTexList.Add(owl.rightWing.texture);
		partTexList.Add(owl.leftLeg.texture);
		partTexList.Add(owl.rightLeg.texture);
	}



	void GetPartData()
	{
		body_vectorDataList=owl.body.position;
		leftWing_vectorDataList=owl.leftWing.position;
		rightWing_vectorDataList=owl.rightWing.position;
		leftLeg_vectorDataList=owl.leftLeg.position;
		rightLeg_vectorDataList=owl.rightLeg.position;

		total_posList.Add (body_vectorDataList);//把猫头鹰放在本身的位置（初始位置）做动画
		total_posList.Add(leftWing_vectorDataList);
		total_posList.Add(rightWing_vectorDataList);
		total_posList.Add(leftLeg_vectorDataList);
		total_posList.Add(rightLeg_vectorDataList);

		body_angles_double=owl.body.rotation;
		leftWing_angles_double=owl.leftWing.rotation;
		rightWing_angles_double=owl.rightWing.rotation;
		leftLeg_angles_double=owl.leftLeg.rotation;
		rightLeg_angles_double=owl.rightLeg.rotation;

		total_angleList_double.Add(body_angles_double);
		total_angleList_double.Add(leftWing_angles_double);
		total_angleList_double.Add(rightWing_angles_double);
		total_angleList_double.Add(leftLeg_angles_double);
		total_angleList_double.Add(rightLeg_angles_double);
	}


	void Init()
	{
		for (int i = 0; i < partTexList.Count; i++) 
		{
			texList.Add(partTexList[i]);
		}
			
		for (int i = 0; i < 5; i++) 
		{
			widthList.Add((int)(partTexList[i].width*heightRate));
			heightList.Add((int)(partTexList[i].height*heightRate));
		}
			
		//创建对象
		GameObject parent=GameObject.Find("UI Root/PhotoRecognizingPanel(Clone)/Owl");
		for (int i = 0; i < texList.Count; i++) 
		{
			UITexture temp = NGUITools.AddChild<UITexture>(parent);
			temp.mainTexture=texList[i];
			temp.width=widthList[i];
			temp.height=heightList[i];
			temp.transform.localPosition=total_posList[i][0];
			temp.transform.localRotation=Quaternion.AngleAxis((float)total_angleList_double[i][0],Vector3.forward);

			if (i==0) 
			{
				temp.depth=3;
			} else
			{
				temp.depth=2;
			}

			goList.Add(temp.gameObject);
		}	
	}
		

	void Update () 
	{
		if (isPlaying) 
		{
			if (Time.time >= time + Constant.ANIMATION_FRAME_INTERVAL) 
			{
				time=Time.time;
				PlayAnimation();
			}
		}

	}

	void PlayAnimation()                                                                                              
	{
		for (int i=0; i < texList.Count;i++) 
		{
			goList[i].transform.localPosition=total_posList[i%texList.Count][index%length];
			goList[i].transform.localRotation=Quaternion.AngleAxis((float)total_angleList_double[i%texList.Count][index%length],Vector3.forward);
			goList[i].transform.localScale=Vector3.one;
		}
		index++;
	}
		
	public void Play()
	{
		isPlaying=true;
	}

}
