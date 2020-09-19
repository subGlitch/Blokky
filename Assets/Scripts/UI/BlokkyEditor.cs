using UnityEngine;


public class BlokkyEditor : MB_Singleton< BlokkyEditor >
{
#pragma warning disable 0649

	[SerializeField] RectTransform	_spaceForGrid;
	[SerializeField] RectTransform	_dragStartArea;

#pragma warning restore 0649


	public void Init( Vector2Int gridSize )
	{
		// Calc scales
		float gridScale			= Grid.CalcScale( gridSize, _spaceForGrid );
		float uiScale			= Grid.CalcScale( new Vector2Int( 5, 5 ), _dragStartArea );

		// Init Grid
		Factory.Instance.CreateBlock( _spaceForGrid.position, gridSize, gridScale, false );

		// UI selected block
		Factory.Instance.CreateBlock( DragStartArea.Instance.gameObject.transform.position, new Vector2Int( 3, 2 ), uiScale, false );

		// Test
		Factory.Instance.CreateBlock( new Vector2( 0, -4 ), new Vector2Int( 3, 2 ), uiScale, true );
	}
}

