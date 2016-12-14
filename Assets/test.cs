using UnityEngine;
using System.Collections;
using AnimationDemo;
using System;

public class test : MonoBehaviour
{
	String jsonPath = @"Json/body";



	// Use this for initialization
	void Start ()
	{
		Owl olw = new Owl(jsonPath);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

