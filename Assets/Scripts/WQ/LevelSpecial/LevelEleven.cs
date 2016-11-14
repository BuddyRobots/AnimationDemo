using UnityEngine;
using System.Collections;
using MagicCircuit;

//level 11
public class LevelEleven : MonoBehaviour 
{
	[HideInInspector]
	public bool isVOswitchOccur=false;
	private bool isAnimationPlay=false;
	private bool isStartRecord = false;
	/// <summary>
	/// 保证声音收集一次的标志
	/// </summary>
	private bool stayForAwhile = false;

	private PhotoRecognizingPanel photoRecognizePanel;
	private MicroPhoneBtnCtrl microBtnCtrl;
	private Transform voiceSwitch;

	void OnEnable () 
	{
		isVOswitchOccur=false;
		isAnimationPlay=false;
		isStartRecord = false;
		stayForAwhile = false;

		voiceSwitch=transform.Find("voiceOperSwitch");
		photoRecognizePanel=PhotoRecognizingPanel._instance;
		microBtnCtrl=transform.Find ("MicroPhoneBtn").GetComponent<MicroPhoneBtnCtrl> ();
		transform.Find ("MicroPhoneBtn").localPosition=new Vector3(272,338,0);//把小话筒摆在右上角的位置，使界面美观
	}


	void Update () 
	{
		if (isVOswitchOccur) 
		{
				//在话筒按钮出现小手
			photoRecognizePanel.ShowFinger(transform.Find("MicroPhoneBtn").localPosition);
			//点击话筒按钮，
			if (microBtnCtrl.isCollectVoice) 
			{
				if (photoRecognizePanel.finger) 
				{
					Destroy (photoRecognizePanel.finger);	
				}
				if (!isStartRecord) 
				{  
					photoRecognizePanel.noticeToMakeVoice.SetActive(true);//弹出提示框
					photoRecognizePanel.voiceCollectionMark.SetActive(true);//弹出声音收集图片
					photoRecognizePanel.voiceCollectionMark.transform.Find ("Wave").GetComponent<MyAnimation> ().canPlay = true;//显示声音收集动画
					MicroPhoneInput.getInstance().StartRecord();//收集声音
					isStartRecord = true;
				}
				//收集到声音后，播放声音收集完成音效，提示框消失
				if (CommonFuncManager._instance.isSoundLoudEnough ()) 
				{
					isAnimationPlay = true;
					photoRecognizePanel.noticeToMakeVoice.SetActive (false);
					photoRecognizePanel.voiceCollectionMark.transform.Find ("Wave").GetComponent<MyAnimation> ().canPlay = false;
					photoRecognizePanel.voiceCollectionMark.SetActive (false);
					MicroPhoneInput.getInstance ().StopRecord ();
					GetImage._instance.cf.switchOnOff (int.Parse (voiceSwitch.gameObject.tag), true);
					voiceSwitch.GetComponent<UISprite>().spriteName="VOswitchOn";
					CommonFuncManager._instance.CircuitItemRefresh (GetImage._instance.itemList);	
				} 
				CommonFuncManager._instance.ArrowsRefresh(GetImage._instance.itemList);
			}	
		}
	}
}
