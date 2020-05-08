using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : Hitable
{
	[Header( "Variables" )]
	public float MaxHealth;

	protected float Health;

	public virtual void Start()
	{
		Health = MaxHealth;
	}

	public float GetHealth()
	{
		return Health;
	}

	public void SetHealth( float health )
	{
		Health = Mathf.Clamp( health, 0, MaxHealth );
	}

	public void TakeDamage( float damage )
	{
		SetHealth( Health - damage );
		if ( Health <= 0 )
		{
			Die();
		}
	}

	public override void OnHit( Collider other )
	{
		base.OnHit( other );

		// TODO get hitbox damage values here
		var hit = other.transform.GetComponentInChildren<Hitbox>();
		if ( hit != null )
		{
			bool isplayer = ( this == Player.Instance );
			if ( hit.PlayerTeam != isplayer )
			{
				TakeDamage( hit.Damage );
			}
		}
	}

	protected virtual void Die()
	{

	}
}
