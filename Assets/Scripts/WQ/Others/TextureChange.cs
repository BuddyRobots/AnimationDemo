using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureChange : MonoBehaviour {

	public Dictionary<int,Texture> textures = new Dictionary<int, Texture>();
	private int currentID = 0;

	public void Init(params Texture[] data)
	{
		for (int i = 0; i < data.Length; i++) {
			textures.Add (i, data [i]);
		}
		GetComponent<UITexture> ().mainTexture = textures [currentID];
	}


	public bool Change(bool isNext)
	{


		currentID = isNext ? Mathf.Min (currentID + 1, textures.Count - 1) : Mathf.Max (0, currentID - 1);

		int temp = isNext ? Mathf.Min (currentID + 1, textures.Count - 1) : Mathf.Max (0, currentID - 1);
		GetComponent<UITexture> ().mainTexture = textures [currentID];

		return currentID != temp;

	}
}
