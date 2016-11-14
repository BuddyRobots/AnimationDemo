using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCircuit;

	
public class PhotoRecognizingPanel : MonoBehaviour 
{

	#region   公共变量
	public static PhotoRecognizingPanel _instance;

//	[HideInInspector]
//	public  float arrowGenInterval = 0.8f;
	[HideInInspector]
	public int transValue = 0;

	[HideInInspector]
	public Vector3 prePos= Vector3.zero; // 记录当前指向的位置，如果没发生变化，不做任何操作

	[HideInInspector]
	public GameObject noticeToMakeVoice;//发声提示框
	[HideInInspector]
	public GameObject voiceCollectionMark;//声音收集喇叭图片
	[HideInInspector]
	public GameObject finger;


	[HideInInspector]
	public UILabel countDownLabel;

	[HideInInspector]
	public UITexture nightMask;

	/// <summary>
	/// 判断箭头电流动画是否完成的标志
	/// </summary>
	[HideInInspector]
	public  bool isArrowShowDone=false;
	[HideInInspector]
	public bool isNeedToCreateArrow = true;
	[HideInInspector]
	public  bool isAllItemShowDone=false;
	[HideInInspector]
	public bool result;//匹配结果

	[HideInInspector]
	public List<GameObject> switchList = new List<GameObject> ();//开关的集合
	[HideInInspector]
	public List<CircuitItem> itemList=new List<CircuitItem>();//图标的集合
	[HideInInspector]
	public List< List<Vector3> > lines=new List<List<Vector3>>();//所有线条的集合
	[HideInInspector]
	public  List<GameObject> arrowList=new List<GameObject>();
	[HideInInspector]
	public  List<GameObject> batteryList = new List<GameObject> ();
	[HideInInspector]
	public  List<GameObject> bulbList = new List<GameObject> ();
	[HideInInspector]
	public List<List<Vector3>> circuitLines;
	[HideInInspector]
	public List<int> tags=new List<int>();
	#endregion


	#region  私有变量
	private LevelItemData data;

	private const float lineItemInterval = 0.05f;
	private const float itemInterval = 0.5f;
	private const float resultShowInterval=1.5f;

	/// <summary>
	/// 记录图标数量的信号量，为0时表示所有图标都显示完，可以显示箭头了
	/// </summary>
	private int iconCount = 1;
	private int voiceOperSwitchNum = 0;
	private int LightActSwitchNum = 0;
	private int VoiceDelaySwitchNum=0;

	private float distance = 0;
	private float angle = 0;
	private float maskTimer = 0f;//蒙板渐变计时器
	private float maskChangeTotalTime=0f;//蒙板渐变的总时间=所有item开始显示到显示完成的总时间

	private Vector3 fromPos;
	private Vector3 toPos;
	private Vector3 centerPos;
	private Vector3 offSet = new Vector3 (113, -108, 0);

	//  btns on the panel
	private GameObject helpBtn;
	private GameObject replayBtn;
	private GameObject nextBtn;

	//  items need to show
	private GameObject bulb;
	private GameObject battery;
	private GameObject switchBtn;
	private GameObject loudspeaker;
	private GameObject voiceOperSwitch;
	private GameObject lightActSwitch;
	private GameObject voiceTimedelaySwitch;
	private GameObject doubleDirSwitch;
	private GameObject inductionCooker;

	private GameObject sunAndMoon;
	private GameObject microPhone;
	private GameObject labelBgTwinkle;//文字显示的发光背景
	private GameObject lineParent;
	private GameObject linePrefab;
	private GameObject fingerPrefab;
	private GameObject arrowPrefab;

	private UILabel levelNameLabel;

	private UITexture photoImage;//拍摄截取的图像
	private UITexture dayMask;//遮盖背景图片的蒙板，通过改变透明度来显示拍摄的照片
	private UITexture successShow;//welldone 界面
	private UITexture failureShow;//failure界面

	/// <summary>
	/// 保证创建图标的协同只走一遍的标志
	/// </summary>
	private bool isAllItemCreatedOnlyOnce=false;
	private bool isArrowCreated=false;// mark to judge whether arrows are created or not
	private bool isCreateArrowOnSingleLine=false;
	private bool isResultPanelShow = false;// 结果显示界面只打开一次的标志
	private bool isPhotoImageShowDone = false;
	private bool isMaskChangeGradual = false;//渐变时间开关
	private bool isLevelHandleScriptAdd=false;

	/// <summary>
	/// 识别面板新创建出来的对象的集合（界面上新创建的对象需要在关闭面板的时候进行销毁，以保证重新打开界面时上一次操作中的新建的对象不会残留）
	/// </summary>
	private List<GameObject> needToBeDestroyedList=new List<GameObject>();
	#endregion

	void Awake()
	{
		_instance = this;
	}

	void Start()
	{
		helpBtn = transform.Find ("HelpBtn").GetComponent<UIButton> ().gameObject;

		bulb = Resources.Load ("Prefabs/Items/Bulb",typeof(GameObject))  as GameObject;
		battery = Resources.Load ("Prefabs/Items/Battery",typeof(GameObject))  as GameObject;
		switchBtn = Resources.Load ("Prefabs/Items/Switch",typeof(GameObject))  as GameObject;
		loudspeaker = Resources.Load ("Prefabs/Items/Loudspeaker", typeof(GameObject)) as GameObject;
		voiceOperSwitch = Resources.Load ("Prefabs/Items/VoiceOperSwitch", typeof(GameObject)) as GameObject;
		lightActSwitch = Resources.Load ("Prefabs/Items/LightActSwitch", typeof(GameObject)) as GameObject;
		voiceTimedelaySwitch = Resources.Load ("Prefabs/Items/VoiceTimedelaySwitch", typeof(GameObject)) as GameObject;
		doubleDirSwitch = Resources.Load ("Prefabs/Items/DoubleDirSwitch", typeof(GameObject)) as GameObject;
		inductionCooker = Resources.Load ("Prefabs/Items/InductionCooker", typeof(GameObject)) as GameObject;

		arrowPrefab=Resources.Load("Prefabs/Items/Arrow") as GameObject;
		linePrefab = Resources.Load ("Prefabs/Items/lineNew") as GameObject;
		fingerPrefab= Resources.Load ("Prefabs/Items/Finger",typeof(GameObject)) as GameObject;

		UIEventListener.Get (helpBtn).onClick = OnHelpBtnClick;
		UIEventListener.Get (replayBtn).onClick = OnReplayBtnClick;
		UIEventListener.Get (nextBtn).onClick = OnNextBtnClick;


	}

		
	void OnEnable()
	{
		//添加LevelHandle脚本并激活
		if (!gameObject.GetComponent<LevelHandle>()) 
		{
			gameObject.AddComponent<LevelHandle>();
			gameObject.GetComponent<LevelHandle>().enabled=false;
		}
		else
		{
			gameObject.GetComponent<LevelHandle>().enabled=false;
		}

//		HomeBtn.Instance.panelOff = PanelOff;
		data = LevelManager.currentLevelData;
		lineParent = this.gameObject;

		levelNameLabel = transform.Find ("LevelNameBg/Label").GetComponent<UILabel> ();
		levelNameLabel.text = "识别中";//LevelManager.currentLevelData.LevelName;
		countDownLabel = transform.Find ("CountDownLabel").GetComponent<UILabel> ();
		photoImage =transform.Find ("Bg/PhotoImage").GetComponent<UITexture> ();//real code 
		replayBtn=transform.Find("ReplayBtn").gameObject;
		nextBtn=transform.Find("NextBtn").gameObject;
		labelBgTwinkle = transform.Find ("LevelNameBgT").gameObject;
		noticeToMakeVoice = transform.Find ("VoiceNotice").gameObject;
		voiceCollectionMark = transform.Find ("MicrophoneAniBg").gameObject;
		sunAndMoon = transform.Find ("SunAndMoonWidget").gameObject;
		microPhone = transform.Find ("MicroPhoneBtn").gameObject;
		successShow = transform.Find ("WellDoneBg").GetComponent<UITexture> ();
		failureShow=transform.Find ("FailureBg").GetComponent<UITexture> ();
		dayMask = transform.Find ("Bg/DayBgT").GetComponent<UITexture> ();
		nightMask = transform.Find ("Bg/NightBgT").GetComponent<UITexture> ();

		photoImage.gameObject.SetActive (false);
		replayBtn.SetActive(false);
		nextBtn.SetActive (false);
		labelBgTwinkle.SetActive (false);
		noticeToMakeVoice.SetActive (false);
		voiceCollectionMark.SetActive (false);
		sunAndMoon.SetActive (false);
		microPhone.SetActive (false);
		countDownLabel.gameObject.SetActive(false);
		successShow.gameObject.SetActive (false);
		failureShow.gameObject.SetActive (false);
		dayMask.gameObject.SetActive (true);
		nightMask.gameObject.SetActive (true);

		isAllItemShowDone=false;
		isPhotoImageShowDone = false;
		isArrowShowDone=false;
		isAllItemCreatedOnlyOnce=false;
		isArrowCreated=false;
		isCreateArrowOnSingleLine=false;
		isResultPanelShow = false;
		isLevelHandleScriptAdd=false;
		isMaskChangeGradual = false;

		prePos = Vector3.zero; 

		dayMask.alpha = 0;
		nightMask.alpha = 0;
		voiceOperSwitchNum = 0;
		LightActSwitchNum = 0;
		VoiceDelaySwitchNum = 0;
		maskTimer = 0;
		maskChangeTotalTime = 0;
//		arrowGenInterval = 0.8f;
		transValue = 0;
		iconCount = 1;

		circuitLines=new List<List<Vector3>>();

		StartCoroutine (ShowPhotoImage ());//进入识别界面的第一步是显示拍摄的照片
		StartCoroutine (RemoveEmptyArrow ());
	}
		
	IEnumerator ShowPhotoImage()// 显示拍摄的图片
	{
		photoImage.gameObject.SetActive (true);
		photoImage.mainTexture = GetImage._instance.texture;
		yield return new WaitForSeconds (1f);
		isPhotoImageShowDone = true;
	}

	IEnumerator RemoveEmptyArrow()
	{
		while (true) 
		{
			for (int i = 0; i < arrowList.Count; ) 
			{
				if (arrowList[i]) 
				{
					i++;
				} 
				else 
				{
					arrowList.RemoveAt (i);
				}
			}
			yield return new WaitForSeconds (itemInterval);
		}
	}
		
	void Update () 
	{
		if ( GetImage._instance.isThreadEnd) //如果数据处理完了，还没有取数据，就取数据
		{
			itemList=GetImage._instance.itemList;
			result = GetImage._instance.isCircuitCorrect;
			if (!isMaskChangeGradual) 
			{
				GetCircuitLines ();
				maskChangeTotalTime = (itemList.Count-1)  * itemInterval;//显示图标的总时间=(图标个数-1)*图标间隔时间
				foreach (var item in circuitLines) 
				{
					maskChangeTotalTime += (float)((item.Count - 1) * lineItemInterval);//显示一条线的总时间
				}
				isMaskChangeGradual = true;
			}
			if (!isArrowCreated) 
			{
				CreateArrow ();
				isArrowCreated = true;
			}
		} 
		else 
		{
			return;
		}
		//如果图片显示完了，且取到了数据，就进行后面的显示
		if (isPhotoImageShowDone )// && GetImage._instance.isThreadEnd
		{
			#region 蒙板出现，透明度渐变
			maskTimer += Time.deltaTime;
			if (maskTimer >= maskChangeTotalTime) 
			{
				maskTimer =maskChangeTotalTime;
			}
			dayMask.alpha=Mathf.Lerp (0, 1f, maskTimer/maskChangeTotalTime);
			#endregion
			if(isAllItemCreatedOnlyOnce==false)//当一个函数要放在update里面时， 又要保证只执行一次，可以在这个函数之前加一个bool值来标志
			{
				StartCoroutine (CreateAllItems ());//创建图标
				isAllItemCreatedOnlyOnce =true;
			}
			if (iconCount == 0) 
			{
				isAllItemShowDone = true;
			}
			#region 图标都显示完以后，如果结果正确，就根据关卡的等级显示应该显示的按钮
			if (isAllItemShowDone && result) 
			{
				if (LevelManager.currentLevelData.LevelID==11)
				{
					microPhone.SetActive(true); 
				}
				else if (LevelManager.currentLevelData.LevelID==12) 
				{
					sunAndMoon.SetActive(true);
				}
				else if (LevelManager.currentLevelData.LevelID==13 ||LevelManager.currentLevelData.LevelID==14) 
				{
					microPhone.SetActive(true); 
					sunAndMoon.SetActive(true);
				}	
			}
			#endregion
			// 结果展示逻辑
			if (isAllItemShowDone && !isResultPanelShow)
			{
				if (result) 
				{
					WellDone ();
				}
				else
				{
					Fail ();
				}
				isResultPanelShow = true;
			}

			if (isAllItemShowDone && !isLevelHandleScriptAdd ) //如果图标都显示完了，就给面板添加相应的关卡脚本
			{
				//transform.gameObject.AddComponent<LevelHandle>();
				transform.gameObject.GetComponent<LevelHandle>().enabled=true;
				isLevelHandleScriptAdd = true;
			}
			//如果图标都显示完了且匹配成功，就根据关卡等级添加界面操作的脚本
			if (isAllItemShowDone && result)
			{
				LevelHandle._instance.CircuitHandleByLevelID (LevelManager.currentLevelData.LevelID);
			}
		}
	}

	/// <summary>
	/// 从所有的线中选一个随机点
	/// </summary>
	/// <returns>The random point.</returns>
	public Vector3 ChooseRandomPointOnLine()
	{
		int maxNum = 0;//点的最大数量
		int longestLineIndex = 0;//最长线段的下标
		for (int i = 0; i < lines.Count; i++) 
		{
			if (lines [i].Count > maxNum) 
			{
				maxNum=lines [i].Count;
				longestLineIndex = i;
			}	
		}
		//最长的线段是lines[longestLineIndex],该线段上的点的个数是maxNum；需要取这条线段的中点
		int index = 0;
		if (maxNum % 2 == 0) 
		{
			index = maxNum / 2;	
		} 
		else 
		{
			index = (maxNum + 1) / 2;
		}
		Vector3 pos = (lines [longestLineIndex]) [index];
		return pos;
	}
		
	public void ShowFingerOnLine(Vector3 pos)
	{
		StartCoroutine (ShowFingerOnLineT (pos));
	}

	IEnumerator ShowFingerOnLineT(Vector3 pos)//需要传一个线上的随机坐标
	{
		yield return new WaitForSeconds (3f);
		ShowFinger (pos);
	}		
		
	public void ShowFinger(Vector3 pos)
	{
		if (prePos == pos) 
		{
			return;
		}
		prePos = pos;
		if (finger) 
		{
			Destroy (finger);
			finger = null;
		}
		finger = Instantiate (fingerPrefab) as GameObject;
		finger.name="finger";
		finger.transform.parent = transform;
		finger.transform.localScale = Vector3.one;
		finger.GetComponent<FingerCtrl> ().FingerShow (pos + offSet);
	}
		
	IEnumerator CreateAllItems()
	{
		iconCount = itemList.Count;
		for (int i = 0; i < itemList.Count; i++) 
		{
			CreateSingleItem (itemList [i]);
			yield return new WaitForSeconds (itemInterval);//隔0.5秒创建一个图标
		}
	}
		
	public void CreateSingleItem(CircuitItem circuitItem)
	{
		GameObject item = null;
		switch (circuitItem.type) 
		{
		case ItemType.Battery:
			item = GameObject.Instantiate (battery) as GameObject;
			needToBeDestroyedList.Add (item);//新创建一个对象的同时把这个对象加入到对象列表，方便关闭界面的时候销毁这些新创建的对象
			batteryList.Add(item);
			item.name = "battery"; 
			item.tag = circuitItem.ID.ToString ();
			iconCount--;
			break;
	
		case ItemType.Bulb:
			item = GameObject.Instantiate (bulb) as GameObject;
			needToBeDestroyedList.Add (item);
			bulbList.Add (item);
			item.name = "bulb";
			item.tag = circuitItem.ID.ToString ();
			iconCount--;
			break;

		case ItemType.Switch:
			item = GameObject.Instantiate (switchBtn) as GameObject;
			switchList.Add (item);
			needToBeDestroyedList.Add (item);
			item.name = "switch"; 
			item.tag = circuitItem.ID.ToString ();
			iconCount--;
			break;

		case ItemType.SPDTSwitch:
			item = GameObject.Instantiate (doubleDirSwitch) as GameObject;
			switchList.Add (item);
			needToBeDestroyedList.Add (item);
			item.name = "doubleDirSwitch";
			item.tag = circuitItem.ID.ToString ();
			iconCount--;
			break;

		case ItemType.VoiceTimedelaySwitch:
			item = GameObject.Instantiate (voiceTimedelaySwitch) as GameObject;
			switchList.Add (item);
			needToBeDestroyedList.Add (item);
			item.name = "voiceTimedelaySwitch";
			item.tag = circuitItem.ID.ToString ();
			voiceOperSwitchNum++;
			VoiceDelaySwitchNum++;
			iconCount--;
			break;

		case ItemType.VoiceOperSwitch:
			item = GameObject.Instantiate (voiceOperSwitch) as GameObject;
			switchList.Add (item);
			needToBeDestroyedList.Add (item);
			item.name = "voiceOperSwitch";
			item.tag = circuitItem.ID.ToString ();
			voiceOperSwitchNum++;//声控开关出现的时候记录一下，话筒按钮需要伴随出现
			iconCount--;
			break;
		
		case ItemType.LightActSwitch:
			item = GameObject.Instantiate (lightActSwitch) as GameObject;
			switchList.Add (item);
			needToBeDestroyedList.Add (item);
			item.name = "lightActSwitch";
			item.tag = circuitItem.ID.ToString ();
			LightActSwitchNum++;//光敏开关出现的时候记录一下，太阳月亮切换按钮需要伴随出现
			iconCount--;
			break;

		case ItemType.Loudspeaker:
			item = GameObject.Instantiate (loudspeaker) as GameObject;
			needToBeDestroyedList.Add (item);
			item.name = "loudspeaker"; 
			item.tag = circuitItem.ID.ToString ();
			iconCount--;
			break;

		case  ItemType.InductionCooker:
			item = GameObject.Instantiate (inductionCooker) as GameObject;
			needToBeDestroyedList.Add (item);
			item.name = "inductionCooker";
			item.tag = circuitItem.ID.ToString ();
			iconCount--;
			break;

		case ItemType.CircuitLine:
			
			lines.Add (circuitItem.list);//如果是线路，则加入线路列表中，方便计算所有图标创建完的总时间
			StartCoroutine (DrawCircuit (circuitItem.list,circuitItem.ID));
			break;
		default:
			break;
		}
		if (item!=null) 
		{
			item.transform.parent = transform;
			item.transform.localPosition = circuitItem.list [0];// 如果测试用的坐标是根据localPosition设定的，就要用localPosition来接收
			item.transform.localScale = new Vector3 (1, 1, 1); 
			//根据图标数据的旋转角度进行旋转，旋转的是Z上的弧度
			item.transform.Rotate(new Vector3(0,0,(float)circuitItem.theta));
		}
	}
		
	/// <summary>
	/// 画线路itemList
	/// </summary>
	/// <param name="pos">线上点的集合</param>
	IEnumerator DrawCircuit(List <Vector3> pos, int lineID)
	{
		for (int i = 0; i < pos.Count - 1; i++)
		{

			DrawLineBetweenTwoPoints (pos [i], pos [i + 1], lineID);
			yield return new WaitForSeconds (lineItemInterval);//画一条线，隔0.1秒再画一条

		}
		iconCount--;
	}
		
	//两点之间画线，需要知道两点之间的距离，线段的中心点，以及角度------思想是把要显示的图片放在中心点的位置，然后把图片的宽度拉伸到和线段一样长，再依照角度旋转
	void DrawLineBetweenTwoPoints(Vector3 posFrom, Vector3 posTo, int lineID)
	{
		distance = Vector3.Distance (posFrom, posTo);
		centerPos = Vector3.Lerp (posFrom, posTo, 0.5f);
		angle = CommonFuncManager._instance.TanAngle (posFrom, posTo);
		GameObject lineGo = NGUITools.AddChild(lineParent, linePrefab);//生成新的连线 

		lineGo.tag=lineID.ToString();

		needToBeDestroyedList.Add(lineGo);
		UISprite lineSp = lineGo.GetComponent<UISprite>();//获取连线的 UISprite 脚本  
		lineSp.width = (int)(distance+6);//将连线图片的宽度设置为上面计算的距离  
		lineGo.transform.localPosition = centerPos;//设置连线图片的坐标 
		if (LevelManager.currentLevelData.LevelID==2) //只有第二关的线条才可以被点击消除，才需要碰撞体
		{
			lineGo.AddComponent<BoxCollider>();
			lineGo.GetComponent<BoxCollider>().size = lineGo.GetComponent<UISprite>().localSize;
		}

		lineGo.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);//旋转连线图片  
	}

	#region  箭头的生成和销毁
	public void StopCreateArrows()
	{
		isNeedToCreateArrow = false;
	}

	public void ContinueCreateArrows()
	{
		isNeedToCreateArrow = true;
	} 
	/// <summary>
	/// 电流停止移动
	/// </summary>
	public void StopCircuit()//先停止创建箭头，再隐藏箭头
	{
		StopCreateArrows ();
		foreach (var item in arrowList) 
		{
			if (item) 
			{
				item.GetComponent<UISprite> ().alpha = 0;
				item.GetComponent<MoveCtrl> ().Stop ();
			}
		}
	}
	/// <summary>
	/// 电流继续移动
	/// </summary>
	public void ContinueCircuit()//隐藏的箭头先出现，再继续创建箭头
	{
		foreach (var item in arrowList) 
		{
			if (item) 
			{
				item.GetComponent<UISprite> ().alpha = 1;
				item.GetComponent<MoveCtrl> ().ContinueStart ();
			}

		}
		ContinueCreateArrows ();
	}
		
	#endregion

	//获得线路上的所有线-----用来创建电流
	void GetCircuitLines()
	{
		for (int i = 0; i < itemList.Count; i++) 
		{
			if (itemList[i].type==ItemType.CircuitLine) 
			{

				circuitLines.Add (itemList[i].list);
				tags.Add (itemList[i].ID);//itemsList[i].ID equals to tag of arrow

			}
		}
	}


	/// <summary>
	/// create arrow around the circuit
	/// </summary>
	public void CreateArrow()
	{
		for (int i = 0; i < circuitLines.Count; i++) 
		{
			isCreateArrowOnSingleLine = true;
			if (isCreateArrowOnSingleLine) 
			{
				//create arrows on every single line
				List<Vector3> templine = new List<Vector3>();
				for (int n = 0; n < circuitLines[i].Count; n++) 
				{
					templine.Add (circuitLines[i] [n]);
				}

				StartCoroutine (CreateArrowOnSingleLine(templine,tags[i]));
			}
		}
	}

	IEnumerator CreateArrowOnSingleLine(List<Vector3> line,int tag)
	{
		isCreateArrowOnSingleLine = false;
		for (int k = 0;; k++) 
		{
			if (isNeedToCreateArrow) 
			{
				//从线的第一个点出箭头，一直往前移动，移动到最后一个销毁
				GameObject arrow = Instantiate (arrowPrefab) as GameObject;
				arrow.tag = tag.ToString ();
				arrow.GetComponent<UISprite> ().alpha =transValue;
				arrowList.Add (arrow);
				arrow.transform.parent = transform;
				arrow.name = "arrow";
				arrow.transform.localPosition = line [0];
				arrow.transform.localScale = Vector3.one;
				arrow.GetComponent<MoveCtrl> ().Move (line);

				yield return new WaitForSeconds (Constant.ARROW_GEN_INTERVAL);
			} 
			else 
			{
				yield return new WaitForFixedUpdate ();
			}
		}

	}

	public void Fail()
	{
		levelNameLabel.text="很遗憾...";
		labelBgTwinkle.SetActive (false);
		replayBtn.SetActive (true);//显示重玩按钮
		StartCoroutine (FailureShow ());

	}
		
	IEnumerator FailureShow()
	{
		yield return new WaitForSeconds (resultShowInterval);
		failureShow.gameObject.SetActive (true);//显示失败界面

	}

	public void WellDone()
	{
		levelNameLabel.text="祝贺你";
		labelBgTwinkle.SetActive (true);
		replayBtn.SetActive (true);
		nextBtn.SetActive (true);
		StartCoroutine (SuccessShow ());
	}

	IEnumerator SuccessShow()
	{
		yield return new WaitForSeconds (resultShowInterval);
		successShow.gameObject.SetActive (true);
	}
		
	void OnReplayBtnClick(GameObject btn)
	{
		PanelTranslate.Instance.GetPanel(Panels.PhotoTakingPanel );
		PanelTranslate.Instance.DestoryAllPanel();
	}
		
	void OnNextBtnClick(GameObject btn)
	{
		//记录通关的关卡
		if(data.Progress != LevelProgress.Done)
		{
			PlayerPrefs.SetInt ("LevelID",data.LevelID);
			PlayerPrefs.SetInt ("LevelProgress",2);
			LevelManager._instance.LoadLocalLevelProgressData ();
		}
		PanelTranslate.Instance.GetPanel(Panels.LevelSelectedPanel);

		PanelOff();
	}

	void OnHelpBtnClick(GameObject btn)
	{
		PlayerPrefs.SetInt("toDemoPanelFromPanel",4);
		PanelTranslate.Instance.GetPanel(Panels.DemoShowPanel,false);
		GameObject.Find("UI Root/DemoShowPanel(Clone)/DemoPic").GetComponent<HelpDataShow>().InitFromLevel();
	
	}

	public void PanelOff()
	{
		if (needToBeDestroyedList.Count>0) 
		{
			for (int i = 0; i < needToBeDestroyedList.Count; i++) //销毁创建的对象，保证再次打开该界面时是最初的界面，如果不销毁的话重新打开时上一次创建的对象会出现在界面
			{
				Destroy (needToBeDestroyedList [i]);
			}
			
		}

		for (int i = 0; i < arrowList.Count; i++) 
		{
			Destroy (arrowList [i]);
		}

		for (int i = 0; i < lines.Count; i++) 
		{
			lines[i].Clear();
		}
		lines.Clear ();

		for (int i = 0; i < circuitLines.Count; i++) 
		{
			circuitLines[i].Clear();
		}
		circuitLines.Clear ();
		PanelTranslate.Instance.DestoryAllPanel();

		itemList.Clear();
	}

	void OnDisable()
	{
		if (finger) 
		{
			Destroy (finger);
			finger = null;
		}
		switchList.Clear ();
		bulbList.Clear ();
		batteryList.Clear ();
		Destroy (transform.GetComponent<LevelHandle> ());
	}
}



