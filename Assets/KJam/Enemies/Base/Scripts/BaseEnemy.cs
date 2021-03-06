﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent( typeof(NavMeshAgent) )]
public class BaseEnemy : Killable
{
	public float AttackRange = 1.5f;
	public float Damage = 5;
	public float KillDelay = 0.5f;

	protected float NextAttack = 0;
	protected bool Killed = false;
	protected float StartHealth;

	protected Animator Animator;
	protected NavMeshAgent Agent;
	protected Slider HealthBar;

	#region MonoBehaviour
	public override void Start()
    {
		base.Start();

		Animator = GetComponentInChildren<Animator>();
		Agent = GetComponent<NavMeshAgent>();

		Height = Agent.height;

		if ( Health != 0 )
		{
			StartHealth = Health;

			var bar = StaticHelpers.SpawnResource( "Prefabs/Healthbar" );
			bar.transform.parent = transform;
			bar.transform.localPosition = Vector3.up * ( Height + 0.1f );
			HealthBar = bar.GetComponentInChildren<Slider>();
		}
	}

    public override void Update()
    {
		base.Update();

		// This should still work for non-navmesh areas, but tables have to be jumped on to
		var player = FindObjectOfType<Player>();
		if ( player == null ) return;

		Vector3 playerpos = player.transform.position;
		Vector3 groundpos = new Vector3( playerpos.x, transform.position.y, playerpos.z );
		float searchdist = Agent.height * 2;

		NavMeshHit hit;
		if ( NavMesh.SamplePosition( playerpos, out hit, searchdist, Agent.areaMask ) )
		{
			Agent.SetDestination( hit.position );
		}
		else if ( NavMesh.SamplePosition( groundpos, out hit, searchdist, Agent.areaMask ) )
		{
			Agent.SetDestination( hit.position );
		}

		// If near enough player, try to attack
		if ( Vector3.Distance( transform.position, playerpos ) < AttackRange )
		{
			Agent.SetDestination( transform.position );
			transform.LookAt( playerpos );
			transform.localEulerAngles = new Vector3( 0, transform.localEulerAngles.y, 0 );

			if ( NextAttack <= Time.time )
			{
				Attack();
			}
		}
	}
	#endregion

	#region Actions
	public virtual void Attack()
	{
		NextAttack = Time.time + 1;
	}
	#endregion

	#region Health
	public override void OnHit( Collider other )
	{
		base.OnHit( other );

		//if ( Health > 0 )
		{
			var hit = other.transform.GetComponentInChildren<Hitbox>();
			if ( hit != null )
			{
				bool isplayer = ( this == Player.Instance );
				if ( hit.PlayerTeam != isplayer )
				{
					Vector3 dir = ( transform.position - other.transform.position ).normalized;
					StaticHelpers.GetOrCreateCachedPrefab( name + " Hit", other.ClosestPointOnBounds( transform.position ), Quaternion.LookRotation( dir, Vector3.up ), Vector3.one );// * hit.Damage / 5 );

					if ( HealthBar != null )
					{
						HealthBar.value = Health / StartHealth;
					}
				}
			}
		}
	}

	protected override void Die()
	{
		base.Die();

		if ( HealthBar != null )
		{
			HealthBar.gameObject.SetActive( false );
		}
	}
	#endregion
}
