using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class DragSystem : ComponentSystem
{
	Vector2		_mouseLast_w;


	protected override void OnUpdate()
	{
		bool isDragStart		= Input.GetMouseButtonDown( 0 );
		bool isDragContinue		= Input.GetMouseButton( 0 );

		if (!isDragStart && !isDragContinue)
			return;
		
		Vector2 mouse_w			= Camera.main.ScreenToWorldPoint( Input.mousePosition );

		if (!isDragStart)
			Shift( mouse_w - _mouseLast_w );

		_mouseLast_w			= mouse_w;
	}


	void Shift( Vector2 shift )
	{
	    EntityManager entityManager				= World.DefaultGameObjectInjectionWorld.EntityManager;
			
		Entities.ForEach( (Entity entity, ref DraggableComponent draggable) =>
		{
			DynamicBuffer< Cell > cells			= entityManager.GetBuffer< Cell >( entity );
			Translation cellTranslation			= entityManager.GetComponentData< Translation >( cells[ 0 ].cell );
			cellTranslation.Value				+= (float3)(Vector3)shift;

			entityManager.SetComponentData( cells[ 0 ].cell, cellTranslation );
		});
	}
}

