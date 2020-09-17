using System;
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


	void Start()
	{
		Vector2Int gridSize		= new Vector2Int( 2, 2 );


		Vector2 gridSize_w		= CalcGridWorldSize( gridSize );
		Vector2 gridCenter_w	= Vector2.zero;
		Vector2 gridMin_w		= gridCenter_w - gridSize_w / 2;

		float blockSize			= gridSize_w.x / gridSize.x;
		float blockScale		= blockSize;

		int count							= gridSize.x * gridSize.y;
        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		NativeArray< Entity > nativeArray	= entityManager.Instantiate( PrefabEntities.entityPrefab_Lego, count, Allocator.Temp );

		RenderMesh renderMesh				= new RenderMesh();
		material							= new Material( refMaterial );
		renderMesh.material					= material;
		renderMesh.mesh						= refMesh;


		// Create cells
		ForEach( nativeArray, gridSize, (entity, x, y) =>
		{
			Vector2 posMin			= gridMin_w + new Vector2( x, y ) * blockSize;
			Vector2 posCenter		= posMin + Vector2.one * blockSize / 2;

			entityManager.SetComponentData( entity, new Translation { Value = (Vector3)posCenter } );
			entityManager.AddComponentData( entity, new Scale { Value = blockScale } );
			entityManager.AddComponentData( entity, new GridPosition( x, y ) );
			entityManager.SetSharedComponentData( entity, renderMesh );
		});


		// Create grid
		Entity grid							= entityManager.CreateEntity();
		entityManager.AddComponent< Draggable >( grid );
		DynamicBuffer< Cell > cells			= entityManager.AddBuffer< Cell >( grid );
		ForEach( nativeArray, gridSize, (entity, x, y) =>
			cells.Add( new Cell( entity ) )
		);


		nativeArray.Dispose();
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
	void ForEach( NativeArray< Entity > nativeArray, Vector2Int gridSize, Action< Entity, int, int > action )
	{
		for (int y = 0; y < gridSize.y; y ++)
		for (int x = 0; x < gridSize.x; x ++)
		{
			Entity entity		= nativeArray[ y * gridSize.x + x ];
			
			action( entity, x, y );
		}
	}
}

