using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	[Header( "References" )]
	public GameObject ItemPrefab;

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
	}

    void Update()
    {

    }

	private void AddListing( BaseItem item )
	{
		GameObject listing = Instantiate( ItemPrefab, transform );
		listing.GetComponentsInChildren<Text>()[0].text = item.Name;
	}
}
