using UnityEngine;
using System.Collections.Generic;

public class ParticleSounds : MonoBehaviour
{
	public AudioClip OnCreate;
	//public AudioClip OnImpact; // This part doesn't work teehee

	private ParticleSystem part;
	private List<ParticleCollisionEvent> collisionEvents;

	void Start()
	{
		part = GetComponent<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();

		StaticHelpers.GetOrCreateCachedAudioSource( OnCreate, transform.position, Random.Range( 0.8f, 1.2f ), 1 );
	}

	//void OnParticleCollision( GameObject other )
	//{
	//	int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
	//	int i = 0;

	//	while ( i < numCollisionEvents )
	//	{
	//		StaticHelpers.SpawnAudioSource( OnImpact, collisionEvents[i].intersection, Random.Range( 2.0f, 3.8f ), Random.Range( 0.8f, 1.0f ) );
	//		i++;
	//	}
	//}
}