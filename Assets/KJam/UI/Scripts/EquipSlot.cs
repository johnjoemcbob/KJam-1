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
		UpdateUINames();
	}

	private void Update()
	{
		if ( !Application.isPlaying )
		{
			UpdateUINames();
		}

		// Lerp
		float size = 1.2f;
		Vector3 target = Vector3.one;
		if ( DragItem.CurrentDragged != null )
		{
			bool correct = ( InventoryUI.Instance.Listings[DragItem.CurrentDragged.gameObject].Item.Type == AcceptsItemType );
			target = correct ? target * size : target / size;
		}
		transform.localScale = Vector3.Lerp( transform.localScale, target, Time.deltaTime * 5 );
	}

	public void OnDrop( PointerEventData data )
	{
		InventoryUI.Instance.DropOnEquipSlot( AcceptsItemType.ToString(), DragItem.CurrentDragged.gameObject, this );
		DragItem.CurrentDragged = null;
	}

	public void UpdateUINames()
	{
		name = AcceptsItemType.ToString() + " (Slot)";
		GetComponentInChildren<Text>().text = AcceptsItemType.ToString();
	}
}
