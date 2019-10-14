using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualChild : MonoBehaviour
{

	public Transform targetParent = null;
	public bool updateExact = false;
	public bool followPositioning = true;
	public bool followRotation = true;
	public bool followScalling = true;
	public bool ignoreZpos = false;
	public bool ignoreYpos = false;
	public bool ignoreXpos = false;

	private Vector3 positionOffset;
	private Vector3 rotationOffset;
	private Vector3 scaleOffset;

	private Vector3 originalPosition;
	private Quaternion originalRotation;
	private Vector3 originalLocalScale;
	// Use this for initialization

	void Start ()
	{
		
		originalPosition = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
		originalRotation = new Quaternion (gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z, gameObject.transform.rotation.w);
		originalLocalScale = new Vector3 (gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
		if (targetParent != null) {
			positionOffset = gameObject.transform.position - targetParent.position;
			rotationOffset = gameObject.transform.rotation.eulerAngles - targetParent.rotation.eulerAngles;
			scaleOffset = gameObject.transform.localScale - targetParent.localScale;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (followPositioning) {
            if ((!ignoreXpos && !gameObject.transform.position.x.Equals(targetParent.position.x + positionOffset.x))
                || (!ignoreYpos && !gameObject.transform.position.y.Equals(targetParent.position.y + positionOffset.y))
                ||(!ignoreZpos && !gameObject.transform.position.z.Equals(targetParent.position.z + positionOffset.z)))
		//	if (!gameObject.transform.position.Equals(targetParent.position + positionOffset))
			{
				Debug.Log("positioning");
                float x, y, z;
                x = targetParent.position.x + positionOffset.x;
                y = targetParent.position.y + positionOffset.y;
                z = targetParent.position.z + positionOffset.z;
                if (ignoreXpos)
                    x = gameObject.transform.position.x;
				if (ignoreYpos)
					y = gameObject.transform.position.y;
				if (ignoreZpos)
					z = gameObject.transform.position.z;
                //gameObject.transform.position = targetParent.position + positionOffset;
                gameObject.transform.position = new Vector3(x, y, z);
			}
		}
		if (followRotation) {
			if (!gameObject.transform.rotation.eulerAngles.Equals (targetParent.rotation.eulerAngles + rotationOffset)) {
				Debug.Log ("rotating");
				gameObject.transform.rotation = Quaternion.Euler (targetParent.rotation.eulerAngles + rotationOffset);
			}
		}
		if (followScalling) {
			if (!gameObject.transform.localScale.Equals (targetParent.localScale + scaleOffset)) {
				Debug.Log ("scaling");
				gameObject.transform.localScale = targetParent.localScale + scaleOffset;
			}
		}
	}
}
