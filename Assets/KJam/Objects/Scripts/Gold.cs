using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
	[Header( "Variables" )]
	public float BobSpeed = 5;
	public float BobAmount = 0.5f;
	public float LerpSpeed = 5;
	public float LerpAccel = 5;
	public float RotateSpeed = 1;
	public float BagSpeed = 5;
	public float BagAccel = 5;
	public float ParabolicSpeed = 1;
	public float ParabolicHeight = 1;

	[HideInInspector]
	public int Amount = 1; // Of gold
	protected bool Touched = false;
	protected bool Bagged = false;
	protected bool Parabolad = false;
	protected static Transform Bag;
	protected Vector3 StartPos;
	[HideInInspector]
	public Vector3 TargetPos;

	private void Start()
	{
		if ( Bag == null )
		{
			Bag = GameObject.Find( "GOLDBAG" ).transform;
		}

		StartPos = transform.position;
		//TargetPos = transform.position + transform.forward * 5;
	}

	private void Update()
	{
		var visual = transform.GetChild( 0 );

		if ( Parabolad )
		{
			// Bob up and down
			visual.localPosition = new Vector3( 0, Mathf.Sin( Time.time * BobSpeed ) * BobAmount, 0 );
		}

		// Lerp towards player
		if ( Touched )
		{
			if ( !Bagged )
			{
				// Catch up to player, even if they are very fast
				LerpSpeed += Time.deltaTime * LerpAccel;

				transform.position = Vector3.Lerp( transform.position, Bag.position, Time.deltaTime * LerpSpeed );
				if ( Vector3.Distance( transform.position, Bag.position ) < 0.2f )
				{
					Bagged = true;
				}
			}
			else
			{
				// Catch up to player, even if they are very fast
				BagSpeed += Time.deltaTime * BagAccel;

				transform.position = Vector3.Lerp( transform.position, Bag.position, Time.deltaTime * BagSpeed );
				visual.localScale = Vector3.Lerp( visual.localScale, Vector3.zero, Time.deltaTime * BagSpeed );

				if ( visual.localScale.x < 0.01f )
				{
					Player.Instance.AddGold( Amount );
					Destroy( gameObject );
				}
			}
			//transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.Euler( transform.eulerAngles + new Vector3( 90, 180, 0 ) ), Time.deltaTime * RotateSpeed );
		}
		else if ( !Parabolad )
		{
			ParabolicMove();
		}
		else
		{
			transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.Euler( new Vector3( 0, 0, 0 ) ), Time.deltaTime * RotateSpeed );
		}
	}

	private void OnTriggerEnter( Collider other )
	{
		if ( !Touched && other.GetComponent<Player>() )
		{
			GetComponent<Punchable>().Punch();
			Touched = true;
		}
	}

	// From: http://luminaryapps.com/blog/arcing-projectiles-in-unity/
	private void ParabolicMove()
	{
		// Compute the next position, with arc added in
		float x0 = StartPos.x;
		float x1 = TargetPos.x;
		float z0 = StartPos.z;
		float z1 = TargetPos.z;
		float dist = Mathf.Abs( x1 - x0 ) + Mathf.Abs( z1 - z0 );
		float nextX = Mathf.MoveTowards( transform.position.x, x1, Time.deltaTime * ParabolicSpeed );
		float nextZ = Mathf.MoveTowards( transform.position.z, z1, Time.deltaTime * ParabolicSpeed );
		float baseY = Mathf.Lerp( TargetPos.y, TargetPos.y, ((nextX - x0)+(nextZ - z0)) / dist );
		float arc = ParabolicHeight * ( (nextX - x0) * (nextX - x1) + (nextZ - z0) * (nextZ - z1) ) / (-0.25f * dist * dist);
		var nextPos = new Vector3( nextX, baseY + arc, nextZ );

		// Rotate to face the next position, and then move there
		if ( dist != 0 )
		{
			transform.LookAt( nextPos - transform.position );
			transform.position = nextPos;
		}

		if ( nextPos == TargetPos )
		{
			Parabolad = true;
		}
		else if ( dist == 0 )
		{
			// Straight vertical
			transform.position = Vector3.Lerp( transform.position, TargetPos, Time.deltaTime * ParabolicSpeed );
		}
	}
}
