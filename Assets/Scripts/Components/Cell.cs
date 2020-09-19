using Unity.Entities;


public struct Cell : IBufferElementData
{
	public Entity Value;


	public Cell( Entity entity )		=> Value	= entity;
}

