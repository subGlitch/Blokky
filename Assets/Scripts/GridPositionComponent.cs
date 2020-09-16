using Unity.Entities;


public struct GridPositionComponent : IComponentData
{
	public int x;
	public int y;


	public GridPositionComponent( int x, int y )
	{
		this.x		= x;
		this.y		= y;
	}
}

