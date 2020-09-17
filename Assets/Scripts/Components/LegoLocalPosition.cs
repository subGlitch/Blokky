using Unity.Entities;
using Unity.Mathematics;


public struct LegoLocalPosition : IComponentData
{
	public int2		position;


	public LegoLocalPosition( int2 position )		=> this.position		= position;
}

