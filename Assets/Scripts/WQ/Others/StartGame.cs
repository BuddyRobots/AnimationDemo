﻿using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		
		PanelTranslate.Instance.GetPanel(Panels.PhotoTakingPanel);
		PanelTranslate.Instance.DestoryThisPanel();
	}
	

}
