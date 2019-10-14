using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinItToScreen : MonoBehaviour {

    public GameObject TargetObject;
    public bool OnObjectMoved = false;
    public bool OnScreenReSized = true;
    private RectTransform CanvasRect;
    private Vector2 res;
    private Vector3 oldPosition;
    //public Vector3 pos1;
    // Use this for initialization
    void Start () {
        res = new Vector2(Screen.width, Screen.height);
        CanvasRect = GameObject.FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        if (TargetObject != null)
        {
      //      pos1 = Camera.allCameras[0].WorldToViewportPoint(TargetObject.transform.position);
        //    pos1 = new Vector3(
          //      ((pos1.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            //    ((pos1.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)), 100);
            Resized();
        }
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        oldPosition = TargetObject.transform.position;
    }

    // Update is called once per frame
    void Update () {
        if(OnScreenReSized)
        if (res.x != Screen.width || res.y != Screen.height)
        {
            Resized();
            res = new Vector2(Screen.width, Screen.height);
        }
        if(OnObjectMoved)
        if(TargetObject.transform.position != oldPosition)
        {
            Resized();
            oldPosition = TargetObject.transform.position;
        }
    }

    public void Resized()
    {
        if (TargetObject == null)
            return;
        Vector3 pos2 = Camera.allCameras[0].WorldToScreenPoint(gameObject.transform.position);
        pos2.z = TargetObject.transform.position.z;
        pos2 = Camera.allCameras[0].ScreenToWorldPoint(pos2);
        TargetObject.transform.position = pos2;
    }
}
