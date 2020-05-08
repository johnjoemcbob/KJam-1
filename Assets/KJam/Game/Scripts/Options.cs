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

	#region Options - Initialize UI
	public void OnEnable()
	{
		// TODO Load here


		// MouseCameraSensitivity
		transform.GetChild( 0 ).GetComponentInChildren<Slider>().value = MouseCameraSensitivity;
	}
	#endregion

	// TODO save and load also
}
