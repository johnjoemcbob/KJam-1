using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
		if ( Camera.main != null )
		{
			transform.LookAt( Camera.main.transform );
			transform.localEulerAngles = new Vector3( 0, transform.localEulerAngles.y, 0 );
		}
    }
}
