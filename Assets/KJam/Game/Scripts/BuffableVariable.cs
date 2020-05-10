using System.Collections.Generic;

public struct BuffableVariable
{
	public BuffableVariable( float val )
	{
		Base = val;
		Current = val;
	}

	public float Base;
	public float Current;
	//public Dictionary<string, VariableEffect> Effectors;
}
