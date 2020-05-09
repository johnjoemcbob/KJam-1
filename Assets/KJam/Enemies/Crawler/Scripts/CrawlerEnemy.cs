using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerEnemy : BaseEnemy
{
	private AudioSource Source;

	#region MonoBehaviour
	public override void Start()
	{
		base.Start();

		Source = GetComponent<AudioSource>();
	}

	public override void Update()
	{
		base.Update();

		// Update animations
		Animator.SetFloat( "Speed", Agent.speed );

		// Move sounds
		if ( Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "WalkFWD" )
		{
			// Swerve left and right slightly
			transform.GetChild( 0 ).localPosition = new Vector3( Mathf.Sin( Time.time * 2 ) * 0.1f, 0, 0 );

			if ( !Source.isPlaying )
			{
				Source.Play();
			}
		}
		else
		{
			Source.Stop();
		}
	}
	#endregion

	#region Actions
	public override void Attack()
	{
		base.Attack();

		GetComponentInChildren<Animator>().SetTrigger( "Attack" );
		StaticHelpers.SpawnResourceAudioSource( "skeleton_attack", transform.position, Random.Range( 0.8f, 1.2f ) );

		// Spawn projectile
		Hitbox.Spawn( false, 1, transform.position + transform.up * 1 + transform.forward * 1, transform.rotation, transform.localScale );
	}
	#endregion

	#region Health
	protected override void Die()
	{
		if ( !Killed )
		{
			base.Die();

			// Explode
			GetComponentInChildren<Animator>().SetTrigger( "Die" );
			GetComponent<Collider>().isTrigger = true;
			StaticHelpers.SpawnResourceAudioSource( "crawler_die", transform.position, Random.Range( 0.8f, 1.2f ) );
			//GameObject death = Instantiate( Resources.Load( "Prefabs/Crawler Death" ), Game.RuntimeParent ) as GameObject;
			//death.transform.position = transform.position;
			//death.transform.rotation = transform.rotation;
			//Destroy( death, 1 );

			// TODO TEMP spawn new replacement
			//Game.Instance.SpawnEnemy( Resources.Load( "Prefabs/Crawler" ) as GameObject );

			// Communicate with Game
			Game.Instance.OnEnemyKilled( this );

			// Delay particle system destruction until trail disappears completely
			var obj = GetComponentInChildren<ParticleSystem>().gameObject;
			obj.transform.parent = Game.RuntimeParent;
			Destroy( obj, 4 );

			// Destroy npc
			//gameObject.SetActive( false );
			this.enabled = false;
			Destroy( gameObject, KillDelay );
			Killed = true;
		}
	}
	#endregion
}
