using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
	[Header( "Variables" )]
	public bool PlayerTeam;
	public float Damage;

	[HideInInspector]
	public bool DestroyNextFrame = false;

	private List<Transform> HasHit = new List<Transform>();

	public void LateUpdate()
	{
		if ( DestroyNextFrame )
		{
			Destroy( gameObject );
		}
	}

	public bool CanHit( Transform other )
	{
		var hit = other.GetComponent<Hitable>();
		return !HasHit.Contains( other ) && hit && PlayerTeam != ( other == Player.Instance.transform );
	}

	public void Hit( Transform other )
	{
		HasHit.Add( other );

		// First hit of players always makes a noise
		if ( HasHit.Count == 1 && PlayerTeam == true )
		{
			StaticHelpers.SpawnResourceAudioSource( "skeleton_attack", transform.position, Random.Range( 0.8f, 1.2f ) );
		}
	}

	public static GameObject Spawn( bool player, float damage, Vector3 pos, Quaternion rot, Vector3 scale )
	{
		var hitbox = StaticHelpers.SpawnPrefab( "Hitbox", pos, rot, scale );
		{
			// Set hitbox info
			var hit = hitbox.GetComponent<Hitbox>();
			hit.PlayerTeam = player;
			hit.Damage = damage;

			// Timer to destroy soon
			Destroy( hitbox, Time.deltaTime * 5 );
		}
		return hitbox;
	}
}
