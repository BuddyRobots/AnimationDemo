using UnityEngine;
using System;

namespace AnimationDemo
{
	public static class MyUtils
	{
		public static string loadJson(string path)
		{
			string filePath = "Json/" + path.Replace(".json", "");

			TextAsset targetFile = Resources.Load<TextAsset>(filePath);

			return targetFile.text;
		}


		public static Texture2D loadPNG(string filePath)
		{
			return Resources.Load<Texture2D>(filePath);
		}
	}
}