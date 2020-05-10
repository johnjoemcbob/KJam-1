using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
	[Header( "Variables" )]
	public bool PlayerTeam;
	public float Damage;

	private List<Transform> HasHit = new List<Transform>();

	private void OnEnable()
	{
		HasHit = new List<Transform>();
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
			StaticHelpers.GetOrCreateCachedAudioSource( "skeleton_attack", transform.position, Random.Range( 0.8f, 1.2f ) );
		}
	}

	public static GameObject Spawn( bool player, float damage, Vector3 pos, Quaternion rot, Vector3 scale )
	{
		var hitbox = StaticHelpers.GetOrCreateCachedPrefab( "Hitbox", pos, rot, scale, 1 );// Time.deltaTime * 5 );
		{
			// Set hitbox info
			var hit = hitbox.GetComponent<Hitbox>();
			hit.PlayerTeam = player;
			hit.Damage = damage;
		}
		return hitbox;
	}
}
