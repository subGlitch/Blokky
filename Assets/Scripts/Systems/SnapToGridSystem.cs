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
				Entity grid						= FindGridOverlappedBy( block, grids );
				if (grid == Entity.Null)
					continue;

				float2 gridPos					= EntityManager.GetComponentData< Translation >( grid ).Value.xy;
				float2 dragPos					= EntityManager.GetComponentData< DragPosition >( block ).Value;

				float2 otnc_Grid				= OffsetToNearestCell( grid );
				float2 otnc_Block				= OffsetToNearestCell( block );

				// For snapping it doesn't matter which cell to take (here we taking nearest from block's center), so any cell will do
				float2 anyCellOnGrid			= gridPos + otnc_Grid;
				float2 anyCellOnBlock			= dragPos + otnc_Block;

				float2 snapPositive				= (anyCellOnGrid - anyCellOnBlock).NegativeSafeMod( Grid.LegoSize );
				float2 snapNegative				= snapPositive - Grid.LegoSize;
				float2 snapNearest				= new float2(
																snapPositive.x < snapNegative.x * (-1) ? snapPositive.x : snapNegative.x,
																snapPositive.y < snapNegative.y * (-1) ? snapPositive.y : snapNegative.y
				);

				float2 snappedPosition			= dragPos + snapNearest;

				Translation translation			= EntityManager.GetComponentData< Translation >( block );
				translation						= new Translation{ Value = new float3( snappedPosition, translation.Value.z ) };
				EntityManager.SetComponentData( block, translation );

				RenderMesh renderMesh			= EntityManager.GetSharedComponentData< RenderMesh >( block );
				// renderMesh.material.color		= overlaps ? Color.blue : Color.gray;
			}
	}


	// Offset from center of the block to nearest cell center
	float2 OffsetToNearestCell( Entity block )
	{
		int2 blockSize					= EntityManager.GetComponentData< BlockSize >( block ).Value;
		float2 blockSize_w				= blockSize.ToWorldSize();

		// _l suffix - local (but scaled) coordinates >>>

		float2 blockMin_l				= blockSize_w / (-2);
		float2 localMinCell_l			= blockMin_l + Grid.LegoSize / 2;

		float2 blockCenter_l			= float2.zero;
		float2 someCellCenter_l			= localMinCell_l;
		float2 delta					= blockCenter_l - someCellCenter_l;

		float2 offsetToNearestCell		= delta.NegativeSafeMod( Grid.LegoSize );

		return offsetToNearestCell;
	}


	Entity FindGridOverlappedBy( Entity block, NativeArray< Entity > grids )
	{
		Rect rect		= GetRect( block );

		foreach (Entity grid in grids)
			if (rect.Overlaps( GetRect( grid ) ))
				return grid;
		
		return Entity.Null;;
	}


	Rect GetRect( Entity block )
	{
		Translation translation			= EntityManager.GetComponentData< Translation >( block );
		BlockSize blockSize				= EntityManager.GetComponentData< BlockSize >( block );
		
		Vector2 size_w					= blockSize.Value.ToWorldSize();
		Vector2 rectPos_w				= (Vector2)translation.Value.xy - size_w / 2;
		Rect rect_w						= new Rect( rectPos_w, size_w );

		return rect_w;
	}
}

