using Unity.Entities;
using Unity.Mathematics;


public struct BlockSize : IComponentData
{
	public int2 size;


	public BlockSize( int2 size )		=> this.size		= size;
}

