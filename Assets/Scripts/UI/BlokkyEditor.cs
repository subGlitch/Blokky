using UnityEngine;


public class BlokkyEditor : MB_Singleton< BlokkyEditor >
{
	[SerializeField] RectTransform	_spaceForGrid;


	public void Init( Vector2Int gridSize )
	{
		Rect space		= _spaceForGrid.GetWorldRect();

		Grid.SetGridSize( gridSize );

		Factory.Instance.CreateBlock( Vector2.zero, gridSize, false );
		Factory.Instance.CreateBlock( Vector2.one, new Vector2Int( 3, 2 ), true );
	}
}

