using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChangeSceneBtn : MonoBehaviour {
	GameObject btn;

	// Use this for initialization
	void Start () {
		btn=this.gameObject;
		UIEventListener.Get(btn).onClick=OnChangeSceneBtnClick;
	}
	
	void OnChangeSceneBtnClick(GameObject btn)
	{
		SceneManager.LoadScene("Test_0");

	}
}
