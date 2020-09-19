using FullSerializer;
using Unity.Mathematics;
using UnityEngine;


public class BlokkyEditor : MB_Singleton< BlokkyEditor >
{
	public static float		GridScale;
	public static float		UiScale;
	public static Painting	Painting		= new Painting();


#pragma warning disable 0649

	[SerializeField] RectTransform	_spaceForGrid;
	[SerializeField] RectTransform	_dragStartArea;

#pragma warning restore 0649


	void Start()
	{
		Canvas.ForceUpdateCanvases();

		UiScale			= Grid.CalcScale( new Vector2Int( 6, 6 ), _dragStartArea );
	}


	public void Init( Vector2Int gridSize )
	{
		ShapeSelectors.Instance.Init();

		Painting.gridSize		= new int2( gridSize.x, gridSize.y );
		GridScale				= Grid.CalcScale( gridSize, _spaceForGrid );

		// Create Grid
		Factory.Instance.CreateBlock(
			_spaceForGrid.position,
			gridSize,
			Flags.IsGrid
		);
	}


	public void OnSave()
	{
		fsSerializer serializer		= new fsSerializer();

		serializer.TrySerialize( typeof( Painting ), Painting, out fsData data ).AssertSuccessWithoutWarnings();

		string json					= fsJsonPrinter.CompressedJson( data );

		Debug.Log( json );
	}
}

