using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Hitable
{
	[Header( "Variables" )]
	public float OpenDuration = 0.5f;
	public int Golds = 5;
	public int GoldValue = 5;

	[Header( "References" )]
	public Transform Lid;

	[Header( "Assets" )]
	public AudioClip OpenClip;

	private float OpenTime = -1;
	protected bool Spewed = false;

	public override void Update()
	{
		base.Update();

        if ( OpenTime != -1 )
		{
			float progress = Mathf.Clamp( ( Time.time - OpenTime ) / OpenDuration, 0, 1 );
			Lid.localRotation = Quaternion.Lerp( Quaternion.Euler( 90, 0, 0 ), Quaternion.Euler( -10, 0, 0 ), progress );

			if ( progress >= 0.5f && !Spewed )
			{
				for ( int gold = 0; gold < Golds; gold++ )
				{
					var forward = 5 + Random.Range( -0.5f, 0.5f );
					var right = ( (float) gold / Golds ) - 0.5f + Random.Range( -0.5f, 0.5f ) * 2;
					var up = 1.5f;

					var obj = StaticHelpers.SpawnPrefab( "Gold" );
					obj.transform.position = transform.position + Vector3.up * 0.5f;
					obj.GetComponent<Gold>().TargetPos = transform.position + transform.forward * forward + transform.right * right + Vector3.up * up;
					obj.GetComponent<Gold>().ParabolicSpeed *= Random.Range( 0.75f, 1.5f );
					obj.GetComponent<Gold>().Amount = GoldValue;
				}

				Spewed = true;
			}
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
