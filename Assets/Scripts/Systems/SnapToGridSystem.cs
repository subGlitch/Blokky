﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


[UpdateAfter( typeof(DragSystem) )]
public class SnapToGridSystem : DragSystemBase
{
	protected override void OnUpdate()
	{
		/*
			Entity grid				= GetSingletonEntity< IsGrid >();
			Entity draggedBlock		= GetSingletonEntity< DragPosition >();

			We can go this (simple) way (using GetSingletonEntity()), but that's boring :-p
			Let's go more generalized approach, instead!
			So we can drag multiple blocks simultaneously and have multiple grids =)))
		*/

		bool isDragFinish						= Input.GetMouseButtonUp( 0 );

		EntityQuery draggedQuery				= GetEntityQuery( typeof(DragPosition) );
		if (draggedQuery.CalculateEntityCount() == 0)
			return;

		EntityQuery gridsQuery					= GetEntityQuery( typeof(IsGrid) );
		if (gridsQuery.CalculateEntityCount() == 0)
			return;

		using (NativeArray< Entity > blocks		= draggedQuery	.ToEntityArray( Allocator.TempJob ))
		using (NativeArray< Entity > grids		= gridsQuery	.ToEntityArray( Allocator.TempJob ))
			foreach (Entity block in blocks)
			{
				if (!FindGridOverlappedBy( block, grids, out Entity grid, out float gridScale ))
				{
					// No overlapping with grid
					if (isDragFinish)
						EntityManager.DestroyEntity( block );
					continue;
				}

				float2 snappedPosition			= CalcSnappedPosition( block, grid, gridScale );

				if (IsOutOfGrid( block, grid, snappedPosition, gridScale ))
				{
					// Overlapping with grid, but snapped position is not completely inside the grid
					if (isDragFinish)
						EntityManager.DestroyEntity( block );
					continue;
				}

				Snap( block, snappedPosition, gridScale );

				// Set Color
				RenderMesh renderMesh			= EntityManager.GetSharedComponentData< RenderMesh >( block );
				// renderMesh.material.color		= outOfGrid ? Color.red : Color.gray;

				if (isDragFinish)
					PlaceOnGrid( block, grid );
			}
	}
	
	void Snap( Entity block, float2 snappedPosition, float gridScale )
	{
		Translation translation			= EntityManager.GetComponentData< Translation >( block );
		translation						= new Translation{ Value = new float3( snappedPosition, translation.Value.z ) };
		EntityManager.SetComponentData( block, translation );
		EntityManager.SetComponentData( block, new Scale{ Value = gridScale } );
	}


	void PlaceOnGrid( Entity block, Entity grid )
	{
		DynamicBuffer< Cell > children		= EntityManager.GetBuffer< Cell >( grid );
		// EntityManager.AddComponent< IsTaken >( children[ 0 ].Value );

		PostUpdateCommands.AddComponent( children[ 0 ].Value, new Scale{ Value = .7f } );

		EntityManager.RemoveComponent< DragPosition >( block );
		EntityManager.RemoveComponent< IsDraggable >( block );

		PostUpdateCommands.DestroyEntity( block );
	}


	bool IsOutOfGrid( Entity block, Entity grid, float2 snappedPosition, float gridScale )
	{
		int2 blockSize		= EntityManager.GetComponentData< BlockSize >( block ).Value;
		int2 gridSize		= EntityManager.GetComponentData< BlockSize >( grid ).Value;

		int2 blockMin		= int2.zero;
		int2 blockMax		= blockSize - 1;

		int2 min_g			= ProjectOnGrid( block, snappedPosition, grid, gridScale, blockMin );
		int2 max_g			= ProjectOnGrid( block, snappedPosition, grid, gridScale, blockMax );

		return
			min_g.x < 0				||
			min_g.y < 0				||
			max_g.x >= gridSize.x	||
			max_g.y >= gridSize.y
		;
	}


	float2 CalcSnappedPosition( Entity block, Entity grid, float gridScale )
	{
		float2 gridPos					= EntityManager.GetComponentData< Translation >( grid ).Value.xy;
		float2 dragPos					= EntityManager.GetComponentData< DragPosition >( block ).Value;

		float2 otnc_Grid				= OffsetToNearestCell( grid, gridScale );
		float2 otnc_Block				= OffsetToNearestCell( block, gridScale );

		// For snapping it doesn't matter which cell to take (here we taking nearest from block's center), so any cell will do
		float2 anyCellOnGrid			= gridPos + otnc_Grid;
		float2 anyCellOnBlock			= dragPos + otnc_Block;

		float2 snapPositive				= (anyCellOnGrid - anyCellOnBlock).NegativeSafeMod( gridScale );
		float2 snapNegative				= snapPositive - gridScale;
		float2 snapNearest				= new float2(
														snapPositive.x < snapNegative.x * (-1) ? snapPositive.x : snapNegative.x,
														snapPositive.y < snapNegative.y * (-1) ? snapPositive.y : snapNegative.y
		);

		float2 snappedPosition			= dragPos + snapNearest;

		return snappedPosition;
	}


	// Offset from center of the block to nearest cell center
	int2 ProjectOnGrid( Entity block, float2 blockSnappedPos, Entity grid, float gridScale, int2 local )
	{
		float2 gridMin					= GetRectOnGrid( grid, grid ).min;

		float2 blockSize				= WorldSizeOnGrid( block, grid );
		float2 blockSnappedMin			= blockSnappedPos - blockSize / 2;

		int2 blockGridMinDelta			= (int2)math.round( (blockSnappedMin - gridMin) / gridScale );

		return local + blockGridMinDelta;
	}


	float2 OffsetToNearestCell( Entity block, float gridScale )
	{
		float2 offsetToMinCell			= OffsetToMinCell( block, gridScale );
		float2 offsetToNearestCell		= offsetToMinCell.NegativeSafeMod( gridScale );

		return offsetToNearestCell;
	}


	float2 OffsetToMinCell( Entity block, float gridScale )
	{
		float2 blockSize_w				= WorldSizeOnGrid( block, gridScale );

		// _lw suffix - local (relative to block's center), but scaled (so in world coordinates) >>>

		float2 blockMin_lw				= blockSize_w / (-2);
		float2 localMinCell_lw			= blockMin_lw + gridScale / 2;

		float2 blockCenter_lw			= float2.zero;
		float2 minCellCenter_lw			= localMinCell_lw;
		float2 offset					= blockCenter_lw - minCellCenter_lw;

		return offset;
	}


	bool FindGridOverlappedBy( Entity block, NativeArray< Entity > grids, out Entity grid, out float gridScale )
	{
		foreach (Entity entity in grids)
		{
			grid			= entity;
			gridScale		= EntityManager.GetComponentData< Scale >( grid ).Value;
			Rect rect		= GetRectOnGrid( block, grid );

			if (rect.Overlaps( GetRectOnGrid( grid, grid ) ))
				return true;
		}
		
		grid			= Entity.Null;
		gridScale		= 0;
		return false;
	}


	Rect GetRectOnGrid( Entity block, Entity grid )
	{
		Translation translation			= EntityManager.GetComponentData< Translation >( block );
		Vector2 size_w					= WorldSizeOnGrid( block, grid );
		Vector2 rectPos_w				= (Vector2)translation.Value.xy - size_w / 2;
		Rect rect_w						= new Rect( rectPos_w, size_w );

		return rect_w;
	}


	float2 WorldSizeOnGrid( Entity grid )		=> WorldSizeOnGrid( grid, grid );


	float2 WorldSizeOnGrid( Entity block, Entity grid )
	{
		float gridScale			= EntityManager.GetComponentData< Scale >( grid ).Value;

		return WorldSizeOnGrid( block, gridScale );
	}


	float2 WorldSizeOnGrid( Entity block, float gridScale )
	{
		int2 blockSize			= EntityManager.GetComponentData< BlockSize >( block ).Value;

		return (float2)blockSize * gridScale;
	}
}

