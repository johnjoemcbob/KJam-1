﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : Hitable
{
	public float MaxHealth;

	[HideInInspector]
	public Dictionary<string, BuffableVariable> BuffableVariable = new Dictionary<string, BuffableVariable>();
	protected float Health;
	protected bool Dead = false;

	public override void Start()
	{
		base.Start();

		BuffableVariable.Add( "MaxHP", new BuffableVariable( MaxHealth ) );
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
		SpawnDamageIndicator( damage );
		if ( Health <= 0 )
		{
			Die();
			Dead = true;
		}
	}

	public override void OnHit( Collider other )
	{
		// Get hitbox damage values here
		var hit = other.transform.GetComponentInChildren<Hitbox>();
		if ( hit != null )
		{
			bool isplayer = ( this == Player.Instance );
			if ( hit.PlayerTeam != isplayer )
			{
				base.OnHit( other );

				TakeDamage( hit.Damage );
			}
		}
	}

	protected virtual void Die()
	{

	}

	public void SpawnDamageIndicator( float damage )
	{
		var obj = StaticHelpers.GetOrCreateCachedPrefab( "Damage Indicator", transform.position + transform.up * 1.5f, transform.rotation, Vector3.one, 0 );
		var dmg = obj.GetComponent<DamageIndicator>();
		dmg.Init();
		dmg.SetDamage( damage );
		dmg.SetTeam( this == Player.Instance );
	}
}
