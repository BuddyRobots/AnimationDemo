﻿using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenCVForUnity;
using MagicCircuit;

public class GetImage : MonoBehaviour
{
	public static GetImage _instance;

	// Flag for taking 10 photos
	public bool isTakingPhoto = false;

	// Parameters for using WebCam
	[HideInInspector]
	public  Texture2D texture;
	private WebCamTexture webCamTexture;
	private WebCamDevice webCamDevice;
	private bool initDone = false;
	private int webCam_width  = 640;
	private int webCam_height = 480;

	public bool isStartUpdate=true;


	void OnEnable()
	{
		_instance = this;
		StartCoroutine(init());
	}

	private IEnumerator init()
	{
		if (webCamTexture != null)
		{
			webCamTexture.Stop();
			initDone = false;
		}
		WebCamDevice[] devices = WebCamTexture.devices;

		#if UNITY_EDITOR  
		webCamDevice = WebCamTexture.devices[0];
		#elif UNITY_IPHONE 
		webCamDevice = WebCamTexture.devices[1];
		#endif 

		webCamTexture = new WebCamTexture (webCamDevice.name, webCam_width, webCam_height);
		webCamTexture.Play();

		while (true)
		{
			if (webCamTexture.didUpdateThisFrame)
			{
				Mat frameImg = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC3);
				webCam_width  = webCamTexture.width;
				webCam_height = webCamTexture.height;

				texture = new Texture2D(frameImg.cols(), frameImg.rows(), TextureFormat.RGBA32, false);
				gameObject.GetComponent<Renderer>().material.mainTexture = texture;

				initDone = true;
				break;
			}
			else
			{
				yield return 0;
			}
		}
	}
		
	void Update()
	{
		if (isStartUpdate) 
		{
			if (!initDone)
				return;

			Mat frameImg = new Mat(webCam_height, webCam_width, CvType.CV_8UC3);
			if (webCamTexture.didUpdateThisFrame)
			{
				Utils.webCamTextureToMat(webCamTexture, frameImg);

				#if UNITY_EDITOR
				#elif UNITY_IPHONE
				RotateCamera.rotate(ref frameImg);
				#endif
	

				texture.Resize(frameImg.cols(), frameImg.rows());
				Utils.matToTexture2D(frameImg, texture);

			}
			frameImg.Dispose();
		}

	}

}