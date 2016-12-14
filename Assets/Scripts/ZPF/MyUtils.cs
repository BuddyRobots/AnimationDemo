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


		public static class JsonHelper
		{
			public static T[] FromJson<T>(string json)
			{
				Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
				return wrapper.Items;
			}

			public static string ToJson<T>(T[] array)
			{
				Wrapper<T> wrapper = new Wrapper<T>();
				wrapper.Items = array;
				return UnityEngine.JsonUtility.ToJson(wrapper);
			}

			[Serializable]
			private class Wrapper<T>
			{
				public T[] Items;
			}
		}
	}
}