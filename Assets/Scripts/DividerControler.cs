using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DividerControler : MonoBehaviour
{
	public float Xspeed = 300;
	public float Yspeed = 300;
	public float smoothingSpeed = 10;
	public float xSpeedLimit = 20;
	public float ySpeedLimit = 20;
	public Space rotatingSpace = Space.Self;
	private float xsmooth = 0;
	private float ysmooth = 0;
	public GameObject targetObject;
	public string TargetObjectName = "";
	//public RectTransform CanvasRect;
	//public Transform ancher;
	//public Vector3 pos1;
	//private Vector2 res;
	//public GameObject obj;
	// Use this for initialization
	void Start ()
	{
		if (targetObject == null)
			targetObject = GameObject.Find (TargetObjectName);
		//res = new Vector2 (Screen.width, Screen.height);
		//CanvasRect = GameObject.FindObjectOfType<Canvas> ().GetComponent<RectTransform> ();
		//if (obj != null) {
		//	pos1 = Camera.allCameras [0].WorldToViewportPoint (obj.transform.position);
		//	pos1 = new Vector3 (
		//		((pos1.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
		//		((pos1.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)), 100);
		//	Resized ();
		//}
	}

	// Update is called once per frame
	void Update ()
	{
		if (xsmooth > 0 || ysmooth > 0) {
			targetObject.transform.Rotate (ysmooth, xsmooth, 0, rotatingSpace);
			xsmooth -= smoothingSpeed * Time.deltaTime;
			ysmooth -= smoothingSpeed * Time.deltaTime;
			if (xsmooth < 0)
				xsmooth = 0;
			if (ysmooth < 0)
				ysmooth = 0;

		}
		if (xsmooth < 0 || ysmooth < 0) {
			targetObject.transform.Rotate (ysmooth, xsmooth, 0, rotatingSpace);
			xsmooth += smoothingSpeed * Time.deltaTime;
			ysmooth += smoothingSpeed * Time.deltaTime;
			if (xsmooth > 0)
				xsmooth = 0;
			if (ysmooth > 0)
				ysmooth = 0;
		}
		//if (res.x != Screen.width || res.y != Screen.height) {
		//	Resized ();
		//	res = new Vector2 (Screen.width, Screen.height);
		//}
		// 
	}

	public void OnDrag (BaseEventData e)
	{
		xsmooth = Input.GetAxis ("Mouse X") * Time.deltaTime * Xspeed * -1;
		ysmooth = Input.GetAxis ("Mouse Y") * Time.deltaTime * Yspeed * -1;
		if (Input.touchCount > 0) {
			xsmooth = Input.touches [0].deltaPosition.x * Time.deltaTime * Xspeed / 3;
			ysmooth = Input.touches [0].deltaPosition.y * Time.deltaTime * Yspeed / 3;
		}
		xsmooth = Mathf.Clamp (xsmooth, xSpeedLimit * -1, xSpeedLimit);
		ysmooth = Mathf.Clamp (ysmooth, ySpeedLimit * -1, ySpeedLimit);
		//	targetObject.transform.Rotate (Input.GetAxis ("Mouse Y") * Time.deltaTime * Yspeed * -1, Input.GetAxis ("Mouse X") * Time.deltaTime * Xspeed * -1, 0, Space.Self);
	}

	public void OnEnter (BaseEventData e)
	{

	}

	public void OnLeave (BaseEventData e)
	{

	}

	//public void Resized ()
	//{
	//	if (obj == null)
	//		return;
	//	//float x = gameObject.GetComponent<BoxCollider> ().size.x;
	//	//float y = gameObject.GetComponent<BoxCollider> ().size.y;

	//	//x = Screen.width * x / res.x;
	//	//y = Screen.height * y / res.y;
	//	//gameObject.GetComponent<BoxCollider> ().size = new Vector3 (x, y, gameObject.GetComponent<BoxCollider> ().size.z);
	//	//obj.transform.position = cam.ScreenToWorldPoint (pos.position);
	//	Vector3 pos2 = Camera.allCameras [0].WorldToScreenPoint (ancher.position);
	//	pos2.z = 15;
	//	pos2 = Camera.allCameras [0].ScreenToWorldPoint (pos2);
	//	obj.transform.position = pos2;//pos.position; // Camera.allCameras [0].ViewportToWorldPoint (pos.right);
	//}
}
