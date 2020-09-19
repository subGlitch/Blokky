using UnityEngine;


public class BlokkyEditor : MB_Singleton< BlokkyEditor >
{
	public void Init( Vector2Int gridSize )
	{
		Grid.SetGridSize( gridSize );

		Factory.Instance.CreateBlock( Vector2.zero, gridSize, false );
		Factory.Instance.CreateBlock( Vector2.one, new Vector2Int( 3, 2 ), true );
	}
}

