using Unity.Collections;
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
					continue;

				SnapToGrid( block, grid, gridScale );

				// Set Color
				RenderMesh renderMesh			= EntityManager.GetSharedComponentData< RenderMesh >( block );
				// renderMesh.material.color		= overlaps ? Color.blue : Color.gray;

				if (Input.GetMouseButtonUp( 0 ))
					PlaceOnGrid( block );
			}
	}


	void PlaceOnGrid( Entity block )
	{
		EntityManager.RemoveComponent< DragPosition >( block );
		EntityManager.RemoveComponent< IsDraggable >( block );
	}


	void SnapToGrid( Entity block, Entity grid, float gridScale )
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

		Translation translation			= EntityManager.GetComponentData< Translation >( block );
		translation						= new Translation{ Value = new float3( snappedPosition, translation.Value.z ) };
		EntityManager.SetComponentData( block, translation );
		EntityManager.SetComponentData( block, new Scale{ Value = gridScale } );
	}


	// Offset from center of the block to nearest cell center
	float2 OffsetToNearestCell( Entity block, float gridScale )
	{
		float2 blockSize_w				= WorldSizeOnGrid( block, gridScale );

		// _l suffix - local (but scaled) coordinates >>>

		float2 blockMin_l				= blockSize_w / (-2);
		float2 localMinCell_l			= blockMin_l + gridScale / 2;

		float2 blockCenter_l			= float2.zero;
		float2 someCellCenter_l			= localMinCell_l;
		float2 delta					= blockCenter_l - someCellCenter_l;

		float2 offsetToNearestCell		= delta.NegativeSafeMod( gridScale );

		return offsetToNearestCell;
	}


	bool FindGridOverlappedBy( Entity block, NativeArray< Entity > grids, out Entity grid, out float gridScale )
	{
		foreach (Entity entity in grids)
		{
			grid			= entity;
			gridScale		= EntityManager.GetComponentData< Scale >( grid ).Value;
			Rect rect		= GetRectOnGrid( block, gridScale );

			if (rect.Overlaps( GetRectOnGrid( grid, gridScale ) ))
				return true;
		}
		
		grid			= Entity.Null;
		gridScale		= 0;
		return false;
	}


	Rect GetRectOnGrid( Entity block, float gridScale )
	{
		Translation translation			= EntityManager.GetComponentData< Translation >( block );
		Vector2 size_w					= WorldSizeOnGrid( block, gridScale );
		Vector2 rectPos_w				= (Vector2)translation.Value.xy - size_w / 2;
		Rect rect_w						= new Rect( rectPos_w, size_w );

		return rect_w;
	}


	float2 WorldSizeOnGrid( Entity block, float gridScale )
	{
		int2 blockSize			= EntityManager.GetComponentData< BlockSize >( block ).Value;

		return (float2)blockSize * gridScale;
	}
}

