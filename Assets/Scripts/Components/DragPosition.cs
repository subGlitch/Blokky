﻿using Unity.Entities;
using Unity.Mathematics;


public struct DragPosition : IComponentData
{
	public float2 Value;


	public DragPosition( float2 value )		=> Value		= value;
}

