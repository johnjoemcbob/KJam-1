using UnityEngine;
using System.Collections;

// From: https://stackoverflow.com/questions/41923939/orbiting-target-with-camera-and-changing-height
public class CameraControls : MonoBehaviour
{
	#region Inspector
	// Height and distance are now set in the scene (by positioning the dolly and changing that camera z value).
	[Header( "Variables" )]
	public Vector2 Distance = new Vector2( 0.5f, 5 );
	public float BackcastOffset = 1;
	public float ColliderSphereRadius = 1;
	public float OffsetBackFromHit = 0.2f;
	public float LerpSpeed = 20;
	#endregion

	#region Variables
	private float horizontal;
	private float vertical;
	#endregion

	void LateUpdate()
	{
		// Hold right click to show cursor and stop turning camera
		if ( UI.Instance.ShouldShowCursor() )//|| Input.GetMouseButton( 1 ) )
		{
			Cursor.lockState = CursorLockMode.None;

			// UI Lobby camera
			if ( !Player.Instance.Controllable )
			{
				var rect = GameObject.Find( "Blank (Spacer)" ).GetComponent<RectTransform>();
				float dir = 0;
				float mult = 2;
				if ( rect.position.x < Screen.width / 5 * 2 )
				{
					dir = 1;
				}
				else if ( rect.position.x > Screen.width / 5 * 3 )
				{
					dir = -1;
				}
				Vector3 target = -Vector3.forward * Distance.y + Vector3.right * dir * mult;
				transform.localPosition = Vector3.Lerp( transform.localPosition, target, Time.deltaTime * LerpSpeed );
			}

			return;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		// Update horizontal/ vertical angles
		horizontal += Input.GetAxis( "Mouse X" ) * Options.MouseCameraSensitivity;
		vertical -= Input.GetAxis( "Mouse Y" ) * Options.MouseCameraSensitivity;
		vertical = Mathf.Clamp( vertical, -30, 79 );

		// Update the rotation
		Transform boom = transform.parent;
		//boom.parent.localRotation = Quaternion.Lerp( boom.parent.localRotation, Quaternion.Euler( 0, horizontal, 0 ), Time.deltaTime * LerpSpeed );
		boom.localRotation = Quaternion.Lerp( boom.localRotation, Quaternion.Euler( vertical, horizontal, 0 ), Time.deltaTime * LerpSpeed );

		// Raycast from the boom outwards to desired distance to get proper position
		RaycastHit hit;
		LayerMask layer = 1 << LayerMask.GetMask( "Player" );
		//Debug.DrawLine( boom.position + transform.forward * BackcastOffset, boom.position - transform.forward * Distance.y, Color.red, 0.2f );
		if ( Physics.SphereCast( boom.position + transform.forward * BackcastOffset, ColliderSphereRadius, -transform.forward, out hit, Distance.y + BackcastOffset, layer ) )
		{
			transform.localPosition = Vector3.Lerp( transform.localPosition, -Vector3.forward * Mathf.Max( Distance.x, hit.distance - OffsetBackFromHit ), Time.deltaTime * LerpSpeed );
		}
		else
		{
			transform.localPosition = Vector3.Lerp( transform.localPosition, -Vector3.forward * Distance.y, Time.deltaTime * LerpSpeed );
		}

		//transform.LookAt( boom );
	}
}