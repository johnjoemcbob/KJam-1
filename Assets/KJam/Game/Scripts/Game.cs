using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Game : MonoBehaviour
{
	public static Game Instance;
	public static Transform RuntimeParent;

	#region ==Enums
	public enum State
	{
		Lobby,
		Match,
	}
	#endregion

	#region ==Variables
	protected State CurrentState;
	#endregion

	#region MonoBehaviour
	void Start()
    {
		Instance = this;
		RuntimeParent = GameObject.Find( "Runtime" ).transform;

		StartState( State.Lobby );
    }

    void Update()
    {
		UpdateState( CurrentState );

		// Testing
		if ( Input.GetKeyDown( KeyCode.T ) )
		{
			SwitchState( State.Match );
			//UI.Instance.SwitchState( UI.State.HUD );
		}
		if ( Input.GetKeyDown( KeyCode.Y ) )
		{
			SwitchState( State.Lobby );
			//UI.Instance.SwitchState( UI.State.Lobby );
		}
		if ( Input.GetKeyDown( KeyCode.U ) )
		{
			bool sucess = UI.Instance.SwitchLobbyState( UI.LobbyState.Main );
		}
		if ( Input.GetKeyDown( KeyCode.O ) )
		{
			bool sucess = UI.Instance.SwitchLobbyState( UI.LobbyState.Store );
		}
		if ( Input.GetKeyDown( KeyCode.P ) )
		{
			bool sucess = UI.Instance.SwitchLobbyState( UI.LobbyState.Inventory );
		}
	}
	#endregion

	#region States
	public void SwitchState( int newstate )
	{
		SwitchState( (State) newstate );
	}
	public void SwitchState( State newstate )
	{
		FinishState( CurrentState );
		CurrentState = newstate;
		StartState( CurrentState );
	}

	private void StartState( State state )
	{
		switch ( state )
		{
			case State.Lobby:
				// Show menu
				UI.Instance.SwitchState( UI.State.Lobby );
				Player.Instance.Controllable = false;
				break;
			case State.Match:
				UI.Instance.SwitchState( UI.State.HUD );
				Player.Instance.Controllable = true;
				// Spawn enemies
				// Spawn player on character menu spot
				// Smooth camera
				break;
			default:
				break;
		}
	}

	private void UpdateState( State state )
	{
		switch ( state )
		{
			case State.Lobby:
				break;
			case State.Match:
				if ( Input.GetKeyDown( KeyCode.Tab ) )
				{
					UI.Instance.ToggleState( UI.State.Menu );
				}

				break;
			default:
				break;
		}
	}

	private void FinishState( State state )
	{
		switch ( state )
		{
			case State.Lobby:
				break;
			case State.Match:
				// Delete any runtimes
				// Including player
				break;
			default:
				break;
		}
	}
	#endregion

	#region Level
	public void SpawnEnemy( GameObject prefab )
	{
		GameObject enemy = Instantiate( prefab, RuntimeParent );
		enemy.transform.position = GetRandomPoint( Vector3.zero, 100 );
	}

	// From: https://gist.github.com/IJEMIN/f2510a85b1aaf3517da1af7a6f9f1ed3
	public static Vector3 GetRandomPoint( Vector3 center, float maxDistance )
	{
		// Get Random Point inside Sphere which position is center, radius is maxDistance
		Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

		NavMeshHit hit; // NavMesh Sampling Info Container

		// from randomPos find a nearest point on NavMesh surface in range of maxDistance
		NavMesh.SamplePosition( randomPos, out hit, maxDistance, NavMesh.AllAreas );

		return hit.position;
	}
	#endregion
}
