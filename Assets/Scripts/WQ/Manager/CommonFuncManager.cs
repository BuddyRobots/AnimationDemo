using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCircuit;

public class CommonFuncManager : MonoBehaviour 
{

	public static CommonFuncManager _instance;
	const int SOUND_CRITERION = 1;//音量大小标准，可以调整以满足具体需求

	void Awake()
	{
		_instance = this;
	}

	/// <summary>
	/// 接通电路
	/// </summary>
	public void OpenCircuit()
	{
		PhotoRecognizingPanel._instance.transValue = 1;
		for (int i = 0; i < PhotoRecognizingPanel._instance.arrowList.Count; i++) 
		{
			if (PhotoRecognizingPanel._instance.arrowList [i]) 
			{
				//PhotoRecognizingPanel._instance.arrowList [i].GetComponent<ArrowCtrl> ().speed *=2;
				PhotoRecognizingPanel._instance.arrowList [i].GetComponent<UISprite> ().alpha = 1;//显示电流
			}

		}
		transform.Find ("bulb").GetComponent<UISprite> ().spriteName = "bulbOn";//灯亮 
		GetComponent<PhotoRecognizingPanel> ().isArrowShowDone = true;//标记已经播放电流
	}
		

	public bool isSoundLoudEnough()
	{
		float volume = MicroPhoneInput.getInstance ().getSoundVolume();
		if(volume > SOUND_CRITERION)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// compute  the angle according to two points
	/// </summary>
	/// <returns>The angle.</returns>
	/// <param name="from">vector2 first</param>
	/// <param name="to">vector2 second</param>
	public float TanAngle(Vector2 from, Vector2 to)
	{
		float xdis = to.x - from.x;//计算临边长度  
		float ydis = to.y - from.y;//计算对边长度  
		float tanValue = Mathf.Atan2(ydis, xdis);//反正切得到弧度  
		float angle = tanValue * Mathf.Rad2Deg;//弧度转换为角度  
		return angle;  
	}

	/// <summary>
	///1个电池的情况下刷新items 遍历新的circuitItem，根据其powered属性值来刷新（譬如灯泡，音响这种受开关控制的item）UI
	/// </summary>
	/// <param name="circuitItems">Circuit items.</param>
	public void CircuitItemRefresh(List<CircuitItem> circuitItems)
	{
		for (int i = 0; i < circuitItems.Count ; i++) 
		{
			string tag = circuitItems [i].ID.ToString ();//获取每一个item的ID，
			GameObject temp = GameObject.FindGameObjectWithTag (tag);//对应界面上的Tag来找到对应的图标对象
			switch (circuitItems [i].type) //根据图标的类型，和power值，来更改sprite进行刷新
			{
				//这里只需要刷新受开关控制的item（灯泡，音响，电磁炉）的显示，开关的更新不在这里进行,声控，光敏，延时开关的显示都不在这里进行控制
				case ItemType.Bulb:
					temp.GetComponent<UISprite>().spriteName=(circuitItems [i].powered ? "bulbOn":"bulbOff");
					break;
				case ItemType.VoiceTimedelaySwitch:
					temp.GetComponent<UISprite>().spriteName=(circuitItems [i].powered ? "VoiceDelayOn":"VoiceDelayOff");
					break;
				//如果是电磁炉
				case ItemType.InductionCooker:
				GameObject steam = temp.transform.Find ("Steam").gameObject;
				if (circuitItems [i].powered) 
				{
					steam.GetComponent<MyAnimation> ().canPlay = true;
				}
				else 
				{
					steam.GetComponent<MyAnimation> ().canPlay=false;
				}
					break;
	
				case ItemType.Loudspeaker:
					AudioSource tempAudio = temp.GetComponent<AudioSource> ();
					if (circuitItems [i].powered) // this item is power on
					{
					temp.GetComponent<MyAnimation> ().canPlay = true;
						if (!tempAudio.isPlaying) 
						{
							tempAudio.Play ();
							if (LevelManager.currentLevelData.LevelID==9) 
							{
								tempAudio.volume = 1f;
							}
							else
							{
								tempAudio.volume = 0.5f;
							}
						}
					} 
					else // this item is power off
					{
					temp.GetComponent<MyAnimation> ().canPlay=false;
						if (tempAudio.isPlaying) 
						{
							tempAudio.Stop ();
						}
					}
					break;
				default:
					break;
			}
		}
	}



	//传进来所有的线
	public void ArrowsRefresh(List<CircuitItem> circuitItems)
	{
		for (int i = 0; i < circuitItems.Count; i++) 
		{

			if (circuitItems[i].type==ItemType.CircuitLine) 
			{

				string tag = circuitItems [i].ID.ToString ();
				GameObject[] temps = GameObject.FindGameObjectsWithTag(tag);
				List<GameObject> arrows=new List<GameObject>();
				arrows.Clear();
				for (int k = 0; k < temps.Length; k++)
				{
					if (temps[k].name.Contains("arrow")) 
					{
						arrows.Add(temps[k]);
					}

				}
				foreach (var item in arrows) 
				{ 
					if (item) 
					{
						item.GetComponent<UISprite>().alpha = (circuitItems [i].powered ? 1:0);
					}
				}

			}
		}

	}




	/// <summary>
	/// 两个电池的情况下刷新items
	/// </summary>
	/// <param name="circuitItems">Circuit items.</param>
	public void CircuitItemRefreshWithTwoBattery(List<CircuitItem> circuitItems)
	{
		for (int i = 0; i < circuitItems.Count ; i++) 
		{
			string tag = circuitItems [i].ID.ToString ();//获取每一个item的ID，
			GameObject temp = GameObject.FindGameObjectWithTag (tag);//对应界面上的Tag来找到对应的图标对象
			switch (circuitItems [i].type) //根据图标的类型，和power值，来更改sprite进行刷新
			{
			case ItemType.Bulb:
				temp.GetComponent<UISprite>().spriteName=(circuitItems [i].powered ? "bulbSpark":"bulbOff");
				break;
			case ItemType.InductionCooker:
				GameObject steam = temp.transform.Find ("Steam").gameObject;
				if (circuitItems [i].powered) 
				{
					steam.GetComponent<MyAnimation> ().canPlay = true;
				}
				else 
				{
					steam.GetComponent<MyAnimation> ().canPlay=false;
				}
				break;
			case ItemType.Loudspeaker:
				AudioSource tempAudio = temp.GetComponent<AudioSource> ();

				if (circuitItems [i].powered) // this item is power on
				{
					temp.GetComponent<MyAnimation> ().canPlay = true;
					if (!tempAudio.isPlaying) 
					{
						tempAudio.Play ();
						tempAudio.volume = 1f;
					}
					else
					{

						tempAudio.volume = 1f;

					}
				} 
				else // this item is power off
				{
					temp.GetComponent<MyAnimation> ().canPlay=false;
					if (tempAudio.isPlaying) 
					{
						tempAudio.Stop ();
					}
				}
				break;
			default:
				break;

			}
		}

	}

}
