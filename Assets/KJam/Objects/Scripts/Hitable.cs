using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour
{
	[Header( "Variables" )]
	public bool PunchWhenHit = true;

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
	}

	private void OnTriggerStay( Collider other )
	{
		var hitbox = other.GetComponent<Hitbox>();
		if ( hitbox && hitbox.CanHit( transform ) )
		{
			OnHit( other );
			hitbox.Hit( transform );
		}
	}

	public virtual void OnHit( Collider other )
	{
		if ( PunchWhenHit )
		{
			GetComponentInChildren<Punchable>().Punch();
		}
	}
}
