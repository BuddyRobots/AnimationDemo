using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class BtnClick_Test : MonoBehaviour {

	GameObject btn;
	GameObject tex;

	// Use this for initialization
	void Start () 
	{
		btn=this.gameObject;
		tex=transform.parent.Find("Tex").gameObject;
		UIEventListener.Get(btn).onClick=OnBtnClick;
	
	}
	
	void OnBtnClick(GameObject btn)
	{
		switch(btn.name)
		{

		case "DestroyBtn":
			Debug.Log("OnBtnClick");
			if (tex) 
			{
				Destroy(tex);

			}

			break;
		case "HideBtn":
			if (tex!=null) 
			{
				tex.SetActive(false);
			}

			break;
		case "ShowBtn":
			if (tex) 
			{
				tex.SetActive(true);
			}

			break;

		case "GCBtn":
//			GC.Collect();
//			Debug.Log("GCBtnClick----------GC collect done");
			SceneManager.LoadScene("Test_1");
			break;

		default:
			break;
		}



	}
}
