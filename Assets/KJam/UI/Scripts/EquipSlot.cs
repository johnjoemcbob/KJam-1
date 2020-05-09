using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class EquipSlot : MonoBehaviour, IDropHandler
{
	public ItemType AcceptsItemType;

	private void Start()
	{
		if ( InventoryUI.Instance != null )
		{
			InventoryUI.Instance.AddSlot( this );
		}
		UpdateUI();
	}

	private void Update()
	{
		if ( !Application.isPlaying )
		{
			UpdateUI();
		}
	}

	public void OnDrop( PointerEventData data )
	{
		// Check its the correct item type
		InventoryUI.Instance.DropOnEquipSlot( AcceptsItemType.ToString(), DragItem.CurrentDragged.gameObject, this );
		DragItem.CurrentDragged = null;
		//if ( true )
		//{
		//	DragItem.CurrentDragged.transform.parent = transform;
		//	DragItem.CurrentDragged.transform.localPosition = Vector3.zero;
		//	DragItem.CurrentDragged = null;
		//}
	}

	public void UpdateUI()
	{
		name = AcceptsItemType.ToString() + " (Slot)";
		GetComponentInChildren<Text>().text = AcceptsItemType.ToString();
	}
}
