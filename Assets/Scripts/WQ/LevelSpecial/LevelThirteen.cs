using UnityEngine;
using System.Collections;
using MagicCircuit;
//level 13---声控+光敏

public class LevelThirteen : MonoBehaviour
{
	private Transform VOswitch;
	private Transform LAswitch;
	private Transform micphoneBtn;

	private UITexture nightBg=null;

	private BoxCollider micPhoneBoxCol;
	private UIButton micPhoneUIBtn;
	private MicroPhoneBtnCtrl micPhontBtnCtrl;
	private float changeTime = 3f;//渐变的总时间
	private  float changeTimer = 0;

	[HideInInspector]
	public bool isVOswitchAndLAswitchTogether = false;
	/// <summary>
	/// 光敏开关是否闭合的标志
	/// </summary>
	private bool isLAswitchOn = false;
	private bool isVOswitchOn=false;
	private bool isStartRecord = false;
	private bool isFingerShow = false;
	private bool isFingerDestroyed=false;
	private bool isNightModeOnce=false;
	private bool CurrLASwitchStatus=false;
	private bool PreLASwitchStatus=false;
	private bool CurrVOSwitchStatus=false;
	private bool PreVOSwitchStatus=false;
	/// <summary>
	/// 标记太阳月亮按钮的初始状态是显示太阳
	/// </summary>
	private bool preSunSwitchStatues = true;

	//如果玩家先点击了声控开关，声控开关闭合，再点击太阳月亮，光敏开关闭合，线路虽然连通了，也是不行的,只有先闭合光敏，再闭合声控，电路才通

	void OnEnable ()
	{

		CurrLASwitchStatus=false;
		PreLASwitchStatus=false;
		CurrVOSwitchStatus=false;
		PreVOSwitchStatus=false;

		isVOswitchAndLAswitchTogether = false;
		isLAswitchOn = false;
		changeTime = 3f;
		changeTimer = 0;
		isVOswitchOn=false;
		isFingerShow = false;
		isNightModeOnce=false;
		isFingerDestroyed=false;
		isStartRecord = false;

		VOswitch = transform.Find ("voiceOperSwitch");
		LAswitch = transform.Find ("lightActSwitch");
		micphoneBtn = transform.Find ("MicroPhoneBtn");
		nightBg = PhotoRecognizingPanel._instance.nightMask;


		micPhoneBoxCol = micphoneBtn.gameObject.GetComponent<BoxCollider>();
		micPhoneUIBtn = micphoneBtn.gameObject.GetComponent<UIButton>();
		micPhontBtnCtrl = micphoneBtn.gameObject.GetComponent<MicroPhoneBtnCtrl>();

	}
	
	//声控+光敏    （只有晚上有声音的时候灯才亮）
	//需要伴随话筒按钮，太阳/月亮按钮出现
	//在太阳月亮按钮出现小手，点击后变成月亮，小手消失，背景渐变暗，光敏开关在背景变暗之前闭合；
	//在话筒按钮出现小手，点击后小手消失，弹出提示框提示玩家发出声音，收集到声音后提示框消失，声控开关闭合



	void Update () 
	{
		if (isVOswitchAndLAswitchTogether) 
		{
			if (!isFingerShow) 
			{
				GetComponent<PhotoRecognizingPanel> ().ShowFinger(transform.Find("SunAndMoonWidget").localPosition);
				isFingerShow=true;
			}

			if (transform.Find ("SunAndMoonWidget").GetComponent<MoonAndSunCtrl> ().isDaytime && !preSunSwitchStatues)
			{
				micphoneBtn.GetComponent<MicroPhoneBtnCtrl> ().isCollectVoice=false;
				isStartRecord = false;
			}
			#region 如果是晚上（点击了太阳按钮）
			if (!transform.Find ("SunAndMoonWidget").GetComponent<MoonAndSunCtrl> ().isDaytime) 
			{ 
				isNightModeOnce = true;
				//销毁小手
				if (!isFingerDestroyed) 
				{
					Destroy (PhotoRecognizingPanel._instance.finger);
					isFingerShow=false;
					isFingerDestroyed = true;
				}
				//蒙板渐变暗，快全暗时，光敏开关闭合标志打开
				changeTimer += Time.deltaTime;
				if (changeTimer >= changeTime) 
				{
					changeTimer = changeTime;
				}
				nightBg.alpha = Mathf.Lerp (0, 1f, changeTimer / changeTime);
				if (changeTimer >= changeTime * 5 / 6) 
				{
					isLAswitchOn = true;
					CurrLASwitchStatus=true;
					if (PreLASwitchStatus!=CurrLASwitchStatus) 
					{
						GetImage._instance.cf.switchOnOff (int.Parse (LAswitch.gameObject.tag), true);
						LAswitch.GetComponent<UISprite> ().spriteName = "LAswitchOn";
						PreLASwitchStatus=CurrLASwitchStatus;
					}
			

				}
				if (isLAswitchOn)//如果光敏开光闭合了
				{	
					micPhoneBoxCol.enabled=true;
					micPhoneUIBtn.enabled=true;
					micPhontBtnCtrl.enabled=true;

					GetComponent<PhotoRecognizingPanel> ().ShowFinger (transform.Find ("MicroPhoneBtn").localPosition);//在话筒按钮出现小手
				}
				if (micphoneBtn.GetComponent<MicroPhoneBtnCtrl> ().isCollectVoice)//点击了话筒按钮
				{ 
					if (PhotoRecognizingPanel._instance.finger) 
					{
						Destroy (PhotoRecognizingPanel._instance.finger);	//小手消失
					}

					if (!isStartRecord) //开始收集声音
					{ 
						PhotoRecognizingPanel._instance.noticeToMakeVoice.SetActive (true);//弹出提示框
						PhotoRecognizingPanel._instance.voiceCollectionMark.SetActive(true);//弹出声音收集图片
						PhotoRecognizingPanel._instance.voiceCollectionMark.transform.Find ("Wave").GetComponent<MyAnimation> ().canPlay = true;//显示声音收集动画
						MicroPhoneInput.getInstance ().StartRecord ();
						isStartRecord = true;
					}
					if (CommonFuncManager._instance.isSoundLoudEnough ()) //收集到声音
					{
						isVOswitchOn=true;
						PhotoRecognizingPanel._instance.noticeToMakeVoice.SetActive (false);//提示框消失
						PhotoRecognizingPanel._instance.voiceCollectionMark.transform.Find ("Wave").GetComponent<MyAnimation> ().canPlay = false;
						PhotoRecognizingPanel._instance.voiceCollectionMark.SetActive(false);

						MicroPhoneInput.getInstance().StopRecord();
						GetImage._instance.cf.switchOnOff (int.Parse (VOswitch.gameObject.tag), true);
						VOswitch.GetComponent<UISprite>().spriteName="VOswitchOn";

						CommonFuncManager._instance.CircuitItemRefresh (GetImage._instance.itemList);
					}
				}

			}
			#endregion
			#region 如果是白天
			else 
			{
				if (isNightModeOnce)//白天已经切换到过黑夜了
				{
					transform.Find("MicroPhoneBtn").gameObject.GetComponent<BoxCollider>().enabled=false;
					transform.Find("MicroPhoneBtn").gameObject.GetComponent<MicroPhoneBtnCtrl>().enabled=false;

					changeTimer -= Time.deltaTime;
					if (changeTimer <= 0) 
					{
						changeTimer =0;
					}
					nightBg.alpha = Mathf.Lerp (0, 1f, changeTimer / changeTime);
					if (changeTimer <= changeTime* 1/6) 
					{
						CurrLASwitchStatus = false;
						if (PreLASwitchStatus!=CurrLASwitchStatus) 
						{
							GetImage._instance.cf.switchOnOff (int.Parse (LAswitch.gameObject.tag), false);
							LAswitch.GetComponent<UISprite>().spriteName= "LAswitchOff";
							GetImage._instance.cf.switchOnOff (int.Parse (VOswitch.gameObject.tag), false);

							CommonFuncManager._instance.CircuitItemRefresh (GetImage._instance.itemList);
							VOswitch.GetComponent<UISprite>().spriteName="VOswitchOff";
							PreLASwitchStatus=CurrLASwitchStatus;
						}

					}
				}
			}
			#endregion
			preSunSwitchStatues = transform.Find ("SunAndMoonWidget").GetComponent<MoonAndSunCtrl> ().isDaytime;
			CommonFuncManager._instance.ArrowsRefresh(GetImage._instance.itemList);
		}
	}
}
	