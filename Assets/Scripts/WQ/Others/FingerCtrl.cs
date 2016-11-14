using UnityEngine;
using System.Collections;

public class FingerCtrl : MonoBehaviour 
{
	private float speed = 4;
	//private bool isMove;
	// GameObject fingerPrefab;
	//开关坐标--（ 17， 211， 0）
	//POSTo----（ 63， 171，0）
	//POSFrom---（130，103，0）
	//手指出现的位置到开关的偏移距离-----posFrom-switchPos  = （113,-108,0）
	//手指需要移动的距离--------posFrom-posTo  = (67,-68,0)

	Vector3 itemPos=new Vector3(17, 211, 0);
	Vector3 posTo = new Vector3 (64, 171, 0);
	Vector3 posFrom = new Vector3 (130,103,0);


	//private Vector3 offSet = new Vector3 (113, -108, 0);//手指出现的位置到开关的偏移距离
	//private Vector3 moveOffset = new Vector3 (67, -68, 0);//手指需要移动的距离
	//private Vector3 direction;
	private Vector3 dest;
	private Vector3 startPos;
	Vector3 moveOffset;

	private Vector3 dir;

	void Start () 
	{
		speed = 4f;
		//dest = Vector3.zero;
		//startPos = Vector3.zero;


	}

	void OnEnable()
	{
		//isMove = false;
		moveOffset=new Vector3((posFrom-posTo).x,(posFrom-posTo).y,0);
	} 

	//出现手指，需要传入坐标
	public void FingerShow(Vector3 fingerPos)
	{
		transform.localPosition = fingerPos;
		startPos=fingerPos;
		dest =startPos - moveOffset;
		//dest = startPos + new Vector3(-100,100,0);
		StartCoroutine (FingerMove(startPos, dest));
	}


	IEnumerator FingerMove(Vector3 start, Vector3 end) 
	{

		while (Vector3.Distance(transform.localPosition , end) > 1f) 
		{
			transform.localPosition = Vector3.Lerp (transform.localPosition, end, speed * Time.deltaTime);
			yield return new WaitForFixedUpdate ();
		}
		//yield return new WaitForSeconds (0.5f);

		while (Vector3.Distance (transform.localPosition, start) > 1f) 
		{
			transform.localPosition = Vector3.Lerp (transform.localPosition, start, speed * Time.deltaTime);
			yield return new WaitForFixedUpdate ();
		}
		StartCoroutine (FingerMove (start, end));//协同递归，保证一直移动
	}

}
