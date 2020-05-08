using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent( typeof(NavMeshAgent) )]
public class BaseEnemy : Killable
{
	public float AttackRange = 1.5f;

	protected float NextAttack = 0;

	protected NavMeshAgent Agent;

	#region MonoBehaviour
	public virtual void Start()
    {
		Agent = GetComponent<NavMeshAgent>();
    }

    public virtual void Update()
    {
		//if ( Input.GetMouseButtonDown( 0 ) )
		//{
		//	var ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		//	RaycastHit hit;
		//	if ( Physics.Raycast( ray, out hit ) )
		//	{
		//		Agent.SetDestination( hit.point );
		//	}
		//}

		// This should still work for non-navmesh areas, but tables have to be jumped on to
		Vector3 playerpos = FindObjectOfType<Player>().transform.position;
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

			if ( NextAttack <= Time.time )
			{
				transform.localEulerAngles = new Vector3( 0, transform.localEulerAngles.y, 0 );
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
	protected override void Die()
	{
		base.Die();
	}
	#endregion
}
