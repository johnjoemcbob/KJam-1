using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public struct SaveInfo
{
	public float Gold;
	public string[] Items;
	public string[] EquippedItemsKey;
	public int[] EquippedItemsValue;
}
