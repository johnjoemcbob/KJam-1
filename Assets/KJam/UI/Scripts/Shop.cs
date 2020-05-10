using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Shop : MonoBehaviour
{
	[Header( "References" )]
	public GameObject ItemListingPrefab;

	#region MonoBehaviour
	void Start()
    {
		// Clear
		foreach ( Transform child in transform )
		{
			Destroy( child.gameObject );
		}

		// Add
		//AddListing( "Item", ItemType.Amulet, 6 );
		//AddListing( "Sword", ItemType.Weapon, 12 );
		//AddListing( "Potion", ItemType.Ring, 3 );
		//AddListing( "Shield", ItemType.Weapon, 2 );
		//AddListing( "Amulet", ItemType.Amulet, 1 );
		//AddListing( "Thing", ItemType.Ring, 5 );
		//AddListing( "Ring", ItemType.Ring, 7 );
		//AddListing( "Belt", ItemType.Belt, 12 );

		// Find all item resources
		var items = Resources.LoadAll( "Items", typeof( BaseItem ) );
		foreach ( var item in items )
		{
			AddListing( item as BaseItem );
		}
	}
	#endregion

	#region Listings
	private void AddListing( string name, ItemType type, int cost )
	{
		BaseItem item = new BaseItem();
		{
			item.Name = name;
			item.Type = type;
			item.Cost = cost;
		}
		AddListing( item );
	}

	private void AddListing( BaseItem item )
	{
		if ( !item.Buyable ) return;

		GameObject listing = Instantiate( ItemListingPrefab, transform );
		listing.GetComponentsInChildren<Text>()[0].text = item.Cost + "G";
		listing.GetComponentsInChildren<Text>()[1].text = "Buy " + item.Name;

		listing.GetComponentsInChildren<Image>()[2].sprite = item.Sprite;

		listing.GetComponentInChildren<Button>().onClick.AddListener( delegate { ButtonClickBuyListing( listing, item ); } );
	}
	#endregion

	#region Buttons
	private void ButtonClickBuyListing( GameObject listing, BaseItem item )
	{
		if ( Player.Instance.HasGold( item.Cost ) )
		{
			Player.Instance.AddGold( -item.Cost );
			Player.Instance.AddItem( item );

			Destroy( listing );
		}
	}
	#endregion
}
