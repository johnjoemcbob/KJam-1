using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Killable
{
	public static Player Instance;

	public const float DEADZONE = 0.1f;

	#region ==Inspector
	public float Speed = 10.0f;
	public float RotationSpeed = 2.0f;

	public float GroundedExtraAllowance = 0.5f;
	public float JumpSpeed = 15.0f;
	public float Gravity = 30.0f;

	public float maxHeadRotation = 80.0f;
	public float minHeadRotation = -80.0f;

	[Header( "References" )]
	public Transform Root;
	public Transform head;

	[Header( "Assets" )]
	public AudioClip JumpClip;
	public AudioClip SwordClip;
	#endregion

	#region ==Variables
	[HideInInspector]
	public SaveInfo Data;
	[HideInInspector]
	public List<BaseItem> Items = new List<BaseItem>();
	[HideInInspector]
	public Dictionary<string, int> EquippedItems = new Dictionary<string, int>();
	[HideInInspector]
	public bool Grounded;
	[HideInInspector]
	public bool Controllable = true;

	private float LastGrounded = 0;
	private float currentHeadRotation = 0;
	private float yVelocity = 0;
	private Vector3 TargetVelocity = Vector3.zero;
	private Vector3 moveVelocity = Vector3.zero;

	#endregion

	#region ==References
	private CharacterController controller;
	private Animator animator;
	private Transform BoomTrans;
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;
	}

	public override void Start()
	{
		base.Start();

		controller = GetComponent<CharacterController>();
		animator = GetComponentInChildren<Animator>();
		BoomTrans = GetComponentInChildren<CameraControls>().transform.parent;

		SaveLoad.Load();
	}

	void Update()
	{
		animator.SetBool( "Controllable", Controllable );

		if ( Controllable )
		{
			// Input - Jump
			Grounded = IsGrounded();
			if ( Grounded && yVelocity <= 0 )
			{
				yVelocity = -Gravity * Time.deltaTime * 5;

				// Not the best way but heyho
				// TODO need to be dependant on air time?
				//if ( LastGrounded < Time.time - Time.deltaTime * 2 )
				//{
				//	StaticHelpers.EmitParticleDust( transform.position );
				//}

				LastGrounded = Time.time;
			}
			else
			{
				// Gravity
				yVelocity -= Gravity * Time.deltaTime;
			}
			if ( Grounded || LastGrounded + GroundedExtraAllowance >= Time.time )
			{
				if ( Input.GetButtonDown( "Jump" ) )
				{
					yVelocity = JumpSpeed;
					LastGrounded = 0;
					Jump();
				}
				else
				{
					animator.SetTrigger( "Grounded" );
				}
			}
			animator.SetFloat( "FallSpeed", Grounded ? 0 : yVelocity );

			// Input - Horizontal
			TargetVelocity = BoomTrans.TransformDirection( new Vector3( Input.GetAxisRaw( "Horizontal" ), 0, 0 ) ) * Speed;
			var vertical = BoomTrans.transform.TransformDirection( new Vector3( 0, 0, Input.GetAxisRaw( "Vertical" ) ) );
			vertical.y = 0;
			TargetVelocity += vertical.normalized * Speed;
			moveVelocity = Vector3.Lerp( moveVelocity, TargetVelocity, Time.deltaTime * Speed );
			moveVelocity.y = yVelocity;

			// Attack test
			if ( Input.GetMouseButtonDown( 0 ) )
			{
				Attack();
			}

			// Velocity
			Vector3 velocity = moveVelocity + yVelocity * Vector3.up;
			Vector3 horizontalvel = new Vector3( velocity.x, 0, velocity.z );

			// Update animator
			animator.SetFloat( "RunSpeed", horizontalvel.magnitude );

			// Undo rootmotion
			animator.transform.localPosition = Vector3.zero;

			if ( velocity.magnitude >= DEADZONE )
			{
				controller.Move( velocity * Time.deltaTime );

				// Face character model at velocity direction
				Root.LookAt( Root.position + horizontalvel );
				Root.localEulerAngles = new Vector3( 0, Root.localEulerAngles.y, 0 );
			}

			// Look sine (TESTING)
			currentHeadRotation = Mathf.Clamp( currentHeadRotation + Mathf.Sin( Time.time ) * RotationSpeed, minHeadRotation, maxHeadRotation );
			// Update head
			foreach ( Transform child in head )
			{
				child.localRotation = Quaternion.identity;
				child.Rotate( Vector3.left, currentHeadRotation );
			}
		}
		else
		{
			// Update head - Look at cursor
			Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
			var pos = Camera.main.ScreenToWorldPoint( mouse ) + new Vector3( Camera.main.transform.localPosition.x, 0, 0 ) / 2;
			Debug.DrawLine( Camera.main.transform.position, pos, Color.red, 5 );
			foreach ( Transform child in head )
			{
				child.localRotation = Quaternion.identity;
				child.LookAt( pos );
				child.Rotate( child.right, 90 );
				child.Rotate( child.forward, 90 );
			}

			// Look at camera
			Root.LookAt( Camera.main.transform );
			Root.localEulerAngles = new Vector3( 0, Root.localEulerAngles.y, 0 );
		}
	}

	private void OnDestroy()
	{
		SaveLoad.Save();
	}
	#endregion

	#region States
	// TODO
	#endregion

	#region Actions
	private void Attack()
	{
		animator.SetTrigger( "Attack" + 1 );// + Mathf.RoundToInt( Random.Range( 1, 3 ) ) );

		StaticHelpers.SpawnAudioSource( SwordClip, transform.position, Random.Range( 0.8f, 1.2f ), 1 );

		// Spawn the hitbox
		var animtrans = animator.transform;
		Hitbox.Spawn( true, 1, animtrans.position + animtrans.up * 1 + animtrans.forward * 1, animtrans.rotation, animtrans.localScale );

		// todo temp
		// Find all enemies and apply damage
		//foreach ( var enemy in FindObjectsOfType<BaseEnemy>() )
		//{
		//	enemy.TakeDamage( 10 );
		//}
		AddGold( 10 );
	}

	private void Jump()
	{
		animator.SetTrigger( "Jump" );

		StaticHelpers.SpawnAudioSource( JumpClip, transform.position, Random.Range( 0.8f, 1.2f ), 0.5f );
		StaticHelpers.EmitParticleDust( transform.position );
	}
	#endregion

	#region Health
	protected override void Die()
	{
		base.Die();

		// Communicate with main Game class
		// Game.Instance
		gameObject.SetActive( false );
		SceneManager.LoadSceneAsync( 0 );
	}
	#endregion

	#region Gold
	public float AddGold( float add )
	{
		Data.Gold = Mathf.Max( 0, Data.Gold + add );
		return Data.Gold;
	}

	public bool HasGold( float gold )
	{
		return Data.Gold >= gold;
	}

	public float GetGold()
	{
		return Data.Gold;
	}
	#endregion

	#region Inventory
	public void AddItem( BaseItem item )
	{
		Items.Add( item );
	}

	public List<BaseItem> GetInventory()
	{
		return Items;
	}
	#endregion

	#region Equipped Items
	public void Equip( string type, BaseItem item )
	{
		if ( !EquippedItems.ContainsKey( type ) )
		{
			EquippedItems.Add( type, Items.IndexOf( item ) );
		}
		EquippedItems[type] = Items.IndexOf( item );

		UpdateSlot( type );
	}

	public void UnEquip( string type )
	{
		EquippedItems.Remove( type );

		ClearSlot( type );
	}

	public Dictionary<string, int> GetEquippedItems()
	{
		return EquippedItems;
	}

	private void UpdateSlot( string type )
	{
		// Shouldn't be necessary but its a jam teehee
		ClearSlot( type );

		var trans = GetSlot( type );
		GameObject item = Instantiate( Items[EquippedItems[type]].Prefab, trans );
		//item.transform.localPosition = Vector3.zero;
		//item.transform.localEulerAngles = Vector3.zero;
	}

	private void ClearSlot( string type )
	{
		var trans = GetSlot( type );
		foreach ( Transform child in trans )
		{
			Destroy( child.gameObject );
		}
	}

	private Transform GetSlot( string type )
	{
		return GameObject.Find( "ATTACH_" + type.ToString().ToUpper() ).transform;
	}
	#endregion

	#region Grounded
	public bool IsGrounded()
	{
		if ( controller.isGrounded )
		{
			return true;
		}
		else
		{
			// Raycast down from player feet, if hit something close then is grounded
			int layerMask = 1 << LayerMask.NameToLayer( "Default" );
			//Debug.DrawLine( transform.position, transform.position - Vector3.up * 0.5f, Color.red, 1 );
			if ( Physics.Raycast( transform.position, -Vector3.up, 0.5f, layerMask ) )
			{
				return true;
			}
		}
		return false;
	}
	#endregion
}
