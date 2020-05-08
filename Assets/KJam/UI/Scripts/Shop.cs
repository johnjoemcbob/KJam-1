using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	[Header( "References" )]
	public GameObject ItemListingPrefab;

    void Start()
    {
		// Clear
		foreach ( Transform child in transform )
		{
			Destroy( child.gameObject );
		}

		// Add
		AddListing( "Item", 6 );
		AddListing( "Sword", 12 );
		AddListing( "Potion", 3 );
		AddListing( "Shield", 2 );
		AddListing( "Amulet", 1 );
		AddListing( "Thing", 5 );
		AddListing( "Ring", 7 );
		AddListing( "Belt", 12 );
	}

    void Update()
    {

    }

	private void AddListing( string name, int cost )
	{
		BaseItem item = new BaseItem();
		{
			item.Name = name;
			item.Cost = cost;
		}
		AddListing( item );
	}

	private void AddListing( BaseItem item )
	{
		GameObject listing = Instantiate( ItemListingPrefab, transform );
		listing.GetComponentsInChildren<Text>()[0].text = item.Cost + "G";
		listing.GetComponentsInChildren<Text>()[1].text = "Buy " + item.Name;

		listing.GetComponentInChildren<Button>().onClick.AddListener( delegate { ButtonClickBuyListing( listing, item ); } );
	}

	private void ButtonClickBuyListing( GameObject listing, BaseItem item )
	{
		if ( Player.Instance.HasGold( item.Cost ) )
		{
			Player.Instance.AddGold( -item.Cost );
			Player.Instance.AddItem( item );

			Destroy( listing );
		}
	}
}
