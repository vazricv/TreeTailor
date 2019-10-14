using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
	public float zOffset = -200.00f;
	public Camera m_Camera;

	void Start ()
	{
		m_Camera = Camera.allCameras [0];
	}

	void Update ()
	{
        transform.LookAt (new Vector3 (m_Camera.transform.position.x + transform.position.x, m_Camera.transform.position.y+ transform.position.y, m_Camera.transform.position.z + zOffset));
		//transform.LookAt (m_Camera.transform);
		//transform.LookAt (transform.position + m_Camera.transform.rotation * Vector3.forward,
		//	m_Camera.transform.rotation * Vector3.up);
	}
}