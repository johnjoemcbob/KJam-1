using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
	public enum State
	{
		In,
		Stay,
		Out
	}

	[Header( "Variables" )]
	public float DurationIn = 0.5f;
	public float DurationStay = 0.5f;
	public float DurationOut = 0.5f;
	public float UpSpeed = 0.1f;
	public Color[] MainColours;
	public Color[] OutlineColours;

	[Header( "References" )]
	public Text Text;
	public Outline[] Outlines;

	protected float CurrentTime = 0;
	protected State CurrentState;
	protected int Team;

	private void OnEnable()
	{
		CurrentTime = 0;
		CurrentState = State.In;
	}

	private void Update()
	{
		// Face camera
		transform.LookAt( Camera.main.transform );

		// Lerp position up
		if ( CurrentState != State.Stay )
		{
			transform.localPosition += Vector3.up * Time.deltaTime * UpSpeed;
		}
		transform.GetChild( 0 ).localPosition = transform.GetChild( 0 ).right * Mathf.Sin( Time.time ) * 0.05f +
			transform.GetChild( 0 ).up * Mathf.Cos( Time.time * 2 ) * 0.05f;

		// Lerp colour in/out
		CurrentTime += Time.deltaTime;
		switch ( CurrentState )
		{
			case State.In:
				// Update
				LerpColours( CurrentTime / DurationIn );

				// End
				if ( CurrentTime >= DurationIn )
				{
					CurrentTime = 0;
					CurrentState = State.Stay;
				}

				break;
			case State.Stay:
				// End
				if ( CurrentTime >= DurationStay )
				{
					CurrentTime = 0;
					CurrentState = State.Out;
				}

				break;
			case State.Out:
				// Update
				LerpColours( 1 - ( CurrentTime / DurationOut ) );

				// End
				if ( CurrentTime >= DurationOut )
				{
					CurrentTime = 0;
					CurrentState = State.In;
					gameObject.SetActive( false );
				}

				break;
			default:
				break;
		}
	}

	private void LerpColours( float progress )
	{
		Text.color = Color.Lerp( new Color( 0, 0, 0, 0 ), MainColours[Team], progress );
		foreach ( var outline in Outlines )
		{
			outline.effectColor = Color.Lerp( new Color( 0, 0, 0, 0 ), OutlineColours[Team], progress );
		}
	}

	public void Init()
	{
		// Randomise location a little to be around the head
		float off = 0.4f;
		float up = 0.6f;
		transform.position += transform.right * Random.Range( -off, off ) +
			transform.forward * Random.Range( -off, off ) +
			transform.up * Random.Range( -up, up );
	}

	public void SetDamage( float damage )
	{
		// Update ui numbers
		Text.text = "-" + damage;
	}

	public void SetTeam( bool player )
	{
		Team = player ? 0 : 1;
	}
}
