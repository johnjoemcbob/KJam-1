using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
	#region Options - Variables
	public static float MouseCameraSensitivity = 5;
	#endregion

	#region Options - Value Changers
	public void ValueChange_MouseCameraSensitivity( float sens )
	{
		MouseCameraSensitivity = sens;
	}
	#endregion

	#region MonoBehaviour
	public void OnEnable()
	{
		Load();

		// MouseCameraSensitivity
		transform.GetChild( 0 ).GetComponentInChildren<Slider>().value = MouseCameraSensitivity;
	}

	public void OnDisable()
	{
		Save();
	}
	#endregion

	#region Save/Load
	public static void Save()
	{
		PlayerPrefs.SetFloat( "MouseCameraSensitivity", MouseCameraSensitivity );

		// Save last
		PlayerPrefs.Save();
	}

	public static void Load()
	{
		MouseCameraSensitivity = PlayerPrefs.GetFloat( "MouseCameraSensitivity", MouseCameraSensitivity );
	}
	#endregion
}
