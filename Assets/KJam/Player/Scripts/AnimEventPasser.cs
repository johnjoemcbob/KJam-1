using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simply catches any events passed by the animator and sends them on to the actual Player class, higher up in the heirarchy
public class AnimEventPasser : MonoBehaviour
{
	public void SpawnHitbox()
	{
		Player.Instance.SpawnHitbox();
	}
}
