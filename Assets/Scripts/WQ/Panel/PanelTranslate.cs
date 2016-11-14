using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Panels
{
	None,
	StartPanel,
	LevelSelectedPanel,
	LevelDescriptionPanel,
	PhotoTakingPanel,
	PhotoRecognizedPanel,
	DemoShowPanel
}
/// <summary>
/// 管理界面相互跳转的类
/// </summary>
public class PanelTranslate : SceneSinglton<PanelTranslate> {

	private Transform UIRoot
	{
		get
		{
			return GameObject.Find("UI Root").transform;
		}
	}
		

	private Stack<GameObject> panels = new Stack<GameObject>();//用栈来存储当前显示的界面
	private GameObject prePanel = null;

	public GameObject GetPanel(Panels panel, bool isDeleteThisPanel = true)
	{
		if (panel == Panels.None) {
			return null;
		}
		GameObject ret ;
		if (!(ret = Instantiate(GetResourceGameObject(panel)))) {
			return null;
		}

		ret.transform.parent = UIRoot;
		ret.transform.localScale = Vector3.one;
		ret.transform.localPosition = Vector3.zero;

		//如果isDeleteThisPanel为true（表示需要销毁之前的对象，只显示当前的对象），就把栈中已经有的对象先弹出，再把当前需要显示的对象入栈；如果为false，则表示不需要销毁之前的对象，当前的对象直接入栈
		if (isDeleteThisPanel) {
			if (panels.Count > 0) {
				prePanel = panels.Pop();
			}
		}
		panels.Push(ret);
		return ret;
	}
	public void DestoryThisPanel()
	{
		if (prePanel) {
			Destroy(prePanel);
			prePanel = null;
		}
	}
	public void DestoryAllPanel()
	{
		if (prePanel) {
			Destroy(prePanel);
		}
		GameObject temp = panels.Pop();
		while (panels.Count > 0) {
			Destroy(panels.Pop());
		}
		panels.Push(temp);
	}

	private GameObject GetResourceGameObject(Panels panel)
	{
		GameObject go;
		string path = "Prefabs/Panel/";
		switch (panel) {
//		case Panels.StartPanel:
//			go = Resources.Load<GameObject>(path + "StartPanel");
//			break;
//		case Panels.LevelSelectedPanel:
//			go = Resources.Load<GameObject>(path + "LevelSelectPanel");
//			break;
//		case Panels.LevelDescriptionPanel:
//			go = Resources.Load<GameObject>(path + "DescriptionPanel");
//			break;
		case Panels.PhotoTakingPanel:
			go = Resources.Load<GameObject>(path + "PhotoTakingPanel");
			break;
		case Panels.PhotoRecognizedPanel:
			go = Resources.Load<GameObject>(path + "PhotoRecognizingPanel");
			break;
//		case Panels.DemoShowPanel:
//			go = Resources.Load<GameObject>(path + "DemoShowPanel");
//			break;
		default:
			go = null;
			break;
		}
		return go;
	}
}
