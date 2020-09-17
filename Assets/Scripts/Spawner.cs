﻿using System;
using Unity.Collections;
using Unity.Entities;
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


	float	_blockSize;
	float	_blockScale;


	void Start()
	{
		Vector2Int gridSize		= new Vector2Int( 2, 2 );

		SetGridSize( gridSize );

		CreateBlock( gridSize, Vector2.zero );
	}


	void SetGridSize( Vector2Int gridSize )
	{
		Vector2 gridSize_w		= CalcGridWorldSize( gridSize );

		_blockSize				= gridSize_w.x / gridSize.x;
		_blockScale				= _blockSize;
	}


	void CreateBlock( Vector2Int blockSize, Vector2 blockCenter_w )
	{
		Vector2 blockSize_w					= (Vector2)blockSize * _blockSize;
		Vector2 blockMin_w					= blockCenter_w - blockSize_w / 2;


		// Create Legos
        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		int legosCount						= blockSize.x * blockSize.y;
		NativeArray< Entity > legos			= entityManager.Instantiate( PrefabEntities.entityPrefab_Lego, legosCount, Allocator.Temp );
		RenderMesh renderMesh				= new RenderMesh();
		material							= new Material( refMaterial );
		renderMesh.material					= material;
		renderMesh.mesh						= refMesh;
		ForEach( legos, blockSize, (entity, x, y) =>
		{
			Vector2 posMin					= blockMin_w + new Vector2( x, y ) * _blockSize;
			Vector2 posCenter				= posMin + Vector2.one * _blockSize / 2;

			entityManager.SetComponentData( entity, new Translation { Value = (Vector3)posCenter } );
			entityManager.AddComponentData( entity, new Scale { Value = _blockScale } );
			entityManager.AddComponentData( entity, new GridPosition( x, y ) );
			entityManager.SetSharedComponentData( entity, renderMesh );
		});


		// Create Block
		Entity block						= entityManager.CreateEntity();
		entityManager.AddComponent< Draggable >( block );
		DynamicBuffer< Cell > cells			= entityManager.AddBuffer< Cell >( block );
		ForEach( legos, blockSize, (entity, x, y) =>
			cells.Add( new Cell( entity ) )
		);


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

