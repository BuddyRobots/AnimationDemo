using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AnimationDemo;
using System;

public class SegmentTest : MonoBehaviour {
	private Texture2D tex;
	private List<Texture2D> partTexList=new List<Texture2D>();
	private UITexture uiTex;
	private string pngPath="Pictures/Photos/1479694037";
	private const float heightRate=3.46f;

	private float timeIndex;
	private int callCount;

	// Use this for initialization
	void Start () 
	{
		uiTex=transform.GetComponent<UITexture>();
		uiTex.width=Constant.WIDTH;
		uiTex.height=Constant.HEIGHT;
		//		Debug.Log(uiTex.width+"   "+uiTex.height);

		tex=new Texture2D(Constant.WIDTH,Constant.HEIGHT);
		tex=Segmentation.loadPNG(pngPath);
		uiTex.mainTexture=tex;
		partTexList=Segmentation.segment(tex);

	
	}

	void OnEnable()
	{
		Debug.Log("OnEnable");
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeIndex+=Time.deltaTime;
		if (timeIndex>=5f) 
		{
			timeIndex=0;
			callCount++;
			partTexList=Segmentation.segment(tex);
			Debug.Log("call segment "+ callCount +" time");
			
		}
	
	}

	void OnDisable()
	{
		Debug.Log("OnDisable");

	}

	void OnDestroy()
	{
		Debug.Log("-------OnDestroy");



	}

}
