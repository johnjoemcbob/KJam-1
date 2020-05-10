using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEnemy : BaseEnemy
{
	#region MonoBehaviour
	public override void Update()
	{
		base.Update();

		// Update animations
		GetComponentInChildren<Animator>().SetFloat( "Speed", Agent.speed );
	}

	private void OnDestroy()
	{
		// Explode
		GameObject death = Instantiate( Resources.Load( "Prefabs/Skeleton Death" ), Game.RuntimeParent ) as GameObject;
		death.transform.position = transform.position;
		death.transform.rotation = transform.rotation;
		Destroy( death, 1 );
	}
	#endregion

	#region Actions
	public override void Attack()
	{
		base.Attack();

		GetComponentInChildren<Animator>().SetTrigger( "Attack" );
		StaticHelpers.GetOrCreateCachedAudioSource( "skeleton_attack", transform.position, Random.Range( 0.8f, 1.2f ) );

		Hitbox.Spawn( false, Damage, transform.position + transform.up * 1 + transform.forward * 1, transform.rotation, transform.localScale );
	}
	#endregion

	#region Health
	protected override void Die()
	{
		if ( !Killed )
		{
			base.Die();

			// TODO TEMP spawn new replacement
			//Game.Instance.SpawnEnemy( Resources.Load( "Prefabs/Skeleton" ) as GameObject );

			// Communicate with Game
			Game.Instance.OnEnemyKilled( this );

			// Destroy npc
			this.enabled = false;
			Destroy( gameObject, KillDelay );
			Killed = true;
		}
	}
	#endregion
}
