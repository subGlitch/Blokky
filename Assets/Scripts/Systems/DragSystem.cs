using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


public class DragSystem : ComponentSystem
{
	Vector2		_mouseDragLast_w;


	protected override void OnUpdate()
	{
		bool isDragStart		= Input.GetMouseButtonDown( 0 );
		bool isDragContinue		= Input.GetMouseButton( 0 );

		if (!isDragStart && !isDragContinue)
			return;
		
		Vector2 mouse_w			= Camera.main.ScreenToWorldPoint( Input.mousePosition );

		if (!isDragStart)
			Shift( mouse_w - _mouseDragLast_w );

		_mouseDragLast_w		= mouse_w;
	}


	void Shift( Vector2 shift )
	{
		// Entity grid							= GetSingletonEntity< IsGrid >();								// Simple version
		EntityQuery query						= GetEntityQuery( typeof(IsGrid) );			// Generalized approach (can have multiple grids)

		using (NativeArray< Entity > grids		= query.ToEntityArray( Allocator.TempJob ))
		{
			if (grids.Length == 0)
				return;

			EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;

			Entities
				.WithAll< IsDraggable >()
				.ForEach( (Entity draggable, ref Translation translation) =>
			{
				translation						= new Translation{ Value = translation.Value + (float3)(Vector3)shift };

				bool overlaps					= FindGridOverlappedBy( draggable, grids ) != Entity.Null;

				RenderMesh renderMesh			= entityManager.GetSharedComponentData< RenderMesh >( draggable );
				renderMesh.material.color		= overlaps ? Color.blue : Color.gray;
			});
		}
	}


	Entity FindGridOverlappedBy( Entity block, NativeArray< Entity > grids )
	{
		Rect rect				= GetRect( block );

		foreach (Entity grid in grids)
		{
			Rect gridRect		= GetRect( grid );

			if (rect.Overlaps( gridRect ))
				return grid;
		}
		
		return Entity.Null;;
	}


	Rect GetRect( Entity block )
	{
		EntityManager entityManager		= World.DefaultGameObjectInjectionWorld.EntityManager;
		
		Translation translation			= entityManager.GetComponentData< Translation >( block );
		BlockSize blockSize				= entityManager.GetComponentData< BlockSize >( block );
		
		Vector2 size_w					= (float2)blockSize.size * Grid.LegoScale;
		Vector2 rectPos_w				= (Vector2)translation.Value.xy - size_w / 2;
		Rect rect_w						= new Rect( rectPos_w, size_w );

		return rect_w;
	}
}

