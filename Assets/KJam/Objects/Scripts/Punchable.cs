using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punchable : MonoBehaviour
{
	protected Vector3 TargetScale;

	void Start()
	{
		TargetScale = transform.localScale;
	}

    void Update()
	{
		transform.localScale = Vector3.Lerp( transform.localScale, TargetScale, Time.deltaTime * 5 );
	}

	public void Punch()
	{
		float hor = 1.4f;
		float ver = 0.8f;
		transform.localScale = new Vector3( TargetScale.x * hor, TargetScale.y * ver, TargetScale.z * hor );
	}
}
