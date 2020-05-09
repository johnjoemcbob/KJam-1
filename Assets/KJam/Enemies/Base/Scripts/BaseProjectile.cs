using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : Hitbox
{
	public float Speed = 5;

	private bool HasHit = false;

	public virtual void Update()
	{
		transform.position += transform.forward * Time.deltaTime * Speed;
	}

	public void OnTriggerEnter( Collider other )
	{
		bool isplayer = ( other.transform == Player.Instance.transform );
		bool noteam = !isplayer && !other.GetComponent<BaseEnemy>();
		if ( !HasHit && ( ( PlayerTeam != isplayer ) || noteam ) )
		{
			Hit( other );
			HasHit = true;

			Destroy( gameObject );
		}
	}

	public virtual void Hit( Collider other )
	{
		var dmg = other.GetComponentInChildren<Hitable>();
		if ( dmg != null )
		{
			dmg.OnHit( GetComponent<Collider>() );
		}
	}
}
