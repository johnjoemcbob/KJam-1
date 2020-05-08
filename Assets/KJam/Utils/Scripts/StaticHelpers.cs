using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Main class with helpers
public class StaticHelpers
{
	#region Statics
	public static GameObject EmitParticleImpact( Vector3 point )
	{
		GameObject particle = GameObject.Instantiate( Resources.Load( "Prefabs/Particle Effect" ) ) as GameObject;
		{
			particle.transform.position = point;

			GameObject.Destroy( particle, 1.5f );
		}
		return particle;
	}

	public static GameObject EmitParticleDust( Vector3 point )
	{
		GameObject particle = GameObject.Instantiate( Resources.Load( "Prefabs/Particle Dust" ) ) as GameObject;
		{
			particle.transform.position = point;

			GameObject.Destroy( particle, 2 );
		}
		return particle;
	}

	public static GameObject SpawnPrefab( string name, Vector3 pos, Quaternion rot, Vector3 scale )
	{
		GameObject prefab = GameObject.Instantiate( Resources.Load( "Prefabs/" + name ) as GameObject );
		{
			prefab.transform.position = pos;
			prefab.transform.rotation = rot;
			prefab.transform.localScale = scale;
		}
		return prefab;
	}

	public static GameObject SpawnResourceAudioSource( string clipname, Vector3 point, float pitch = 1, float volume = 1, float delay = 0 )
	{
		AudioClip clip = Resources.Load( "Sounds/" + clipname ) as AudioClip;
		return SpawnAudioSource( clip, point, pitch, volume, delay );
	}

	public static GameObject SpawnAudioSource( AudioClip clip, Vector3 point, float pitch = 1, float volume = 1, float delay = 0 )
	{
		GameObject source = GameObject.Instantiate( Resources.Load( "Prefabs/Audio Source" ) ) as GameObject;
		{
			source.transform.position = point;

			AudioSource audio = source.GetComponent<AudioSource>();
			audio.clip = clip;
			audio.pitch = pitch;
			audio.volume = volume;
			audio.PlayDelayed( delay );

			GameObject.Destroy( source, clip.length + 0.1f );
		}
		return source;
	}
	#endregion
}
