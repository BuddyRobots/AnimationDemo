using UnityEngine;
using System.Collections;
using LitJson;
using AnimationDemo;

public class jsonTest : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		string path = @"Json/body";

		var asset = MyUtils.loadJson(path);


	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

