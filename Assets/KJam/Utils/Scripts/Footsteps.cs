using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
	[Header( "Variables" )]
	public Vector2 PitchA;
	public Vector2 PitchB;
	public Vector2 Volume;

	[Header( "Referenecs" )]
	public AudioClip Clip;
	public Transform[] Feet;

	private bool Left = false;
	private Player Player;

	private void Start()
	{
		Player = GetComponentInParent<Player>();
	}

	// Called from Animator Events
	public void Step()
	{
		Left = !Left;

		bool grounded = true;
			if ( Player != null )
			{
				grounded = Player.Grounded;
			}
		if ( grounded )
		{
			int index = Left ? 0 : 1;
			Vector2 pitch = Left ? PitchA : PitchB;
			StaticHelpers.GetOrCreateCachedAudioSource( Clip, Feet[index].position, Random.Range( pitch.x, pitch.y ), Random.Range( Volume.x, Volume.y ) );
			var dust = StaticHelpers.EmitParticleImpact( Feet[index].position );
			if ( dust != null )
			{
				dust.transform.localScale = Vector3.one * 0.2f;
			}
		}
	}
}
