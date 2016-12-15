using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SliderCtrlManager : MonoBehaviour {

	[HideInInspector]
	public List<int> sliderValueList=new List<int>();


	public Slider h_min;
	public Slider h_max;
	public Slider s_min;
	public Slider s_max;
	public Slider u_min;
	public Slider u_max;

	public Text h_min_text;
	public Text h_max_text;
	public Text s_min_text;
	public Text s_max_text;
	public Text u_min_text;
	public Text u_max_text;

	public GameObject btn;

	void Start()
	{
		for (int i = 0; i < 5; i++) {
			sliderValueList.Add(0);
		}
		UIEventListener.Get(btn).onClick=OnBtnClick;
	}

	public void Set_H_min_Value()
	{

		h_min_text.text=((int)h_min.value).ToString();
	}
	public void Set_H_max_Value()
	{

		h_max_text.text=((int)h_max.value).ToString();
	}
	public void Set_S_min_Value()
	{

		s_min_text.text=((int)s_min.value).ToString();
	}
	public void Set_S_max_Value()
	{

		s_max_text.text=((int)s_max.value).ToString();
	}
	public void Set_U_min_Value()
	{

		u_min_text.text=((int)u_min.value).ToString();
	}
	public void Set_U_max_Value()
	{

		u_max_text.text=((int)u_max.value).ToString();
	}


	
	void OnBtnClick(GameObject go)
	{
//		int.TryParse(h_min_text.text,out sliderValueList[0]);
//		int.TryParse(h_max_text.text,out sliderValueList[1]);
//		int.TryParse(s_min_text.text,out sliderValueList[2]);
//		int.TryParse(s_max_text.text,out sliderValueList[3]);
//		int.TryParse(u_min_text.text,out sliderValueList[4]);
//		int.TryParse(u_max_text.text,out sliderValueList[5]);

//		int.Parse(h_min_text.text);

		sliderValueList.Add(int.Parse(h_min_text.text));
		sliderValueList.Add(int.Parse(h_max_text.text));
		sliderValueList.Add(int.Parse(s_min_text.text));
		sliderValueList.Add(int.Parse(s_max_text.text));
		sliderValueList.Add(int.Parse(u_min_text.text));
		sliderValueList.Add(int.Parse(u_max_text.text));


		for (int i = 0; i < sliderValueList.Count; i++) {
			Debug.Log("sliderValueList["+i+"]=="+sliderValueList[i]);
		}

	}
}
