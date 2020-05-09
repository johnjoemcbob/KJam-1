using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : Hitable
{
	[Header( "Variables" )]
	public float Health = 1;

	[Header( "Assets" )]
	public GameObject DestroyPrefab;

	private bool Destroyed = false;

	private void OnDestroy()
	{
		// Don't call on normal scene cleanup
		if ( Destroyed )
		{
			var destroy = Instantiate( DestroyPrefab, Game.RuntimeParent );
			{
				destroy.transform.position = transform.position;
				destroy.transform.rotation = transform.rotation;
				destroy.transform.localScale = transform.localScale;
			}
			Destroy( destroy, 1.1f );
		}
	}

	public override void OnHit( Collider other )
	{
		base.OnHit( other );

		Health -= 1; // TODO could be other.GetDamageSomehow
		if ( Health <= 0 && !Destroyed )
		{
			Destroy( gameObject );
			Destroyed = true;
		}
	}
}
