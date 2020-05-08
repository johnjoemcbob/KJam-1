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
	public bool Grounded;
	[HideInInspector]
	public bool Controllable = true;

	private float LastGrounded = 0;
	private float currentHeadRotation = 0;
	private float yVelocity = 0;
	private Vector3 TargetVelocity = Vector3.zero;
	private Vector3 moveVelocity = Vector3.zero;

	private List<BaseItem> Inventory = new List<BaseItem>();

	private float Gold = 0;
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
	}

	void Update()
	{
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
			TargetVelocity = BoomTrans.TransformDirection( new Vector3( Input.GetAxisRaw( "Horizontal" ), 0, Input.GetAxisRaw( "Vertical" ) ) ) * Speed;
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
		}
		else
		{
			// Look at cursor
			// TODO
			currentHeadRotation = Mathf.Clamp( currentHeadRotation + Mathf.Sin( Time.time ) * RotationSpeed, minHeadRotation, maxHeadRotation );
		}

		// Update head
		foreach ( Transform child in head )
		{
			child.localRotation = Quaternion.identity;
			child.Rotate( Vector3.left, currentHeadRotation );
		}
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

	#region Inventory
	public void AddItem( BaseItem item )
	{
		Inventory.Add( item );
	}

	public List<BaseItem> GetInventory()
	{
		return Inventory;
	}
	#endregion

	#region Gold
	public float AddGold( float add )
	{
		Gold = Mathf.Max( 0, Gold + add );
		return Gold;
	}

	public bool HasGold( float gold )
	{
		return Gold >= gold;
	}

	public float GetGold()
	{
		return Gold;
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
