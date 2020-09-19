using Unity.Entities;
using Unity.Mathematics;


public struct LegoLocalPosition : IComponentData
{
	public int2		Value;


	public LegoLocalPosition( int2 value )		=> Value		= value;
}

