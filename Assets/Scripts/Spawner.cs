using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


public class Spawner : MonoBehaviour
{
#pragma warning disable 0649

	[SerializeField] Material	material;

	[Header( "Refs" )]
	[SerializeField] Material	refMaterial;
	[SerializeField] Mesh		refMesh;

#pragma warning restore 0649


	float	_legoSize;
	float	_legoScale;


	void Start()
	{
		Vector2Int gridSize		= new Vector2Int( 5, 5 );

		SetGridSize( gridSize );

		CreateBlock( Vector2.zero, gridSize, false );
		CreateBlock( Vector2.one, new Vector2Int( 3, 2 ), true );
	}


	void SetGridSize( Vector2Int gridSize )
	{
		Vector2 gridSize_w		= CalcGridWorldSize( gridSize );

		_legoSize				= gridSize_w.x / gridSize.x;
		_legoScale				= _legoSize;
	}


	void CreateBlock( Vector2 position, Vector2Int size, bool isDraggable )
	{
		CreateBlockParent( position, isDraggable, out Entity parent, out RenderMesh renderMesh );

		CreateBlockChildren( parent, size, renderMesh );
	}


	void CreateBlockParent(
			Vector2				position,
			bool				isDraggable,
			out Entity			block,
			out RenderMesh		renderMesh
		)
	{
		int layer							= isDraggable ? 1 : 0;
		float3 position3D					= (Vector3)position + Vector3.back * layer;

		// Create RenderMesh
		material							= new Material( refMaterial ) { color = isDraggable ? Color.gray : Color.white };
		renderMesh							= new RenderMesh { material = material, mesh = refMesh };

		// Create entity
        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		block								= entityManager.CreateEntity();
		entityManager.SetName( block, "Block" );

		// Add components
		entityManager.AddComponent< LocalToWorld >( block );
		entityManager.AddComponentData( block, new Translation{ Value = position3D } );
		entityManager.AddComponentData( block, new Scale { Value = _legoScale } );
		if (isDraggable)
			entityManager.AddComponent< Draggable >( block );
		entityManager.AddSharedComponentData( block, renderMesh );
	}


	void CreateBlockChildren(
			Entity			parent,
			Vector2Int		blockSize,
			RenderMesh		renderMesh
		)
	{
		// Instantiate entity prefabs
		int legosCount						= blockSize.x * blockSize.y;
        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		NativeArray< Entity > legos			= entityManager.Instantiate( PrefabEntities.entityPrefab_Lego, legosCount, Allocator.Temp );

		// Set/Add components
		ForEach( legos, blockSize, (lego, x, y) =>
		{
			entityManager.AddComponentData( lego, new Parent{ Value = parent } );
			entityManager.AddComponentData( lego, new LocalToParent() );

			entityManager.SetComponentData( lego, new Translation { Value = new float3( x, y, 0 ) } );
			entityManager.AddComponentData( lego, new GridPosition( x, y ) );

			entityManager.SetSharedComponentData( lego, renderMesh );
		});

		legos.Dispose();
	}


	Vector2 CalcGridWorldSize( Vector2Int gridSize )
	{
		Camera mainCamera			= Camera.main;

		float screenWorldHeight		= mainCamera.orthographicSize * 2;
		float screenWorldWidth		= screenWorldHeight * mainCamera.aspect;

		float girdWorldWidth		= screenWorldWidth;
		float girdWorldHeight		= girdWorldWidth * gridSize.y / gridSize.x;

		return new Vector2( girdWorldWidth, girdWorldHeight );
	}


	/// <summary> Helper method to iterate NativeArray representation of 2D grid </summary>
	void ForEach( NativeArray< Entity > nativeArray, Vector2Int blockSize, Action< Entity, int, int > action )
	{
		for (int y = 0; y < blockSize.y; y ++)
		for (int x = 0; x < blockSize.x; x ++)
		{
			Entity entity		= nativeArray[ y * blockSize.x + x ];
			
			action( entity, x, y );
		}
	}
}

