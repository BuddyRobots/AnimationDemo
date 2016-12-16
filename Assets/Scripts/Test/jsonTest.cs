using UnityEngine;
using System.Collections;
using LitJson;
using AnimationDemo;

public class jsonTest : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		string path = @"Json/leftwing";

		var asset = MyUtils.loadJson(path);

		JsonData data = JsonMapper.ToObject(asset);
		JsonData position_x = data["vector_x"];
		JsonData position_y = data["vector_y"];
		JsonData rotation  = data["rotation"];


		Debug.Log(rotation[0]);
		Debug.Log((int)rotation[0]);
		Debug.Log((double)rotation[0]);


		Vector2 point = new Vector2((int)position_x[0], (int)position_x[10]);
		Debug.Log(point);

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

