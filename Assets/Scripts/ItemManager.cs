using System.Collections;
using System.Collections.Generic;
using BF.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using BF.Treetailor;
using System.Linq;

public class ItemManager : MonoBehaviour
{
	public float zPosition = 12.5f;
	public GameObject dragableObjPrefab;
	private GameObject dragingObj;
	//public int itemAdded = 0;
	//public int maxItemCount = 0;
	public int productId = 0;
    public ProductInfo.ProductType productType;
	public SceneManager sceneManager;
    public ColorFader colorFader;
    public ProductInfo.ProductType[] acceptableProducts;
	private bool canBeDraged = false;
	private ProductItem productItemLink = null;
	private Vector3 originalSize = Vector3.one;
	private Transform oldParent;
	// Use this for initialization
	void Start ()
	{
		//gameObject.layer = LayerMask.NameToLayer ("Default");
		sceneManager = FindObjectOfType<SceneManager> ();
		originalSize = gameObject.transform.localScale;
        if(sceneManager != null)
        zPosition = sceneManager.DragItemszPos;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void BeginDrag ()
	{
		canBeDraged = false;
		if (sceneManager.GetProductLeft (productId) > 0) {
			Debug.Log ("Draging Begins");
			dragingObj = Instantiate (dragableObjPrefab, gameObject.transform.position, dragableObjPrefab.transform.rotation);
			dragingObj.GetComponent<ItemManager> ().productId = productId;
            dragingObj.GetComponent<ItemManager>().productType = productType;
			dragingObj.GetComponent<ItemManager> ().productItemLink = gameObject.GetComponent<ProductItem> ();
			dragingObj.name = dragingObj.name.Replace ("(Clone)", "") + "(" + sceneManager.GetProductUsedCount (productId) + ")";
			canBeDraged = true;
		}
	}

	public void DragEnded (BaseEventData e)
	{
		Debug.Log ("Draging ended");
		if (e.selectedObject != null) {
			GameObject.Destroy (dragingObj);
		} else {
			sceneManager.UseOne (productId);
		}
		UpdateDisplayName ();
		canBeDraged = false;
	}

	public void OnDrag (BaseEventData e)
	{
		if (!canBeDraged)
			return;

		Debug.Log ("Draging");
		Vector3 pos = Vector3.zero;
		//RaycastHit hitInfo;

		//Vector3 fwd = transform.TransformDirection((dragingObj.transform.position -new Vector3(dragingObj.transform.position.x, dragingObj.transform.position.y, 100)));

		//Physics.Raycast(new Vector3(dragingObj.transform.position.x, dragingObj.transform.position.y, 13), fwd, out hitInfo, 100); // THIS WORKS
		//Debug.DrawRay(new Vector3(dragingObj.transform.position.x, dragingObj.transform.position.y, 13), fwd, Color.green);  // draw the debug;


        //RaycastHit hit;
        //if (Physics.Raycast(dragingObj.transform.position, Vector3.forward, out hit))
        //{
        //    Collider coll = dragingObj.GetComponent<Collider>();
           // var bottom = coll.bounds.max.z;

         //   zPosition = dragingObj.transform.position.z - hit.distance;// + bottom;
        //}
        //else
        //    zPosition = 12.5f;

        //Ray myRay = new Ray(new Vector3(dragingObj.transform.position.x, dragingObj.transform.position.y, 16), dragingObj.transform.position + new Vector3(dragingObj.transform.position.x, dragingObj.transform.position.y, 63));

       // zPosition = hitInfo.point.z;
        //Debug.Log("first "+zPosition);
        //if (zPosition < 0)
        //    zPosition = 12.5f;
        //else if (zPosition >= 100)
        //    zPosition = 12.5f;
		if (Input.touchCount > 0)
			pos = new Vector3 (Input.GetTouch (0).position.x, Input.GetTouch (0).position.y, zPosition);
		else
            pos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, zPosition);

		pos = Camera.allCameras [0].ScreenToWorldPoint (pos);
		dragingObj.transform.position = pos;
		e.selectedObject = dragingObj;

	}

	public void OnDrop (BaseEventData e)
	{
		Debug.Log ("Droped");
		//GameObject newg = Instantiate (e.selectedObject, new Vector3 (0, 0, 0), new Quaternion ());

        if (e.selectedObject != null && gameObject.transform.childCount == 0 && IsAccaptable(e.selectedObject)) {
			//int _productId = e.selectedObject.GetComponent<ItemManager> ().productId;
			//if (sceneManager.UseOne (_productId)) { // add one to used product and returns true if there is any left to use
			//gameObject.transform.localScale = new Vector3 (1, 1, 1);
			gameObject.transform.localScale = originalSize;
			e.selectedObject.transform.SetParent (gameObject.transform);
			e.selectedObject.transform.localPosition = new Vector3 (0, 0, 0);
			e.selectedObject.transform.rotation = new Quaternion (0, 0, 0, 0);
			e.selectedObject.layer = LayerMask.NameToLayer ("Droped");

			//itemAdded++;

			e.selectedObject = null;
			//}
		}
	}

	public void BeginReDrag ()
	{
		Debug.Log ("reDraging Begins");
		dragingObj = gameObject;
		gameObject.layer = LayerMask.NameToLayer ("OnDrag");
		canBeDraged = true;
		oldParent = gameObject.transform.parent;

        dragingObj.transform.parent = null;
		gameObject.transform.localScale = originalSize;
	}

	public void ReDragEnded (BaseEventData e)
	{
		Debug.Log ("reDraging ended");
		if (e.selectedObject != null) {
			gameObject.layer = LayerMask.NameToLayer ("Droped");

			if (e.selectedObject.transform.parent == null) {
				e.selectedObject.transform.parent = oldParent;	
				oldParent = null;
			}

			e.selectedObject.transform.localPosition = new Vector3 (0, 0, 0);
			e.selectedObject.transform.rotation = new Quaternion (0, 0, 0, 0);
			e.selectedObject.GetComponent<ItemManager> ().UpdateDisplayName ();
			e.selectedObject = null;
		}
		canBeDraged = false;
	}

	public void OnDropInBasket (BaseEventData e)
	{
		Debug.Log ("Droped in basket");
		//GameObject newg = Instantiate (e.selectedObject, new Vector3 (0, 0, 0), new Quaternion ());

		if (e.selectedObject != null && e.selectedObject.layer == LayerMask.NameToLayer ("OnDrag")) {
			int _productId = e.selectedObject.GetComponent<ItemManager> ().productId;
			sceneManager.BackToBasketOne (_productId);
			e.selectedObject.GetComponent<ItemManager> ().UpdateDisplayName ();
			gameObject.transform.localScale = new Vector3 (1, 1, 1);
			GameObject.Destroy (e.selectedObject);
			e.selectedObject = null;
		}
	}

	public void MouseEnter (BaseEventData e)
	{
        
        if (e.selectedObject != null && gameObject.transform.childCount == 0 && IsAccaptable(e.selectedObject))
        {
            colorFader.FadeOut();
            gameObject.transform.localScale = (gameObject.transform.localScale * 1.5f);
        }
        else if (e.selectedObject == null && gameObject.transform.childCount != 0)
        {
            colorFader.FadeOut();
            gameObject.transform.localScale = (gameObject.transform.localScale * 1.2f);
        }
	}

	public void MouseLeave ()
	{
		gameObject.transform.localScale = originalSize;
        if(!colorFader.IsFadeIn())
            colorFader.FadeIn();
	}

	public void UpdateDisplayName ()
	{
		if (productItemLink == null)
			productItemLink = gameObject.GetComponent<ProductItem> ();
		if (productItemLink != null)
			productItemLink.text.text = sceneManager.GetProductDisplayName (productId);
	}

    public bool IsAccaptable(GameObject selectedObject)
    {
        var temp = selectedObject.GetComponent<ItemManager>();
        if( temp != null)
        {
            if(acceptableProducts.Length ==0 || acceptableProducts.Contains(temp.productType))
            {
                return true;
            }
        }
        return false;
    }
}
