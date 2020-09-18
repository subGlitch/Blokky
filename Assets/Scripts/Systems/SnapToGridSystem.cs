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
				bool overlaps					= grid != Entity.Null;

				RenderMesh renderMesh			= EntityManager.GetSharedComponentData< RenderMesh >( block );
				renderMesh.material.color		= overlaps ? Color.blue : Color.gray;
			}
	}


	float2 OffsetToNearestCell( Entity block )
	{
		// float2 dragPos		= _entityManager
		return float2.zero;
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
		
		Vector2 size_w					= (float2)blockSize.Value * Grid.LegoScale;
		Vector2 rectPos_w				= (Vector2)translation.Value.xy - size_w / 2;
		Rect rect_w						= new Rect( rectPos_w, size_w );

		return rect_w;
	}
}

