using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


public class DragSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;

		if (
				Input.GetMouseButtonDown( 0 ) ||
				Input.GetMouseButton( 0 )
			)
			Entities
				.WithAll< DraggableComponent >()
				.ForEach( (Entity entity) =>
			{
				DynamicBuffer< Cell > cells			= entityManager.GetBuffer< Cell >( entity );
				Translation translation				= new Translation { Value = (Vector3)(Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition ) };

				entityManager.SetComponentData( cells[ 0 ].cell, translation );
			});
	}
}

