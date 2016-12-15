using UnityEngine;
using System;
using System.Collections.Generic;

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


		public static int average(List<int> array)
		{
			int sum = 0;
			for (var i = 0; i < array.Count; i++)
				sum += array[i];
			return sum/array.Count;
		}
	}
}