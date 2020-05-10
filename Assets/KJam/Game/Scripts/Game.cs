using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	public const float MATCHTOLOBBY_ANIM_TIME = 3.5f;

	public static Game Instance;
	public static Transform RuntimeParent;
	public static Volume PostProcessing;
	public static ChromaticAberration ChromAb;
	public static ColorAdjustments ColourAdj;
	public static LensDistortion LensDis;

	#region ==Enums
	public enum State
	{
		Lobby,
		Match,
		MatchToLobbyAnim,
	}
	#endregion

	#region ==Inspector
	[Header( "References" )]
	public Text HUDMessage;
	#endregion

	#region ==Variables
	protected float CurrentStateTime = 0;
	protected State CurrentState;

	protected List<BaseEnemy> CurrentEnemies;
	#endregion

	#region MonoBehaviour
	void Start()
    {
		Instance = this;
		RuntimeParent = GameObject.Find( "Runtime" ).transform;
		PostProcessing = FindObjectOfType<Volume>();
		PostProcessing.profile.TryGet( out ChromAb );
		PostProcessing.profile.TryGet( out ColourAdj );
		PostProcessing.profile.TryGet( out LensDis );

		HUDMessage.color = new Color( HUDMessage.color.r, HUDMessage.color.g, HUDMessage.color.b, 0 );

		StaticHelpers.Reset();

		StartState( State.Lobby );
    }

    void Update()
    {
		UpdateState( CurrentState );

		// Post processes
		ChromAb.intensity.value = Mathf.Lerp( ChromAb.intensity.value, 0, Time.deltaTime );
		float target = 30;
			if ( CurrentState == State.MatchToLobbyAnim )
			{
				target = -100;
			}
		ColourAdj.saturation.value = Mathf.Lerp( ColourAdj.saturation.value, target, Time.deltaTime );
		LensDis.intensity.value = Mathf.Lerp( LensDis.intensity.value, 0, Time.deltaTime );

		// Testing
		//if ( Input.GetKeyDown( KeyCode.T ) )
		//{
		//	SwitchState( State.Match );
		//	//UI.Instance.SwitchState( UI.State.HUD );
		//}
		//if ( Input.GetKeyDown( KeyCode.Y ) )
		//{
		//	SwitchState( State.Lobby );
		//	//UI.Instance.SwitchState( UI.State.Lobby );
		//}
		//if ( Input.GetKeyDown( KeyCode.U ) )
		//{
		//	bool sucess = UI.Instance.SwitchLobbyState( UI.LobbyState.Main );
		//}
		//if ( Input.GetKeyDown( KeyCode.O ) )
		//{
		//	bool sucess = UI.Instance.SwitchLobbyState( UI.LobbyState.Store );
		//}
		//if ( Input.GetKeyDown( KeyCode.P ) )
		//{
		//	bool sucess = UI.Instance.SwitchLobbyState( UI.LobbyState.Inventory );
		//}
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
		CurrentStateTime = 0;

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
				LoadLevel();
				// Spawn player on character menu spot
				// Smooth camera
				break;
			case State.MatchToLobbyAnim:
				break;
			default:
				break;
		}
	}

	private void UpdateState( State state )
	{
		CurrentStateTime += Time.deltaTime;

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
			case State.MatchToLobbyAnim:
				if ( CurrentStateTime <= MATCHTOLOBBY_ANIM_TIME / 3 )
				{
					// Animate in
					float progress = CurrentStateTime / ( MATCHTOLOBBY_ANIM_TIME / 3 );
					HUDMessage.color = new Color( HUDMessage.color.r, HUDMessage.color.g, HUDMessage.color.b, progress );
				}
				else if ( CurrentStateTime >= MATCHTOLOBBY_ANIM_TIME / 3 * 2 )
				{
					// Animate out
					float progress = ( CurrentStateTime - ( MATCHTOLOBBY_ANIM_TIME / 3 * 2 ) ) / ( MATCHTOLOBBY_ANIM_TIME / 3 );
					HUDMessage.color = new Color( HUDMessage.color.r, HUDMessage.color.g, HUDMessage.color.b, 1 - progress );
				}
				// End
				if ( CurrentStateTime >= MATCHTOLOBBY_ANIM_TIME )
				{
					SwitchState( State.Lobby );
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
				//UnloadLevel();
				Player.Instance.Controllable = false;

				break;
			case State.MatchToLobbyAnim:
				UnloadLevel();
				Player.Instance.gameObject.SetActive( false );
				SceneManager.LoadSceneAsync( 0 );

				break;
			default:
				break;
		}
	}

	public State GetState()
	{
		return CurrentState;
	}
	#endregion

	#region Level
	public void LoadLevel()
	{
		// Load level into runtime
		StaticHelpers.SpawnResource( "Levels/TestLevel" );

		// Store all enemies
		CurrentEnemies = new List<BaseEnemy>( FindObjectsOfType<BaseEnemy>() );
	}

	public void UnloadLevel()
	{
		// Clear runtime
		foreach ( Transform child in RuntimeParent )
		{
			Destroy( child.gameObject );
		}
		StaticHelpers.Reset();
	}
	#endregion

	#region Win/Lose
	public void Win()
	{
		SwitchState( State.MatchToLobbyAnim );
		HUDMessage.text = "Mission success!";

		StaticHelpers.GetOrCreateCachedAudioSource( "win", true );
	}

	public void Lose()
	{
		SwitchState( State.MatchToLobbyAnim );
		HUDMessage.text = "Mission failed...";

		StaticHelpers.GetOrCreateCachedAudioSource( "lose", true );
	}
	#endregion

	#region Player
	public void OnPlayerDie()
	{
		if ( CurrentState == State.Match )
		{
			Lose();
		}
	}
	#endregion

	#region Enemies
	public void OnEnemyKilled( BaseEnemy enemy )
	{
		CurrentEnemies.Remove( enemy );

		if ( CurrentEnemies.Count == 0 && CurrentState == State.Match )
		{
			Win();
		}

		// Temp
		Player.Instance.AddGold( 10 );
	}

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
