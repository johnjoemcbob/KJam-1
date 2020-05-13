using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
	public static UI Instance;

	#region ==Enums
	public enum State
	{
		HUD,
		Lobby,
		Menu,
	}

	public enum LobbyState
	{
		Main,
		Store,
		Inventory,
		Win,
		//Skills,

		// Last, used by code
		AnimReal,
		Real,
	}
	#endregion

	#region ==Inspector
	[Header( "Variables" )]
	public float LobbyAnimDuration = 0.5f;

	[Header( "References" )]
	public GameObject Levels_FirstPlay;
	public GameObject Levels_SubsequentPlays;
	#endregion

	#region ==Variables
	private State CurrentState;
	private State LastState;
	private LobbyState CurrentLobbyState;
	private float SwitchLobbyTime = 0;
	#endregion

	#region ==References
	private GameObject[] UIContainers;
	private GameObject[] UILobbyContainers;
	private Transform LobbyAnimateFrom;
	private Transform LobbyAnimateTarget;
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

		// Get UI children in same order as State enum
		UIContainers = new GameObject[transform.childCount];
		foreach ( Transform child in transform )
		{
			UIContainers[child.GetSiblingIndex()] = child.gameObject;
			child.gameObject.SetActive( false );
		}

		// Get UI lobby children in same order as LobbyState enum
		Transform lobby = UIContainers[(int) State.Lobby].transform;
		UILobbyContainers = new GameObject[lobby.childCount];
		foreach ( Transform child in lobby )
		{
			UILobbyContainers[child.GetSiblingIndex()] = child.gameObject;
			child.gameObject.SetActive( false );
		}
		LobbyAnimateTarget = UILobbyContainers[0].transform;
	}

	void Start()
	{
		ResetLobbyAnim();
		SwitchLobbyState( LobbyState.Main, true );
		//OnLoad();
	}

	private void Update()
	{
		UpdateLobbyState( CurrentLobbyState );
	}
	#endregion

	#region States
	public void SwitchState( int state )
	{
		SwitchState( (State) state );
	}
	public void SwitchState( State state )
	{
		FinishState( CurrentState );
		LastState = CurrentState;
		CurrentState = state;
		StartState( CurrentState );
	}

	public void ToggleState( State state )
	{
		if ( CurrentState == state )
		{
			SwitchState( State.HUD );
		}
		else
		{
			SwitchState( state );
		}
	}

	public State GetState()
	{
		return CurrentState;
	}

	private void StartState( State state )
	{
		UIContainers[(int) state].SetActive( true );

		switch ( state )
		{
			case State.HUD:
				break;
			case State.Lobby:
				if ( SwitchLobbyTime != 0 )
				{
					ResetLobbyAnim();
					SwitchLobbyState( LobbyState.Main );
				}
				break;
			case State.Menu:
				var enable = ( LastState == State.Lobby );
				UIContainers[(int) state].transform.GetChild( 0 ).Find( "Back (Button)" ).gameObject.SetActive( enable );
				break;
			default:
				break;
		}
	}

	private void FinishState( State state )
	{
		UIContainers[(int) state].SetActive( false );

		switch ( state )
		{
			case State.HUD:
				break;
			case State.Lobby:
				//SwitchLobbyTime = 1; // Flag as needing reset (not ideal but heyho)
				break;
			case State.Menu:
				break;
			default:
				break;
		}
	}
	#endregion

	#region Lobby States
	public void SwitchLobbyState( int state )
	{
		SwitchLobbyState( (LobbyState) state );
	}
	public bool SwitchLobbyState( LobbyState state, bool first = false, bool force = false )
	{
		if ( SwitchLobbyTime != 0 && !force ) return false;

		FinishLobbyState( CurrentLobbyState );
		CurrentLobbyState = state;
		StartLobbyState( CurrentLobbyState, first );

		OnLoad();
		StaticHelpers.GetOrCreateCachedAudioSource( "ui_swoosh", true, Random.Range( 0.8f, 1.2f ) );

		return true;
	}

	private void StartLobbyState( LobbyState state, bool first = false )
	{
		SwitchLobbyTime = Time.time;
		{
			// Always show the real
			var realanim = UILobbyContainers[(int) LobbyState.AnimReal];
			realanim.SetActive( true );
			var real = UILobbyContainers[(int) LobbyState.Real];
			real.SetActive( true );

			if ( first )
			{
				realanim = real;
			}
			//else
			//{
			// // This was an attempt to make the return to the lobby rotate from transparency
			//	foreach ( Transform section in real.transform )
			//	{
			//		section.transform.localPosition += new Vector3( 0, 1024, 0 );
			//	}
			//}

			// Populate the back of the real
			var copy = UILobbyContainers[(int) state];
			foreach ( Transform section in copy.transform )
			{
				foreach ( Transform child in section )
				{
					var obj = Instantiate( child.gameObject, realanim.transform.GetChild( section.GetSiblingIndex() ) );
					//obj.transform.localEulerAngles = new Vector3( 0, 180, 0 );
				}
				section.localEulerAngles = new Vector3( 0, 180, 0 );
			}
			LobbyAnimateFrom = LobbyAnimateTarget;
			LobbyAnimateTarget = copy.transform;
		}
	}

	private void UpdateLobbyState( LobbyState state )
	{
		// Lerp to current target
		if ( SwitchLobbyTime != 0 )
		{
			var realanim = UILobbyContainers[(int) LobbyState.AnimReal];
			var real = UILobbyContainers[(int) LobbyState.Real];
			float progress = ( ( Time.time - SwitchLobbyTime ) / LobbyAnimDuration );
			progress = Mathf.Clamp( progress, 0, 1 );
			float halfprogress = Mathf.Clamp( progress * 2, 0, 1 );

			// Resize the reals
			foreach ( Transform section in real.transform )
			{
				RectTransform from = LobbyAnimateFrom.transform.GetChild( section.GetSiblingIndex() ) as RectTransform;
				RectTransform targ = LobbyAnimateTarget.transform.GetChild( section.GetSiblingIndex() ) as RectTransform;
				RectTransform rect = section as RectTransform;
				rect.position = Vector3.Lerp( from.position, targ.position, progress );
				rect.sizeDelta = Vector2.Lerp( from.sizeDelta, targ.sizeDelta, Mathf.Max( 0, halfprogress - 0.5f ) * 2 );
			}
			foreach ( Transform section in realanim.transform )
			{
				RectTransform from = LobbyAnimateFrom.transform.GetChild( section.GetSiblingIndex() ) as RectTransform;
				RectTransform targ = LobbyAnimateTarget.transform.GetChild( section.GetSiblingIndex() ) as RectTransform;
				RectTransform rect = section as RectTransform;
				rect.position = Vector3.Lerp( from.position, targ.position, progress );
				rect.sizeDelta = Vector2.Lerp( from.sizeDelta, targ.sizeDelta, Mathf.Max( 0, halfprogress - 0.5f ) * 2 );
			}

			// Flip both gradually
			var zero = new Vector3( 0, 0, 0 );
			var opposite = new Vector3( 0, 180, 0 );
			realanim.transform.rotation = Quaternion.Lerp( Quaternion.Euler( opposite ), Quaternion.Euler( zero ), progress );
			real.transform.rotation = Quaternion.Lerp( Quaternion.Euler( zero ), Quaternion.Euler( opposite ), progress );

			// Switch scene order at half way
			if ( progress >= 0.5f )
			{
				if ( realanim.transform.GetSiblingIndex() < real.transform.GetSiblingIndex() )
				{
					realanim.transform.SetAsLastSibling();

					// Clear the real
					foreach ( Transform section in real.transform )
					{
						foreach ( Transform child in section )
						{
							Destroy( child.gameObject );
						}
					}

				}
			}

			// Finish
			if ( progress >= 1 )
			{
				FinishLobbyAnim();
			}
		}
	}

	private void FinishLobbyState( LobbyState state )
	{
		//UILobbyContainers[(int) state].SetActive( false );

		switch ( state )
		{
			case LobbyState.Main:
				break;
			case LobbyState.Store:
				break;
			case LobbyState.Inventory:
				break;
			default:
				break;
		}
	}

	private void ResetLobbyAnim()
	{
		SwitchLobbyTime = 0;

		var realanim = UILobbyContainers[(int) LobbyState.AnimReal];
		var real = UILobbyContainers[(int) LobbyState.Real];

		// Clear all panels first
		foreach ( Transform section in real.transform )
		{
			foreach ( Transform child in section )
			{
				Destroy( child.gameObject );
			}
		}
		foreach ( Transform section in realanim.transform )
		{
			foreach ( Transform child in section )
			{
				Destroy( child.gameObject );
			}
		}
	}

	private void FinishLobbyAnim()
	{
		SwitchLobbyTime = 0;
		SwapLobbyAnimLayers();
	}

	private void SwapLobbyAnimLayers()
	{
		var realanim = UILobbyContainers[(int) LobbyState.AnimReal];
		var real = UILobbyContainers[(int) LobbyState.Real];

		// Swap real and animreal
		GameObject temp = UILobbyContainers[(int) LobbyState.AnimReal];
		UILobbyContainers[(int) LobbyState.AnimReal] = UILobbyContainers[(int) LobbyState.Real];
		UILobbyContainers[(int) LobbyState.AnimReal].name = "AnimReal";
		UILobbyContainers[(int) LobbyState.Real] = temp;
		UILobbyContainers[(int) LobbyState.Real].name = "Real";
	}
	#endregion

	#region On Load
	public void OnLoad()
	{
		StartCoroutine( DelayOnLoad() );
	}
	// Running out of time sorry!
	// The menu animation/cloning fucks up the loading here
	public IEnumerator DelayOnLoad()
	{
		yield return new WaitForEndOfFrame();

		// Progression/levels
		bool first = ( Player.Instance.Data.LevelsPlayed <= 0 );

		Vector3 zero = Vector3.zero;
		Vector3 off = new Vector3( 0, 1000, 0 );

		Levels_FirstPlay.transform.localPosition = first ? zero : off;
		Levels_SubsequentPlays.transform.localPosition = !first ? zero : off;
		var obj = GameObject.Find( "FirstPlay (Container)(Clone)" );
		if ( obj != null )
		{
			obj.transform.localPosition = first ? zero : off;
			GameObject.Find( "SubsequentPlays (Container)(Clone)" ).transform.localPosition = !first ? zero : off;
		}
	}
	#endregion

	public bool ShouldShowCursor()
	{
		return CurrentState != State.HUD;
	}
}
