using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEnemy : BaseEnemy
{
	#region MonoBehaviour
	public override void Update()
	{
		base.Update();

		GetComponentInChildren<Animator>().SetFloat( "Speed", Agent.speed );
	}
	#endregion

	#region Actions
	public override void Attack()
	{
		base.Attack();

		GetComponentInChildren<Animator>().SetTrigger( "Attack" );

		Hitbox.Spawn( false, 1, transform.position + transform.up * 1 + transform.forward * 1, transform.rotation, transform.localScale );
	}
	#endregion

	#region Health
	protected override void Die()
	{
		base.Die();

		// Explode
		GameObject death = Instantiate( Resources.Load( "Prefabs/Skeleton Death" ), Game.RuntimeParent ) as GameObject;
		death.transform.position = transform.position;
		death.transform.rotation = transform.rotation;
		Destroy( death, 1 );

		// TODO TEMP spawn new replacement
		Game.Instance.SpawnEnemy( Resources.Load( "Prefabs/Skeleton" ) as GameObject );

		// Destroy npc
		gameObject.SetActive( false );
		Destroy( gameObject );
	}
	#endregion
}
