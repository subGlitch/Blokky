using Unity.Entities;


public struct Cell : IBufferElementData
{
	public Entity entity;


	public Cell( Entity cell )		=> this.entity		= cell;
}

