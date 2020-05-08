using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour
{
	private void OnTriggerStay( Collider other )
	{
		if ( other.gameObject.layer == LayerMask.NameToLayer( "Hitbox" ) )
		{
			OnHit( other );
		}
	}

	public virtual void OnHit( Collider other )
	{

	}
}
