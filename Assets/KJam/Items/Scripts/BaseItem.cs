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
public struct VariableEffect
{
	public string Variable;
	public bool Multiply;
	public float Modifier;
}

[Serializable]
public class BaseItem : ScriptableObject
{
	public string Name;
	//public string Description;
	public int Cost;
	public ItemType Type;
	public bool Default;
	public bool Buyable;
	public float StoreAppearChance; // TODO Percentage

	public bool Armour;
	public GameObject Prefab;
	public Sprite Sprite;

	[SerializeField]
	public VariableEffect[] Stats;
}
