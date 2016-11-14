using UnityEngine;
using System.Collections;

public class LevelHandle : MonoBehaviour 
{

	public static LevelHandle _instance;

	void Awake()
	{
		_instance = this;
	}
		
	void OnEnable()
	{
		//给识别界面添加组件
		switch (LevelManager.currentLevelData.LevelID)
		{
		case 1:
			transform.gameObject.AddComponent<LevelOne> ();
			break;
		case 2:
			transform.gameObject.AddComponent<LevelTwo> ();
			break;
		case 3:
			transform.gameObject.AddComponent<LevelThree> ();
			break;
		case 4:
			transform.gameObject.AddComponent<LevelFour> ();
			break;
		case 5:
			transform.gameObject.AddComponent<LevelFive> ();
			break;
		case 6:
			transform.gameObject.AddComponent<LevelSix> ();
			break;
		case  7:      
			transform.gameObject.AddComponent<LevelSeven> ();
			break;
		case 8:
			transform.gameObject.AddComponent<LevelEight> ();
			break;
		case  9:      
			transform.gameObject.AddComponent<LevelNine> ();
			break;
		case 10:      
			transform.gameObject.AddComponent<LevelTen> ();
			break;
		case 11:
			transform.gameObject.AddComponent<LevelEleven> ();
			break;
		case 12:
			transform.gameObject.AddComponent<LevelTwelve> ();
			break;
		case 13:
			transform.gameObject.AddComponent<LevelThirteen> ();
			break;
		case 14:
			transform.gameObject.AddComponent<LevelFourteen> ();
			break;
		case 15:
			transform.gameObject.AddComponent<LevelFifteen> ();
			break;
		default:
			break;
		}
	}


	/// <summary>
	/// 根据关卡等级添加对应的关卡脚本
	/// </summary>
	/// <param name="levelID">Level I.</param>
	public void CircuitHandleByLevelID(int levelID)
	{
		switch (levelID) 
		{
		case 1:
			gameObject.GetComponent<LevelOne> ().isPlayCircuitAnimation = true;
			break;
		case 2:
			gameObject.GetComponent<LevelTwo> ().isRemoveLine = true;
			break;
		case 3:
			GetComponent<LevelThree> ().isNormalSwitchOccur = true;
			break;
		case 4:
			GetComponent<LevelFour> ().isLoudSpeakerOccur = true;
			break;
		case 5:
			GetComponent<LevelFive> ().isTwoSwitchInSeriesCircuit = true;
			break;
		case 6:
			GetComponent<LevelSix>().isParrallelCircuit=true;
			break;
		case 7:
			GetComponent<LevelSeven>().isParallelCircuitWithTwoBattery=true;
			break;
		case 8:
			GetComponent<LevelEight>().isLevelEight=true;
			break;
		case 9:
			GetComponent<LevelNine>().isParallelCircuitWithTwoBulb=true;
			break;
		case 10:
			GetComponent<LevelTen>().isLevelTen=true;
			break;
		case 11:
			GetComponent<LevelEleven> ().isVOswitchOccur = true;
			break;
		case 12:
			GetComponent<LevelTwelve> ().isLAswitchOccur = true;
			break;
		case 13:
			GetComponent<LevelThirteen> ().isVOswitchAndLAswitchTogether = true;
			break;
		case 14:
			GetComponent<LevelFourteen> ().isLevelFourteen = true;
			break;
		case 15:
			GetComponent<LevelFifteen> ().isSPDTswitchOccur = true;
			break;
		default:
			break;
		}
	}

	void OnDisable()
	{
		//删除组件
		switch (LevelManager.currentLevelData.LevelID)
		{
		case 1:
			Destroy (transform.GetComponent<LevelOne> ());
			break;
		case 2:
			Destroy (transform.GetComponent<LevelTwo> ());
			break;
		case 3:
			Destroy (transform.GetComponent<LevelThree> ());
			break;
		case 4:
			Destroy (transform.GetComponent<LevelFour> ());
			break;
		case 5:
			Destroy(transform.GetComponent<LevelFive> ());
			break;
		case 6:
			Destroy(transform.GetComponent<LevelSix> ());
			break;
		case 7:
			Destroy(transform.GetComponent<LevelSeven> ());

			break;
		case 8:
			Destroy(transform.GetComponent<LevelEight> ());
			break;
		case  9:
			Destroy (transform.GetComponent<LevelNine> ());
			break;
		case 10:
			Destroy(transform.GetComponent<LevelTen> ());
			break;
		case 11:
			Destroy(transform.GetComponent<LevelEleven> ());
			break;
		case 12:
			Destroy(transform.GetComponent<LevelTwelve> ());
			break;
		case 13:
			Destroy (transform.GetComponent<LevelThirteen> ());
			break;
		case 14:
			Destroy (transform.GetComponent<LevelFourteen> ());
			break;
		case 15:
			Destroy (transform.GetComponent<LevelFifteen> ());
			break;
		default:
			break;
		}
	}
}
