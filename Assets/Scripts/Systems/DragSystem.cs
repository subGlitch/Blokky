﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;


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
	    EntityManager entityManager		= World.DefaultGameObjectInjectionWorld.EntityManager;
			
		Entities
			.WithAll< Draggable >()
			.ForEach( (Entity entity, ref Translation translation) =>
		{
			translation						= new Translation{ Value = translation.Value + (float3)(Vector3)shift };

			RenderMesh renderMesh			= entityManager.GetSharedComponentData< RenderMesh >( entity );
			// renderMesh.material.color		= Random.ColorHSV();
		});
	}
}
