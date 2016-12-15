using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class EnterGame : MonoBehaviour 
{
	private GameObject manager;

	void Start () 
	{
		manager=GameObject.Find("Manager");
		SceneManager.LoadScene("scene_PhotoTaking");
		GameObject.DontDestroyOnLoad(manager);
	
	}

}
