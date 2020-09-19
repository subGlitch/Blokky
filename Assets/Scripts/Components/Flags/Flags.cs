using System;


[Flags]
public enum Flags
{
	None			= 0,

	IsGrid			= 1,
	IsDraggable		= 2,
}


static class FlagExtMethods
{
	public static bool Has( this Flags flags, Flags flag )		=> (flags & flag) > 0;
}

