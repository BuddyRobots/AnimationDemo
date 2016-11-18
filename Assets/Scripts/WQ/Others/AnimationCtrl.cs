using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationCtrl : MonoBehaviour 
{
	//一个texture有一组对应的list<float> angle和一组对应的 list<vector3> pos信息，依据各自对应的信息进行更新显示
	private List<GameObject> goList=new List<GameObject>();//UI上显示的对象的集合
	private List<Texture> texList=new List<Texture>();
	private List<List<float>> angleList=new List<List<float>>();
	private List<List<Vector3>> posList=new List<List<Vector3>>();
	private List<int> widthList=new List<int>();//texture的宽度信息
	private List<int> heightList=new List<int>(); 

	private bool isPlaying=false;
	private int index;
	private int length;//位置个数，角度个数，（每一帧都有位置信息和旋转信息，位置个数=角度个数=总帧数）

	void Init()
	{
		//for test...   -----texlist的值由图像识别返回
		Texture tex_0=GetTexure("head");
		Texture tex_1=GetTexure("body");
		Texture tex_2=GetTexure("R-arm");
		Texture tex_3=GetTexure("L-hand");
		Texture tex_4=GetTexure("L-leg");
		Texture tex_5=GetTexure("R-leg");

		texList.Add(tex_0);
		texList.Add(tex_1);
		texList.Add(tex_2);
		texList.Add(tex_3);
		texList.Add(tex_4);
		texList.Add(tex_5);

		// 各个部位对应的角度
		List<float> tempAngles_0=new List<float>();
		List<float> tempAngles_1=new List<float>();
		List<float> tempAngles_2=new List<float>();
		List<float> tempAngles_3=new List<float>();
		List<float> tempAngles_4=new List<float>();
		List<float> tempAngles_5=new List<float>();


		for (int i = 0; i < 60; i++) 
		{
			tempAngles_0.Add(0);
			tempAngles_1.Add(0);
		}

		for (int i =30; i> 0; i--) 
		{
			tempAngles_2.Add(i);
		}
		for (int i =0; i<30; i++) 
		{
			tempAngles_2.Add(i);
		}

		for (int i =-30; i < 0; i++) {
			tempAngles_3.Add(i);
		}
		for (int i =0; i >-30; i--) {
			tempAngles_3.Add(i);
		}


		for (int i =30; i> 0; i--) 
		{
			tempAngles_4.Add(i);
		}
		for (int i =0; i<30; i++) 
		{
			tempAngles_4.Add(i);
		}

		for (int i =30; i> 0; i--) 
		{
			tempAngles_5.Add(i);
		}
		for (int i =0; i<30; i++) 
		{
			tempAngles_5.Add(i);
		}

		angleList.Add(tempAngles_0);
		angleList.Add(tempAngles_1);
		angleList.Add(tempAngles_2);
		angleList.Add(tempAngles_3);
		angleList.Add(tempAngles_4);
		angleList.Add(tempAngles_5);

		//各个部位对应的位置
		List<Vector3> pos_0=new List<Vector3>();
		List<Vector3> pos_1=new List<Vector3>();
		List<Vector3> pos_2=new List<Vector3>();
		List<Vector3> pos_3=new List<Vector3>();
		List<Vector3> pos_4=new List<Vector3>();
		List<Vector3> pos_5=new List<Vector3>();

		Vector3 pos=Vector3.zero;

		for (int i = 0; i < 60; i++) 
		{
			pos=new Vector3(-9.6f,88.2f,0);
			pos_0.Add(pos);
		}
		for (int i = 0; i < 60; i++) 
		{
			pos=new Vector3(-0.2f,-19.1f,0);
			pos_1.Add(pos);
		}
		for (int i = 0; i < 60; i++) {
			pos=new Vector3(-88.4f,-27.6f,0);
			pos_2.Add(pos);
		}
		for (int i = 0; i < 60; i++) {
			pos=new Vector3(107.3f,-12.5f,0);
			pos_3.Add(pos);
		}
		for (int i = 0; i < 60; i++) {
			pos=new Vector3(-43.7f,-127.1f,0);
			pos_4.Add(pos);
		}
		for (int i = 0; i < 60; i++) {
			pos=new Vector3(42.7f,-128.5f,0);
			pos_5.Add(pos);
		}
		posList.Add(pos_0);
		posList.Add(pos_1);
		posList.Add(pos_2);
		posList.Add(pos_3);
		posList.Add(pos_4);
		posList.Add(pos_5);

		widthList.Add(160);
		heightList.Add(122);
		widthList.Add(128);
		heightList.Add(148);
		widthList.Add(135);
		heightList.Add(69);
		widthList.Add(149);
		heightList.Add(69);
		widthList.Add(82);
		heightList.Add(104);
		widthList.Add(67);
		heightList.Add(94);

		length=tempAngles_0.Count;
	}



	void Start()
	{
		index=0;
		length=0;
		isPlaying=false;

		Init();

		GameObject parent=GameObject.Find("UI Root/PhotoRecognizingPanel(Clone)");
		for (int i = 0; i < texList.Count; i++) 
		{
			UITexture temp = NGUITools.AddChild<UITexture>(parent);
			temp.mainTexture=texList[i];
			temp.width=widthList[i];
			temp.height=heightList[i];
			temp.transform.localPosition=posList[i][0];
			temp.transform.localRotation=Quaternion.AngleAxis(angleList[i][0],Vector3.forward);
		
			goList.Add(temp.gameObject);
		}	
		Play();
	}


	void Update () 
	{
		if (isPlaying) 
		{
			PlayAnimation();
		}
	}


	void PlayAnimation()
	{

		for (int i = 0; i < texList.Count; i++) 
		{
			goList[i].transform.localPosition=	posList[i][index%length];
			goList[i].transform.localRotation=Quaternion.AngleAxis(angleList[i][index%length],Vector3.forward);

			index++;
		}
	}


	private Texture GetTexure(string name)
	{
		string path="Pictures/Tex/";
		return Resources.Load<Texture>(path+name);

	}
		
	public void Play()
	{
		isPlaying=true;
	}
}
