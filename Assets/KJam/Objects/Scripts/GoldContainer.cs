using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldContainer : Destroyable
{
	[Header( "Variables Mk2" )]
	public int GoldValue = 1;

	protected override void OnDestroyabled()
	{
		base.OnDestroyabled();

		var obj = StaticHelpers.SpawnPrefab( "Gold" );
		obj.transform.position = transform.position + Vector3.up * 0.5f;
		obj.GetComponent<Gold>().TargetPos = transform.position + Vector3.up * 1;
		obj.GetComponent<Gold>().Amount = GoldValue;
	}
}
