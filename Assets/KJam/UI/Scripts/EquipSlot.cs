using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipSlot : MonoBehaviour, IDropHandler
{
	public ItemType AcceptsItemType;

	public void OnDrop( PointerEventData data )
	{
		// Check its the correct item type
		if ( true )
		{
			DragItem.CurrentDragged.transform.parent = transform;
			DragItem.CurrentDragged.transform.localPosition = Vector3.zero;
			DragItem.CurrentDragged = null;
		}
	}
}
