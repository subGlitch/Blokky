using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class Spawner : MonoBehaviour
{
	void Start()
	{
		Vector2Int size			= new Vector2Int( 17, 17 );
		int count				= size.x * size.y;

        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		var nativeArray						= entityManager.Instantiate( PrefabEntities.prefab_Block, count, Allocator.Temp );

		for (int y = 0; y < size.y; y ++)
		for (int x = 0; x < size.x; x ++)
		{
			Entity entity		= nativeArray[ y * size.x + x ];

			entityManager.SetComponentData( entity, new Translation { Value = new float3( x, y, 0 ) } );
		}

		nativeArray.Dispose();
	}
}

