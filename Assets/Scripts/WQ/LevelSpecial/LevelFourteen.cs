using UnityEngine;
using System.Collections;
using MagicCircuit;


public class LevelFourteen : MonoBehaviour
{
	[HideInInspector]
	public bool isLevelFourteen = false;
	/// <summary>
	/// 光敏开关是否闭合的标志
	/// </summary>
	private bool isLAswitchOn = false;

	private float changeTime = 3f;//渐变的总时间
	private  float changeTimer = 0;
	private UITexture nightBg=null;
	private bool isVOswitchOn=false;
	private bool isStartRecord = false;
	private bool isFingerShow = false;
	private bool isFingerDestroyed=false;
	private bool isNightModeOnce=false;

	private bool isCountDownShow=false;
	/// <summary>
	/// 标记太阳月亮按钮的初始状态是显示太阳
	/// </summary>
	private bool preSunSwitchStatues = true;

	/// <summary>
	/// 声控开关闭合一次的总时间
	/// </summary>
	private float VOTime=5.5f;
	private float VOTimer=0;

	private Transform VoiceDelaySwitch;
	private Transform LAswitch;
	private Transform micphoneBtn;
	private Transform sunMoonBtn;
	private Transform microphoneBtn;

	private UISprite voiceDelaySprite;
	private UISprite LAswitchSprite;

	private PhotoRecognizingPanel photoRecognizePanel;


	//如果玩家先点击了声控开关，声控开关闭合，再点击太阳月亮，光敏开关闭合，线路虽然连通了，也是不行的
	//只有先闭合光敏，再闭合声控，电路才通
	// to do ...
	void OnEnable ()
	{
		isLevelFourteen = false;
		isLAswitchOn = false;
		changeTime = 3f;
		changeTimer = 0;
		isVOswitchOn=false;
		isFingerShow = false;
		isNightModeOnce=false;
		isFingerDestroyed=false;
		isStartRecord = false;
		isCountDownShow=false;
		VOTime = 5f;
		VOTimer = 0;


		VoiceDelaySwitch = transform.Find ("voiceTimedelaySwitch");
		LAswitch = transform.Find ("lightActSwitch");
		micphoneBtn = transform.Find ("MicroPhoneBtn");
		nightBg = PhotoRecognizingPanel._instance.nightMask;
		sunMoonBtn=transform.Find("SunAndMoonWidget");
		microphoneBtn=transform.Find ("MicroPhoneBtn");

		voiceDelaySprite=VoiceDelaySwitch.GetComponent<UISprite>();
		LAswitchSprite=LAswitch.GetComponent<UISprite>();

		photoRecognizePanel=PhotoRecognizingPanel._instance;

	}

	//声控+光敏    （只有晚上有声音的时候灯才亮）
	//需要伴随话筒按钮，太阳/月亮按钮出现
	//在太阳月亮按钮出现小手，点击后变成月亮，小手消失，背景渐变暗，光敏开关在背景变暗之前闭合；
	//在话筒按钮出现小手，点击后小手消失，弹出提示框提示玩家发出声音，收集到声音后提示框消失，声控开关闭合



	void Update () 
	{
		if (isLevelFourteen) 
		{
			if (!isFingerShow) 
			{
				photoRecognizePanel.ShowFinger(sunMoonBtn.localPosition);
				isFingerShow=true;
			}

			if (sunMoonBtn.GetComponent<MoonAndSunCtrl> ().isDaytime && !preSunSwitchStatues)//如果是白天，之前的按钮是月亮，说明是从晚上切换到白天
			{
				micphoneBtn.GetComponent<MicroPhoneBtnCtrl> ().isCollectVoice=false;
				isStartRecord = false;
				VOTime = 5f;
			}
			#region 如果是晚上（点击了太阳按钮）
			if (!sunMoonBtn.GetComponent<MoonAndSunCtrl> ().isDaytime) 
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
				}
				if (isLAswitchOn)//如果光敏开光闭合了
				{	
					GetImage._instance.cf.switchOnOff (int.Parse (LAswitch.gameObject.tag), true);
					LAswitchSprite.spriteName = "LAswitchOn";
					//只有晚上光敏开关闭合的时候，小话筒才可以被点击
					microphoneBtn.gameObject.GetComponent<BoxCollider>().enabled=true;
					microphoneBtn.gameObject.GetComponent<UIButton>().enabled=true;
					microphoneBtn.gameObject.GetComponent<MicroPhoneBtnCtrl>().enabled=true;
					photoRecognizePanel.ShowFinger (microphoneBtn.localPosition);//在话筒按钮出现小手
				}
				if (micphoneBtn.GetComponent<MicroPhoneBtnCtrl> ().isCollectVoice)//点击了话筒按钮
				{ 
					if (photoRecognizePanel.finger) {
						Destroy (photoRecognizePanel.finger);	//小手消失
					}

					if (!isStartRecord) //开始收集声音
					{ 
						photoRecognizePanel.noticeToMakeVoice.SetActive (true);//弹出提示框
						photoRecognizePanel.voiceCollectionMark.SetActive(true);//弹出声音收集图片
						photoRecognizePanel.voiceCollectionMark.transform.Find ("Wave").GetComponent<MyAnimation> ().canPlay = true;//显示声音收集动画
						MicroPhoneInput.getInstance ().StartRecord ();
						isStartRecord = true;
					}
					if (CommonFuncManager._instance.isSoundLoudEnough ()) //收集到声音
					{
						//声控延时开关闭合，倒计时文字出现并开始倒计时

						if (!isCountDownShow) 
						{
							StartCoroutine(CountDown());
							isCountDownShow=true;
						}

						isVOswitchOn=true;
						photoRecognizePanel.noticeToMakeVoice.SetActive (false);//提示框消失
						photoRecognizePanel.voiceCollectionMark.transform.Find ("Wave").GetComponent<MyAnimation> ().canPlay = false;
						photoRecognizePanel.voiceCollectionMark.SetActive(false);

						MicroPhoneInput.getInstance().StopRecord();
						GetImage._instance.cf.switchOnOff (int.Parse (VoiceDelaySwitch.gameObject.tag), true);
						voiceDelaySprite.spriteName = "VoiceDelayOn";
					}
					if (isVOswitchOn) 
					{
						VOTimer += Time.deltaTime;
						if (VOTimer >= VOTime) 
						{
							//声控延时开关断开，灯变暗
							VOTimer = 0;
							GetImage._instance.cf.switchOnOff (int.Parse (VoiceDelaySwitch.gameObject.tag), false);
							voiceDelaySprite.spriteName = "VoiceDelayOff";
							microphoneBtn.GetComponent<MicroPhoneBtnCtrl> ().isCollectVoice = false;
							isStartRecord = false;
							isCountDownShow=false;
						}
					}
				}
				CommonFuncManager._instance.CircuitItemRefresh (GetImage._instance.itemList);
			}
			#endregion
			#region 如果是白天
			else 
			{
				if (isNightModeOnce)//白天已经切换到过黑夜了
				{
					microphoneBtn.gameObject.GetComponent<BoxCollider>().enabled=false;
					microphoneBtn.gameObject.GetComponent<MicroPhoneBtnCtrl>().enabled=false;

					changeTimer -= Time.deltaTime;
					if (changeTimer <= 0) 
					{
						changeTimer =0;
					}
					nightBg.alpha = Mathf.Lerp (0, 1f, changeTimer / changeTime);
					if (changeTimer <= changeTime* 1/6) 
					{
						isLAswitchOn = false;
						GetImage._instance.cf.switchOnOff (int.Parse (LAswitch.gameObject.tag), isLAswitchOn);
						LAswitchSprite.spriteName ="LAswitchOff";

						isVOswitchOn=false;
						GetImage._instance.cf.switchOnOff (int.Parse (VoiceDelaySwitch.gameObject.tag), isVOswitchOn);
						voiceDelaySprite.spriteName = "VoiceDelayOff";
					}
					CommonFuncManager._instance.CircuitItemRefresh (GetImage._instance.itemList);	
				}
			}
			#endregion

			preSunSwitchStatues = sunMoonBtn.GetComponent<MoonAndSunCtrl>().isDaytime;

			CommonFuncManager._instance.ArrowsRefresh(GetImage._instance.itemList);
		}
	}



	IEnumerator CountDown()
	{
		UILabel countDown = PhotoRecognizingPanel._instance.countDownLabel;
		countDown.gameObject.SetActive(true);
		countDown.gameObject.transform.localPosition = VoiceDelaySwitch.localPosition;

		//倒计时，每个数字停留一秒后变化
		yield return new WaitForSeconds(1);
		countDown.text = "4";
		yield return new WaitForSeconds (1);
		countDown.text = "3";
		yield return new WaitForSeconds (1);
		countDown.text = "2";
		yield return new WaitForSeconds (1);
		countDown.text = "1";
		yield return new WaitForSeconds (1);
		countDown.gameObject.SetActive (false);
		countDown.text = "5";
	}



}
