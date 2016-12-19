using UnityEngine;
using System.Collections;

/// <summary>
/// 该脚本用来控制播放帧动画
/// </summary>
public class MyAnimation : MonoBehaviour {
	public string atlasPath = "";
	public string name = "";
	public int pictureNumber;
	public float intervalTime = 0.064f;
	[Range(0.1f,10)]
	public float speed;
	public bool isLoop;
	//public bool isStartPlay;


	private UIAtlas atlas;
	private UISprite sprite;
	private int currentID;
	private float time;
	public bool canPlay;

	void Start()
	{
		atlas = Resources.Load<GameObject> (atlasPath).GetComponent<UIAtlas> ();
		currentID = 0;
		sprite = GetComponent<UISprite> ();
		sprite.atlas = atlas;

		//控制是否一开始就播放
		//canPlay = isStartPlay;
		time = Time.time;
	}
	void OnEnable()
	{
		canPlay = false;
		//canPlay=true;

	}

	void Update()
	{
		if (canPlay) 
		{
			if (isLoop) 
			{
				if (Time.time >= time + intervalTime / speed) 
				{
					time = Time.time;
					sprite.spriteName = name + currentID % pictureNumber;
					currentID++;
				}

			}
			else 
			{
				if (currentID < pictureNumber) 
				{
					if (Time.time >= time + intervalTime / speed) 
					{
						time = Time.time;
						sprite.spriteName = name + currentID;
						currentID++;
					}
				}
			}
		} 
		else 
		{
			time = Time.time;
			currentID = 0;
			sprite.spriteName = name + currentID;
		}

	}

	public void Play()
	{
		canPlay = true;
		time = Time.time;
		currentID = 0;
	}

	public void ContinuePlay()
	{
		canPlay = true;
		time = Time.time;
	}

	public void PausePlay()
	{
		canPlay = false;
	}
}
