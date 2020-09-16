﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class Spawner : MonoBehaviour
{
	void Start()
	{
		Vector2Int gridSize		= new Vector2Int( 17, 17 );


		Vector2 gridSize_w		= CalcGridWorldSize( gridSize );
		Vector2 gridCenter_w	= Vector2.zero;
		Vector2 gridMin_w		= gridCenter_w - gridSize_w / 2;

		float blockSize			= gridSize_w.x / gridSize.x;
		float blockScale		= blockSize;

		int count							= gridSize.x * gridSize.y;
        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
		var nativeArray						= entityManager.Instantiate( PrefabEntities.prefab_Block, count, Allocator.Temp );

		for (int y = 0; y < gridSize.y; y ++)
		for (int x = 0; x < gridSize.x; x ++)
		{
			Entity entity		= nativeArray[ y * gridSize.x + x ];
			float3 posMin		= new float3(
												gridMin_w.x + x * blockSize,
												gridMin_w.y + y * blockSize,
												0
			);
			float3 posCenter	= posMin + blockSize / 2;

			entityManager.SetComponentData( entity, new Translation { Value = posCenter } );
			entityManager.AddComponentData( entity, new NonUniformScale { Value = blockScale } );
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

