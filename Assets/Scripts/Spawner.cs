using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


public struct Cell : IBufferElementData
{
	public Entity cell;

	public Cell( Entity cell )		=> this.cell		= cell;
}


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
		var nativeArray						= entityManager.Instantiate( PrefabEntities.prefab_Block, count, Allocator.Temp );

		RenderMesh renderMesh				= new RenderMesh();
		material							= new Material( refMaterial );
		renderMesh.material					= material;
		renderMesh.mesh						= refMesh;


		// Create cells
		for (int y = 0; y < gridSize.y; y ++)
		for (int x = 0; x < gridSize.x; x ++)
		{
			Entity cell				= nativeArray[ y * gridSize.x + x ];
			Vector2 posMin			= gridMin_w + new Vector2( x, y ) * blockSize;
			Vector2 posCenter		= posMin + Vector2.one * blockSize / 2;

			entityManager.SetComponentData( cell, new Translation { Value = (Vector3)posCenter } );
			entityManager.AddComponentData( cell, new Scale { Value = blockScale } );
			entityManager.AddComponentData( cell, new GridPositionComponent( x, y ) );
			entityManager.SetSharedComponentData( cell, renderMesh );
		}


		// Create grid
		Entity grid							= entityManager.CreateEntity();
		DynamicBuffer< Cell > cells			= entityManager.AddBuffer< Cell >( grid );
		for (int y = 0; y < gridSize.y; y ++)
		for (int x = 0; x < gridSize.x; x ++)
		{
			Entity cell			= nativeArray[ y * gridSize.x + x ];
			cells.Add( new Cell( cell ) );
		}


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
}

