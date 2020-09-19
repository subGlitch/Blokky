using UnityEngine;


public class BlokkyEditor : MB_Singleton< BlokkyEditor >
{
	public static float		GridScale;
	public static float		UiScale;


#pragma warning disable 0649

	[SerializeField] RectTransform	_spaceForGrid;
	[SerializeField] RectTransform	_dragStartArea;

#pragma warning restore 0649


	void Start()
	{
		Canvas.ForceUpdateCanvases();

		UiScale				= Grid.CalcScale( new Vector2Int( 6, 6 ), _dragStartArea );
	}


	public void Init( Vector2Int gridSize )
	{
		ShapeSelectors.Instance.Init();

		GridScale			= Grid.CalcScale( gridSize, _spaceForGrid );

		// Create Grid
		Factory.Instance.CreateBlock(
			_spaceForGrid.position,
			gridSize,
			Flags.IsGrid
		);
	}
}

