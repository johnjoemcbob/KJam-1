using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimerEnemy : BaseEnemy
{
	#region MonoBehaviour
	public override void Update()
	{
		base.Update();

		// Bob up and down
		transform.GetChild( 0 ).localPosition = new Vector3( 0, Mathf.Sin( Time.time * 5 ) * 0.1f, 0 );

		// Update animations
		GetComponentInChildren<Animator>().SetFloat( "Speed", Agent.speed );
	}
	#endregion

	#region Actions
	public override void Attack()
	{
		base.Attack();

		GetComponentInChildren<Animator>().SetTrigger( "Attack" );

		// Spawn projectile
		GameObject projectile = Instantiate( Resources.Load( "Prefabs/SlimerProjectile" ), Game.RuntimeParent ) as GameObject;
		projectile.transform.position = Animator.transform.position + Animator.transform.up * 0.5f;
		projectile.transform.LookAt( Player.Instance.transform.Find( "CenterMass" ) );
		projectile.GetComponent<BaseProjectile>().PlayerTeam = false;
		Destroy( projectile, 5 );
	}
	#endregion

	#region Health
	protected override void Die()
	{
		base.Die();

		// Explode
		GameObject death = Instantiate( Resources.Load( "Prefabs/Slimer Death" ), Game.RuntimeParent ) as GameObject;
		death.transform.position = transform.position;
		death.transform.rotation = transform.rotation;
		Destroy( death, 1 );

		// TODO TEMP spawn new replacement
		//Game.Instance.SpawnEnemy( Resources.Load( "Prefabs/Slimer" ) as GameObject );

		// Communicate with Game
		Game.Instance.OnEnemyKilled( this );

		// Destroy npc
		gameObject.SetActive( false );
		Destroy( gameObject );
	}
	#endregion
}
