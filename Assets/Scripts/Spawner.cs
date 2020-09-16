using Unity.Entities;
using UnityEngine;


public class Spawner : MonoBehaviour
{
	void Start()
	{
		/*
        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityArchetype entityArchetype		= entityManager.CreateArchetype(
												typeof( GridPositionComponent ),
												typeof( IsTakenComponent )
		);
		var nativeArray						= entityManager.CreateEntity( entityArchetype, 3, Allocator.Temp );
	
		nativeArray.Dispose();		// Not necessary...

		// entityManager.SetComponentData( entity, new GridPositionComponent{ position = new int2( 3, 5 ) } );
		*/


        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		Entity entity						= entityManager.Instantiate( PrefabEntities.prefab_Block );
	}
}

