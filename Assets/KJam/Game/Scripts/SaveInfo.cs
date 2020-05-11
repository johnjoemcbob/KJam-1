using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public struct SaveInfo
{
	public string BuildVersion; // For jam updates
	public int LevelsPlayed; // For level progression stuff.. just jam things
	public float Gold;
	public string[] Items;
	public string[] EquippedItemsKey;
	public int[] EquippedItemsValue;
}
