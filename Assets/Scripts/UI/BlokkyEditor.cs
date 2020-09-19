using UnityEngine;


public class BlokkyEditor : MB_Singleton< BlokkyEditor >
{
#pragma warning disable 0649

	[SerializeField] RectTransform	_spaceForGrid;
	[SerializeField] RectTransform	_dragStartArea;

#pragma warning restore 0649


	public void Init( Vector2Int gridSize )
	{
		// Init Grid
		Grid.SetGridSize( gridSize, _spaceForGrid );
		Factory.Instance.CreateBlock( _spaceForGrid.position, gridSize, Grid.LegoScale, false );

		// Calc UI scale
		float uiScale			= Grid.CalcScale( new Vector2Int( 5, 5 ), _dragStartArea );

		// UI selected block
		Factory.Instance.CreateBlock( DragStartArea.Instance.gameObject.transform.position, new Vector2Int( 3, 2 ), uiScale, false );

		// Test
		Factory.Instance.CreateBlock( Vector2.one, new Vector2Int( 3, 2 ), Grid.LegoScale, true );
	}
}

