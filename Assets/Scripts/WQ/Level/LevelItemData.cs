using UnityEngine;
using System.Collections;



public enum LevelProgress
{

	Todo=0,
	Doing,
	Done
}

public class LevelItemData 
{
	//这是保存关卡的纯信息类，包括每一关的ID,关卡名字，描述文字,和完成进度
	private int m_LevelID;
	private int m_LevelNumber;//关卡数字
	private string m_LevelName;//关卡名字
	private string m_LevelDescription;//关卡描述
	private LevelProgress progress;
	private int preLevelID;
	private int nextLevelID;

	public int PrelevelID
	{
		get 
		{ 
			return preLevelID;
		}
		set
		{ 
		
			preLevelID = value;
		}
	}
	public int NextLevelID
	{
		get 
		{ 
			return nextLevelID;
		}
		set
		{ 

			nextLevelID = value;
		}
	}
	public int LevelID
	{

		get
		{ 
			return m_LevelID;
		}
		set
		{ 
			m_LevelID = value;
		}

	}
	public int LevelNumber
	{
		get
		{ 
			return m_LevelNumber;
		}
		set
		{ 
			m_LevelNumber = value;
		}
	}

	public string LevelName
	{
		get
		{ 
			return m_LevelName;
		}
		set
		{
			m_LevelName = value;
		}
	}
	public string LevelDescription
	{
		get
		{ 
			return m_LevelDescription;
		}
		set
		{ 
			m_LevelDescription = value;
		}
	}

	public  LevelProgress Progress
	{
		get
		{ 
			return progress;
		}

		set
		{ 
		
			progress = value;
		}

	}


}
