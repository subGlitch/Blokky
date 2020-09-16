using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class Spawner : MonoBehaviour
{
	void Start()
	{
		Vector2Int gridSize			= new Vector2Int( 17, 17 );


		Vector2 gridWorldSize		= CalcGridWorldSize( gridSize );
		float blockWidth			= gridWorldSize.x / gridSize.x;
		float blockScale			= blockWidth;

		int count							= gridSize.x * gridSize.y;
        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		var nativeArray						= entityManager.Instantiate( PrefabEntities.prefab_Block, count, Allocator.Temp );

		for (int y = 0; y < gridSize.y; y ++)
		for (int x = 0; x < gridSize.x; x ++)
		{
			Entity entity		= nativeArray[ y * gridSize.x + x ];

			entityManager.SetComponentData( entity, new Translation { Value = new float3( x, y, 0 ) } );
			entityManager.AddComponentData( entity, new NonUniformScale { Value = new float3( blockScale, blockScale, blockScale ) } );
			entityManager.AddComponentData( entity, new GridPositionComponent( x, y ) );
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

