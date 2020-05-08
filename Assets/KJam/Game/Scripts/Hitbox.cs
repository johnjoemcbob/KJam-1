using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
	public bool PlayerTeam;
	public float Damage;

	public static GameObject Spawn( bool player, float damage, Vector3 pos, Quaternion rot, Vector3 scale )
	{
		var hitbox = StaticHelpers.SpawnPrefab( "Hitbox", pos, rot, scale );
		{
			// Set hitbox info
			var hit = hitbox.GetComponent<Hitbox>();
			hit.PlayerTeam = player;
			hit.Damage = damage;

			// Timer to destroy soon
			Destroy( hitbox, 0.1f );
		}
		return hitbox;
	}
}
