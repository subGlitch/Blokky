using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


public class Factory : MB_Singleton< Factory >
{
#pragma warning disable 0649

	[SerializeField] Material	legoMaterial;
	[SerializeField] Mesh		legoMesh;

#pragma warning restore 0649


	public void CreateBlock( Vector2 position, Vector2Int size, bool isDraggable )
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
		Material material					= new Material( legoMaterial ) { color = isDraggable ? Color.gray : Color.white };
		renderMesh							= new RenderMesh { material = material, mesh = legoMesh };

		// Create entity
        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		block								= entityManager.CreateEntity();
		entityManager.SetName( block, "Block" );

		// Add components
		entityManager.AddComponent< LocalToWorld >( block );
		entityManager.AddComponentData( block, new Translation{ Value = position3D } );
		entityManager.AddComponentData( block, new Scale { Value = Grid.LegoScale } );
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

			int2 gridPos					= new int2( x, y );
			float3 localBlockMin			= (float3)(Vector3)(Vector2)blockSize / -2;
			float3 localPos					= localBlockMin + new float3( x, y, 0 ) + (float3).5f;
			entityManager.AddComponentData( lego, new GridPosition( gridPos ) );
			entityManager.SetComponentData( lego, new Translation { Value = localPos } );

			entityManager.SetSharedComponentData( lego, renderMesh );
		});

		legos.Dispose();
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

