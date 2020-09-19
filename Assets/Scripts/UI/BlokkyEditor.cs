using UnityEngine;


public class BlokkyEditor : MB_Singleton< BlokkyEditor >
{
	public static float		GridScale;
	public static float		UiScale;


#pragma warning disable 0649

	[SerializeField] RectTransform	_spaceForGrid;
	[SerializeField] RectTransform	_dragStartArea;

#pragma warning restore 0649


	public void Init( Vector2Int gridSize )
	{
		// Calc scales
		GridScale			= Grid.CalcScale( gridSize, _spaceForGrid );
		UiScale				= Grid.CalcScale( new Vector2Int( 5, 5 ), _dragStartArea );

		// Create Grid
		Factory.Instance.CreateBlock(
			_spaceForGrid.position,
			gridSize,
			Flags.IsGrid
		);

		// UI selected shape
		Factory.Instance.CreateBlock(
			DragStartArea.Instance.gameObject.transform.position,
			new Vector2Int( 3, 2 )
		);
	}
}

