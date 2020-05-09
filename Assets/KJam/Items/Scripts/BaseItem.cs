using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum ItemType
{
	Weapon,
	Torso,
	Legs,
	Amulet,
	Ring,
	Belt
}

[Serializable]
public class BaseItem : ScriptableObject
{
	public string Name;
	//public string Description;
	public int Cost;
	public ItemType Type;
	public bool Buyable;
	public float StoreAppearChance; // Percentage

	public GameObject Prefab;
	public Sprite Sprite;

	public Dictionary<string, float> Stats;
}
