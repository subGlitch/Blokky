﻿using Unity.Entities;
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
			case DragState.Start:
				DragStart();
				break;

			case DragState.Continue:
			case DragState.Finish:
				DragContinue( Mouse_w - _mouseDragLast_w );
				break;

			default:
				return;
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


	void DragContinue( Vector2 shift )
	=>
		GetDraggable().ForEach(
			(
				Entity				draggable,
				ref Translation		translation,
				ref DragPosition	dragPosComponent,
				ref Scale			scale
			) =>
		{
			float2 dragPosition		= dragPosComponent.Value + (float2)shift;
			dragPosComponent		= new DragPosition( dragPosition );

			float3 position			= new float3( dragPosition, translation.Value.z );
			translation				= new Translation{ Value = position };

			scale					= new Scale{ Value = BlokkyEditor.UiScale };
		});


	protected EntityQueryBuilder GetDraggable()		=> Entities.WithAll< IsDraggable >();
}

