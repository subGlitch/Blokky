using Unity.Entities;
using Unity.Mathematics;


public struct BlockSize : IComponentData
{
	public int2 Value;


	public BlockSize( int2 value )		=> Value		= value;
}

