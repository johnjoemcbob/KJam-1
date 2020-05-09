using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
	public static InventoryUI Instance;

	public struct UIListing
	{
		public GameObject Element;
		public BaseItem Item;
		public string EquipedIn;
	}

	#region ==Inspector
	[Header( "References" )]
	public GameObject ItemPrefab;
	#endregion

	#region ==Variables
	Dictionary<string, EquipSlot> Slots = new Dictionary<string, EquipSlot>();
	private Dictionary<GameObject, UIListing> Listings = new Dictionary<GameObject, UIListing>();
	private Dictionary<string, UIListing> Equipped = new Dictionary<string, UIListing>();
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		// Clear
		foreach ( Transform child in transform )
		{
			Destroy( child.gameObject );
		}

		// TODO TEMP TESTING
		//BaseItem item = new BaseItem();
		//{
		//	item.Name = "Hi";
		//	item.Cost = 0;
		//	item.Type = ItemType.Weapon;
		//}
		//Player.Instance.AddItem( item );

		// Add
		foreach ( var inv in Player.Instance.GetInventory() )
		{
			AddListing( inv );
		}
		InitializeEquippedUI();
	}

    void Update()
    {

    }
	#endregion

	#region UI
	private void AddListing( BaseItem item )
	{
		// Create UI
		GameObject listing = Instantiate( ItemPrefab, transform );
		listing.GetComponentsInChildren<Text>()[0].text = item.Name;
		listing.GetComponentsInChildren<Image>()[2].sprite = item.Sprite;

		// Store
		UIListing uilist = new UIListing();
		{
			uilist.Element = listing;
			uilist.Item = item;
			uilist.EquipedIn = "";
		}
		Listings.Add( listing, uilist );
	}
	#endregion

	#region UI - Drag & Drop
	public void DropOnEquipSlot( string type, GameObject element, EquipSlot slot )
	{
		// Find listing from element
		DropOnEquipSlot( type, Listings[element], slot );
	}

	public void DropOnEquipSlot( string type, UIListing listing, EquipSlot slot )
	{
		if ( listing.Item.Type.ToString() != type )
		{
			DropOnEmpty( listing );
			return;
		}

		// Unequip any old item if there is one
		if ( HasEquipped( type ) )
		{
			UnEquip( type );
		}

		// Equip the new item
		Equip( type, listing, slot );
	}

	public void DropOnEmpty( GameObject element )
	{
		DropOnEmpty( Listings[element] );
	}

	public void DropOnEmpty( UIListing listing )
	{
		// Unequip if was equipped
		if ( listing.EquipedIn != "" && HasEquipped( listing.EquipedIn ) )
		{
			UnEquip( listing.EquipedIn );
		}

		listing.Element.transform.parent = transform;
	}
	#endregion

	#region Equip
	public void Equip( string type, UIListing listing, EquipSlot slot, bool visualonly = false )
	{
		listing.EquipedIn = type;
		Equipped.Add( type, listing );

		// Communicate with player
		if ( !visualonly )
		{
			Player.Instance.Equip( type, listing.Item );
		}

		// Parent UI to slot
		listing.Element.transform.parent = slot.transform;
		listing.Element.transform.localPosition = Vector3.zero;
	}

	public void UnEquip( string type )
	{
		UIListing listing = Equipped[type];
		{
			listing.EquipedIn = "";
		}
		Equipped[type] = listing;
		Equipped.Remove( type );

		// Communicate with player
		Player.Instance.UnEquip( type );

		// Return UI to inventory
		listing.Element.transform.parent = transform;
	}

	public Dictionary<string, UIListing> GetEquippeds()
	{
		return Equipped;
	}

	public bool HasEquipped( string slot )
	{
		return Equipped.ContainsKey( slot );
	}

	public UIListing GetEquipped( string slot )
	{
		return Equipped[slot];
	}

	private void InitializeEquippedUI()
	{
		foreach ( var item in Player.Instance.GetEquippedItems() )
		{
			Equip( item.Key, Listings[transform.GetChild( item.Value ).gameObject], Slots[item.Key], true );
		}
	}

	public void AddSlot( EquipSlot slot )
	{
		string key = slot.AcceptsItemType.ToString();
		if ( !Slots.ContainsKey( key ) )
		{
			Slots.Add( key, slot );
		}
		Slots[key] = slot;
	}
	#endregion
}
