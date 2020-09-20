using System.Linq;
using FullSerializer;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class BlokkyEditor : MB_Singleton< BlokkyEditor >
{
	public static float		GridScale;
	public static float		UiScale;
	public static Painting	Painting		= new Painting();


#pragma warning disable 0649

	[SerializeField] string			_saveFolder					= "D:/";
	[SerializeField] RectTransform	_spaceForGrid;
	[SerializeField] RectTransform	_dragStartArea;
	[SerializeField] Button			_defaultShape;

#pragma warning restore 0649


	void Start()
	{
		Canvas.ForceUpdateCanvases();

		UiScale			= Grid.CalcScale( new Vector2Int( 6, 6 ), _dragStartArea );
	}


	public void Init( Vector2Int gridSize )
	{
		// Set gridSize
		Painting.gridSize		= new int2( gridSize.x, gridSize.y );
		GridScale				= Grid.CalcScale( gridSize, _spaceForGrid );

		// Create Grid
		Factory.Instance.CreateBlock(
			_spaceForGrid.position,
			gridSize,
			Flags.IsGrid
		);

		// Init shape selectors
		ShapeSelectors.Instance.Init();
		_defaultShape.onClick.Invoke();
		_defaultShape.Select();
	}


	// (!) Experimental feature (!)
	public void OnPicker()
	=>
		World.DefaultGameObjectInjectionWorld.GetOrCreateSystem< SnapToGridSystem >().Enabled	^= true;


	// (!) Experimental feature (!)
	public void OnRotate()
	{
		var ecb				= World.DefaultGameObjectInjectionWorld.GetOrCreateSystem< EndSimulationEntityCommandBufferSystem >().CreateCommandBuffer();
		EntityManager entityManager				= World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityQuery gridsQuery					= entityManager.CreateEntityQuery( typeof(IsGrid ) );
		using (NativeArray< Entity > grids		= gridsQuery.ToEntityArray( Allocator.TempJob ))
			foreach (Entity grid in grids)
		{
			bool isDraggable					= entityManager.HasComponent< IsDraggable >( grid );

			if (isDraggable)
				ecb.RemoveComponent		< IsDraggable >( grid );
			else
				ecb.AddComponent		< IsDraggable >( grid );
		}
	}


	public void OnSave()
	{
		// According to the specification: "... набросанный (не пустой) уровень должен сохраняться в файл (json) ..."
		if (!Painting.blocks.Any())
			return;

		fsSerializer serializer		= new fsSerializer();
		serializer.TrySerialize( typeof( Painting ), Painting, out fsData data ).AssertSuccessWithoutWarnings();
		string json					= fsJsonPrinter.CompressedJson( data );

		Save( json );

		Debug.Log( json );
	}


	void Save( string json )
	{
		string folder			=
									Application.isMobilePlatform ?
									Application.persistentDataPath :
									_saveFolder
		;

		bool success			= false;
		int suffix				= 0;
		while (!success && suffix < 1000)
		{
			string fileName		= $"Painting_{suffix}.json";
			string path			= System.IO.Path.Combine( _saveFolder, fileName );
			success				= !System.IO.File.Exists( path );

			if (success)
				SavingSystem.SaveText( path, json );

			suffix ++;
		}
	}
}

