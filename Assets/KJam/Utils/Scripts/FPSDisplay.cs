using UnityEngine;
using UnityEngine.UI;

// From: https://forum.unity.com/threads/fps-counter.505495/
public class FPSDisplay : MonoBehaviour
{
	public int avgFrameRate;
	public Text display_Text;

	private float NextUpdate = 0;

	public void Update()
	{
		if ( Options.ShowFPS )
		{
			if ( NextUpdate <= Time.time )
			{
				float current = 0;
				current = (int) ( 1f / Time.unscaledDeltaTime );
				avgFrameRate = (int) current;
				display_Text.text = avgFrameRate.ToString() + " FPS";

				NextUpdate = Time.time + 0.5f;
			}
		}
		else
		{
			display_Text.text = "";
		}
	}
}