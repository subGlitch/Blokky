using Unity.Entities;


public struct GridPosition : IComponentData
{
	public int x;
	public int y;


	public GridPosition( int x, int y )
	{
		this.x		= x;
		this.y		= y;
	}
}

