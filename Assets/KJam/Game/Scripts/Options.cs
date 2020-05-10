using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
	#region Options - Variables
	public static float MouseCameraSensitivity = 5;
	public static bool ShowFPS = false;
	#endregion

	#region Options - Value Changers
	public void ValueChange_MouseCameraSensitivity( float sens )
	{
		MouseCameraSensitivity = sens;
	}

	public void ValueChange_ShowFPS( bool value )
	{
		ShowFPS = value;
	}
	#endregion

	#region MonoBehaviour
	public void OnEnable()
	{
		Load();

		// MouseCameraSensitivity
		transform.GetChild( 0 ).GetComponentInChildren<Slider>().value = MouseCameraSensitivity;
		transform.GetChild( 1 ).GetComponentInChildren<Toggle>().isOn = ShowFPS;
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
		PlayerPrefs.SetInt( "ShowFPS", ShowFPS ? 1 : 0 );

		// Save last
		PlayerPrefs.Save();
	}

	public static void Load()
	{
		MouseCameraSensitivity = PlayerPrefs.GetFloat( "MouseCameraSensitivity", MouseCameraSensitivity );
		ShowFPS = PlayerPrefs.GetInt( "ShowFPS", ShowFPS ? 1 : 0 ) == 1;
	}
	#endregion
}
