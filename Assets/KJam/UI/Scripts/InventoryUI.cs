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
		public string EquippedIn;
	}

	#region ==Inspector
	[Header( "References" )]
	public GameObject ItemPrefab;
	#endregion

	#region ==Variables
	Dictionary<string, EquipSlot> Slots = new Dictionary<string, EquipSlot>();
	[HideInInspector]
	public Dictionary<GameObject, UIListing> Listings = new Dictionary<GameObject, UIListing>();
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
		listing.GetComponentsInChildren<Image>()[3].sprite = item.Sprite;

		// Store
		UIListing uilist = new UIListing();
		{
			uilist.Element = listing;
			uilist.Item = item;
			uilist.EquippedIn = "";
		}
		Listings.Add( listing, uilist );
	}
	#endregion

	#region UI - Drag & Drop
	public void OnDrag( DragItem item )
	{
		// Store correct slot if removed from Equipped
		var listing = Listings[item.gameObject];
		var slot = listing.Element.GetComponentInParent<EquipSlot>();
		if ( slot != null )
		{
			listing.EquippedIn = slot.AcceptsItemType.ToString();
			Listings[item.gameObject] = listing;
		}

		StaticHelpers.GetOrCreateCachedAudioSource( "ui_drag", true, Random.Range( 0.8f, 1.2f ) );
	}

	protected void OnDrop()
	{
		StaticHelpers.GetOrCreateCachedAudioSource( "ui_drop", true, Random.Range( 0.8f, 1.2f ) );
	}

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

		OnDrop();

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
		OnDrop();

		// Unequip if was equipped
		if ( listing.EquippedIn != "" && HasEquipped( listing.EquippedIn ) )
		{
			UnEquip( listing.EquippedIn );
		}

		listing.Element.transform.parent = transform;
	}
	#endregion

	#region Equip
	public void Equip( string type, UIListing listing, EquipSlot slot, bool visualonly = false )
	{
		listing.EquippedIn = type;
		Equipped.Add( type, listing );

		// Communicate with player
		if ( !visualonly )
		{
			Player.Instance.Equip( type, listing.Item );
		}

		// Parent UI to slot
		listing.Element.transform.parent = slot.transform;
		listing.Element.transform.localPosition = Vector3.zero;
		listing.Element.transform.localScale = Vector3.one;
	}

	public void UnEquip( string type )
	{
		UIListing listing = Equipped[type];
		{
			listing.EquippedIn = "";
		}
		Equipped[type] = listing;
		Equipped.Remove( type );

		// Communicate with player
		Player.Instance.UnEquip( type );

		// Return UI to inventory
		listing.Element.transform.parent = transform;
		listing.Element.transform.localScale = Vector3.one;
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
		// Store all first to avoid messing up order when equipping
		List<GameObject> objs = new List<GameObject>();
		foreach ( var item in Player.Instance.GetEquippedItems() )
		{
			objs.Add( transform.GetChild( item.Value ).gameObject );
		}

		int index = 0;
		foreach ( var item in Player.Instance.GetEquippedItems() )
		{
			Equip( item.Key, Listings[objs[index]], Slots[item.Key], true );
			index++;
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
