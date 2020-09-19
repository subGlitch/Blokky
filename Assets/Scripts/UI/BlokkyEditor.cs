using UnityEngine;


public class BlokkyEditor : MB_Singleton< BlokkyEditor >
{
#pragma warning disable 0649

	[SerializeField] RectTransform	_spaceForGrid;

#pragma warning restore 0649


	public void Init( Vector2Int gridSize )
	{
		Rect availableSpace		= _spaceForGrid.GetWorldRect();
		Vector2 center			= Grid.SetGridSize( gridSize, availableSpace );

		Factory.Instance.CreateBlock( center, gridSize, false );
		Factory.Instance.CreateBlock( Vector2.one, new Vector2Int( 3, 2 ), true );
	}
}

