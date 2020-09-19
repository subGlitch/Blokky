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


	public void CreateBlock( Vector2 position, Vector2Int size, Flags flags = Flags.None )
	{
		CreateBlockParent( position, size, flags, out Entity parent, out RenderMesh renderMesh );

		CreateBlockChildren( parent, size, renderMesh );
	}


	void CreateBlockParent(
			Vector2				position,
			Vector2Int			size,
			Flags				flags,
			out Entity			block,
			out RenderMesh		renderMesh
		)
	{
		float scale							= flags.Has( Flags.IsGrid ) ? BlokkyEditor.GridScale : BlokkyEditor.UiScale;
		bool isDraggable					= flags.Has( Flags.IsDraggable );
		bool isGrid							= flags.Has( Flags.IsGrid );
		int layer							= isDraggable ? 1 : 0;
		float3 position3D					= (Vector3)position + Vector3.back * layer;

		// Create RenderMesh for legos
		Material material					= new Material( legoMaterial ) { color = isDraggable ? Color.gray : Color.white };
		renderMesh							= new RenderMesh { material = material, mesh = legoMesh };

		// Create entity
        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		block								= entityManager.CreateEntity();
#if UNITY_EDITOR
		entityManager.SetName( block, "Block" );
#endif

		// Add components
		entityManager.AddComponent< LocalToWorld >( block );
		entityManager.AddComponentData( block, new Translation{ Value = position3D } );
		entityManager.AddComponentData( block, new Scale { Value = scale } );
		entityManager.AddComponentData( block, new BlockSize( new int2( size.x, size.y ) ) );
		entityManager.AddSharedComponentData( block, renderMesh );

		// Add Flag components
		if (isDraggable)	entityManager.AddComponent< IsDraggable >( block );
		if (isGrid)			entityManager.AddComponent< IsGrid >( block );
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
			entityManager.AddComponentData( lego, new LegoLocalPosition( gridPos ) );
			entityManager.SetComponentData( lego, new Translation { Value = localPos } );

			entityManager.SetSharedComponentData( lego, renderMesh );
		});

		legos.Dispose();
	}


	/// <summary> Helper method to iterate NativeArray representation of 2D array </summary>
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

