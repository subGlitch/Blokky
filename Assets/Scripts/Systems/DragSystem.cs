using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class DragSystem : DragSystemBase
{
	Vector2			Mouse_w			=> Camera.main.ScreenToWorldPoint( Input.mousePosition );


	Vector2			_mouseDragLast_w;


	enum DragState
	{
		None,

		Start,
		Continue,
		Finish,
	}


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
			EntityManager.AddComponentData( draggable, new DragPosition( translation.Value.xy ) );
		});


	void DragFinish()
	=>
		GetDraggable().ForEach( draggable =>
		{
			EntityManager.RemoveComponent< DragPosition >( draggable );
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
				float2 dragPosition				= dragPosComponent.Value + (float2)shift;
				dragPosComponent				= new DragPosition( dragPosition );

				float3 position					= new float3( dragPosition, translation.Value.z );
				translation						= new Translation{ Value = position };
			});
		}
	}


	protected EntityQueryBuilder GetDraggable()		=> Entities.WithAll< IsDraggable >();
}

