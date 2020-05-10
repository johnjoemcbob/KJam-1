using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Hitable
{
	[Header( "Variables" )]
	public float OpenDuration = 0.5f;

	[Header( "References" )]
	public Transform Lid;

	[Header( "Assets" )]
	public AudioClip OpenClip;

	private float OpenTime = -1;

    void Update()
    {
        if ( OpenTime != -1 )
		{
			float progress = Mathf.Clamp( ( Time.time - OpenTime ) / OpenDuration, 0, 1 );
			Lid.localRotation = Quaternion.Lerp( Quaternion.Euler( 90, 0, 0 ), Quaternion.Euler( -10, 0, 0 ), progress );
		}
    }

	public void Open()
	{
		if ( OpenTime != -1 ) return;

		OpenTime = Time.time;

		StaticHelpers.GetOrCreateCachedAudioSource( OpenClip, Lid.position, Random.Range( 0.8f, 1.2f ), Random.Range( 0.8f, 1 ) );
	}

	public override void OnHit( Collider other )
	{
		base.OnHit( other );

		Open();
	}
}
