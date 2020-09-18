using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


[UpdateInGroup( typeof(DragSystemsGroup) )]
public class DragSystem : ComponentSystem
{
	Vector2			Mouse_w			=> Camera.main.ScreenToWorldPoint( Input.mousePosition );


	EntityManager	_entityManager;
	Vector2			_mouseDragLast_w;


	enum DragState
	{
		None,

		Start,
		Continue,
		Finish,
	}


	protected override void OnCreate()		=> _entityManager		= World.DefaultGameObjectInjectionWorld.EntityManager;


	protected override void OnUpdate()
	{
		DragState state			=
									Input.GetMouseButtonDown( 0 )	?	DragState.Start		:
									Input.GetMouseButtonUp( 0 )		?	DragState.Finish	:
									Input.GetMouseButton( 0 )		?	DragState.Continue	:
																		DragState.None
		;

		switch (state)
		{
			case DragState.Start:		DragStart();											break;
			case DragState.Continue:	DragContinue( Mouse_w - _mouseDragLast_w );		break;
			case DragState.Finish:		DragFinish();											break;

			default:					return;
		}

		_mouseDragLast_w		= Mouse_w;
	}


	void DragStart()
	=>
		GetDraggable().ForEach(
			(
				Entity draggable,
				ref Translation translation					// Can't use 'in' param inside ComponentSystem, it's possible only in SystemBase =(((
			) =>
		{
			_entityManager.AddComponentData( draggable, new DragPosition( translation.Value.xy ) );
		});


	void DragFinish()
	=>
		GetDraggable().ForEach( draggable =>
		{
			_entityManager.RemoveComponent< DragPosition >( draggable );
		});


	void DragContinue( Vector2 shift )
	{
		// Entity grid							= GetSingletonEntity< IsGrid >();								// Simple version
		EntityQuery query						= GetEntityQuery( typeof(IsGrid) );			// Generalized approach (can have multiple grids)

		using (NativeArray< Entity > grids		= query.ToEntityArray( Allocator.TempJob ))
		{
			if (grids.Length == 0)
				return;

			GetDraggable().ForEach(
				(
					Entity draggable,
					ref Translation translation,
					ref DragPosition dragPosComponent
				) =>
			{
				float2 dragPosition				= translation.Value.xy + (float2)shift;
				dragPosComponent				= new DragPosition( dragPosition );

				float3 position					= new float3( dragPosition, translation.Value.z );
				translation						= new Translation{ Value = position };

				bool overlaps					= FindGridOverlappedBy( draggable, grids ) != Entity.Null;

				RenderMesh renderMesh			= _entityManager.GetSharedComponentData< RenderMesh >( draggable );
				renderMesh.material.color		= overlaps ? Color.blue : Color.gray;
			});
		}
	}


	EntityQueryBuilder GetDraggable()		=> Entities.WithAll< IsDraggable >();


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
		Translation translation			= _entityManager.GetComponentData< Translation >( block );
		BlockSize blockSize				= _entityManager.GetComponentData< BlockSize >( block );
		
		Vector2 size_w					= (float2)blockSize.Value * Grid.LegoScale;
		Vector2 rectPos_w				= (Vector2)translation.Value.xy - size_w / 2;
		Rect rect_w						= new Rect( rectPos_w, size_w );

		return rect_w;
	}
}

