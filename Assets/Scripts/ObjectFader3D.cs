using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFader3D : MonoBehaviour {

    public GameObject selectedObject;
    public float duration;
    public bool startFading = false;
    Material[] mats;
	// Use this for initialization
	void Start () {
        mats = selectedObject.GetComponent<MeshRenderer>().materials;

	}
	
	// Update is called once per frame
	void Update () {
        if(startFading)
        {
            startFading = false;
            StartCoroutine(FadeOut(duration));
        }
		if (Input.GetKeyUp(KeyCode.T))
		{
            StartCoroutine(FadeTo(0.0f, duration));
		}
		if (Input.GetKeyUp(KeyCode.F))
		{
            StartCoroutine(FadeTo(1.0f, duration));
		}
	}

    /*    public IEnumerator FadeOut()
        {
            int fadedCount = 0;
            do
            {
                foreach (var m in mats)
                {
                    if (m.color.a > 0)
                    {
                        var newColor = new Color(m.color.r, m.color.b, m.color.g, m.color.a - Time.deltaTime / speed);
                        m.color = newColor;
                        if (m.color.a <= 0)
                            fadedCount++;
                    }
                }

            } while (fadedCount < mats.Length);
        }
    */
    float lerp = 0f;
    public IEnumerator FadeOut(float duration)
    {
        int fadedCount = 0;
        while (fadedCount < mats.Length)
        {
            foreach (var mat in mats)
            {
                if (mat.color.a > 0)
                {
                    Color newColor = mat.color;

                    newColor.a = (float)Mathf.Lerp(newColor.a, 0.0f, lerp);
                   
                    lerp += Time.deltaTime / duration;
                    mat.color = newColor;
                    if (mat.color.a <= 0)
                        fadedCount++;
                }
                Debug.Log(fadedCount);
                yield return null;
            }
			
        }
       
    
	}

	IEnumerator FadeTo(float aValue, float aTime)
	{
        foreach (var mat in mats)
        {
            float alpha = mat.color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
                mat.color = newColor;
                yield return null;
            }
        }
	}
}
