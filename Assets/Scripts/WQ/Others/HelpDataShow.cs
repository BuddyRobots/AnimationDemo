using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HelpDataShow : MonoBehaviour {

 List<int> levelPictureNum = new List<int> ();

	TextureChange Helper;

	void Awake()
	{
		InitNameData ();
		//在这初始化，根据关卡
//		Init(LevelManager.currentLevelData.LevelID);
	}



	public void InitFromStart()
	{
		Debug.Log("----------InitFromStart");

		if (Helper)
		{
			Destroy (Helper);
		}
		Init (16);
	}

	public void InitFromLevel()
	{
		Debug.Log("----------InitFromLevel");

		Init(LevelManager.currentLevelData.LevelID);
	}


	/// <summary>
	/// from 1 to 16
	/// </summary>
	/// <param name="level">Level.</param>
	public void Init(int level)
	{
		Helper = gameObject.AddMissingComponent<TextureChange> ();

		Texture[] temp = new Texture[levelPictureNum [level - 1]];
		for (int i = 0; i < levelPictureNum[level - 1]; i++) {
			temp [i] = GetTexture (GetName (level - 1, i));
		}

		Debug.Log("-----------"+temp.Length);

		Helper.Init (temp);

	}

	public bool Next()
	{
		return Helper.Change (true);
	}

	public bool Back()
	{
		return Helper.Change (false);
	}

	private void InitNameData()
	{
		
		levelPictureNum.Add (3);//-----------------0
		levelPictureNum.Add (3);//-----------------1
		levelPictureNum.Add (4);//-----------------2
		levelPictureNum.Add (4);//-----------------3
		levelPictureNum.Add (4);//-----------------4
		levelPictureNum.Add (4);//-----------------5
		levelPictureNum.Add (4);//-----------------6
		levelPictureNum.Add (4);//-----------------7
		levelPictureNum.Add (4);//-----------------8
		levelPictureNum.Add (3);//-----------------9
		levelPictureNum.Add (4);//-----------------10
		levelPictureNum.Add (4);//-----------------11
		levelPictureNum.Add (5);//-----------------12
		levelPictureNum.Add (5);//-----------------13
		levelPictureNum.Add (6);//-----------------14
		levelPictureNum.Add (3);//-----------------15
	}
		
	private Texture GetTexture(string name)
	{
		//图片保存路径
		string path = "Pictures/AllDemoPics/";
		return Resources.Load<Texture> (path + name);
	}
	private string GetName(int level, int id)
	{
		string ret = "Level_" + level + "_" + id;
		return ret;
	}
}
