using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ObjectActrionHelper : MonoBehaviour ,IPointerEnterHandler,IPointerExitHandler,IDropHandler,IDragHandler
{
	public Boolean isActive = true;
	public Boolean objSelOnEnter = true;
	public Boolean objSelOnExit = true;
	public Boolean objSelOnActivate = false;
	public UnityEvent onPointerEnterEvent;
	public UnityEvent onPointerExitEvent;
	public UnityEvent onDropEvent;
	public UnityEvent onPointerOverEvent;
	public UnityEvent onDragEvent = new UnityEvent ();
	public UnityEvent OnEnterNoSelect;

	public delegate void baseEventType (BaseEventData eventData);

	public baseEventType onDragdeli;

	private Transform targetObject;
	private bool isPointerOver = false;
	// Use this for initialization
	void Start ()
	{
		isPointerOver = false;
	}

	void Update ()
	{
		if (isPointerOver && isActive) {
			onPointerOverEvent.Invoke ();
		}
	}

	public void Activate (BaseEventData eventData)
	{
		if (!objSelOnActivate || eventData.selectedObject != null)
			isActive = true;
		else if (objSelOnActivate && isActive)
			isActive = false;
	}

	public void DeactivateIsNoSelection (BaseEventData eventData)
	{
		if (eventData.selectedObject == null)
			isActive = false;
	}

	public void Deactivate ()
	{
		isActive = false;
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (isActive)
		if (!objSelOnEnter || eventData.selectedObject != null) {
			onPointerEnterEvent.Invoke ();
			isPointerOver = true;
		}
		if (eventData.selectedObject == null) {
			OnEnterNoSelect.Invoke ();
		}
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		if (isActive)
		if (!objSelOnExit || eventData.selectedObject != null) {
			onPointerExitEvent.Invoke ();
			isPointerOver = false;
		}
	}

	public void OnDrop (PointerEventData eventData)
	{
		if (isActive) {
			onDropEvent.Invoke ();
			isPointerOver = false;
		}
	}

	public void OnDrag (PointerEventData eventData)
	{
		if (isActive) {
			onDragEvent.Invoke ();
		}
	}

	public void SetTargetObject (Transform obj)
	{
		if (targetObject != obj)
			targetObject = obj;
	}

	public void RotateObjectX (float speed)
	{
		if (targetObject != null) {
			targetObject.Rotate (new Vector3 (speed * Time.deltaTime, 0, 0));
		}
	}

	public void RotateObjectY (float speed)
	{
		if (targetObject != null) {
			targetObject.Rotate (new Vector3 (0, speed * Time.deltaTime, 0));
		}
	}

	public void RotateObjectZ (float speed)
	{
		if (targetObject != null) {
			targetObject.Rotate (new Vector3 (0, 0, speed * Time.deltaTime));
		}
	}
}
