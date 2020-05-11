using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ItemSorter : IComparer<Object>
{
	int IComparer<Object>.Compare( Object x, Object y )
	{
		var a = x as BaseItem;
		var b = y as BaseItem;
		return a.Cost.CompareTo( b.Cost );
	}
}

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

		// Find all item resources
		var items = Resources.LoadAll( "Items", typeof( BaseItem ) );
		var sort = new ItemSorter();
		System.Array.Sort<Object>( items, sort );
		int count = 0;
		int max = 8;
		foreach ( var item in items )
		{
			if ( count < max )
			{
				bool success = AddListing( item as BaseItem );
				if ( success )
				{
					count++;
				}
			}
		}
	}
	#endregion

	#region Listings
	private bool AddListing( string name, ItemType type, int cost )
	{
		BaseItem item = new BaseItem();
		{
			item.Name = name;
			item.Type = type;
			item.Cost = cost;
		}
		return AddListing( item );
	}

	private bool AddListing( BaseItem item )
	{
		if ( !item.Buyable || Player.Instance.Items.Contains( item ) ) return false;

		GameObject listing = Instantiate( ItemListingPrefab, transform );
		listing.GetComponentsInChildren<Text>()[0].text = item.Cost + "G";
		listing.GetComponentsInChildren<Text>()[1].text = item.Name;

		listing.GetComponentsInChildren<Image>()[3].sprite = item.Sprite;

		listing.GetComponentInChildren<Button>().onClick.AddListener( delegate { ButtonClickBuyListing( listing, item ); } );

		return true;
	}
	#endregion

	#region Buttons
	private void ButtonClickBuyListing( GameObject listing, BaseItem item )
	{
		if ( Player.Instance.HasGold( item.Cost ) )
		{
			Player.Instance.AddGold( -item.Cost );
			Player.Instance.AddItem( item );

			StaticHelpers.GetOrCreateCachedAudioSource( "gold_spend", false, Random.Range( 0.8f, 1.2f ) );

			Destroy( listing );
		}
	}
	#endregion
}
