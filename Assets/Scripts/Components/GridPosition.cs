using Unity.Entities;
using Unity.Mathematics;


public struct GridPosition : IComponentData
{
	public int2		position;


	public GridPosition( int2 position )		=> this.position		= position;
}

