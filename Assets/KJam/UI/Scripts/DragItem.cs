using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public static DragItem CurrentDragged;

	private Transform OldParent;
	private Vector3 OldPos;

	public void OnBeginDrag( PointerEventData data )
	{
		OldParent = transform.parent;
		OldPos = transform.localPosition;

		FindObjectOfType<InventoryUI>().OnDrag( this );

		GetComponent<Image>().raycastTarget = false;
		transform.parent = GetComponentInParent<Canvas>().transform;

		CurrentDragged = this;
	}

	public void OnDrag( PointerEventData data )
	{
		transform.position = Input.mousePosition;
		transform.localScale = Vector3.one;
	}

	public void OnEndDrag( PointerEventData data )
	{
		GetComponent<Image>().raycastTarget = true;

		if ( CurrentDragged != null )
		{
			//transform.parent = OldParent;
			transform.localPosition = OldPos;
			//OldParent = null;

			InventoryUI.Instance.DropOnEmpty( gameObject );

			CurrentDragged = null;
		}
	}
}
