using UnityEngine;
using System;

namespace AnimationDemo
{
	public static class MyUtils
	{
		public static string loadJson(string path)
		{
			TextAsset targetFile = Resources.Load<TextAsset>(path);

			return targetFile.text;
		}


		public static Texture2D loadPNG(string filePath)
		{
			return Resources.Load<Texture2D>(filePath);
		}
	}
}