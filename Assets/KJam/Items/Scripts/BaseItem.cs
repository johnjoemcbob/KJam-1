using System;
using System.Collections.Generic;

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
public struct BaseItem
{
	public string Name;
	//public string Description;
	public int Cost;
	public ItemType Type;

	public Dictionary<string, float> Stats;
}
