using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


public class DragSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		if (
				Input.GetMouseButtonDown( 0 ) ||
				Input.GetMouseButton( 0 )
			)
			Entities.ForEach( (Entity entity, ref Translation translation, ref GridPositionComponent pos) =>
			{
				translation.Value					= new float3( (Vector3)(Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition ) );

		        EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;
				var renderMesh						= entityManager.GetSharedComponentData< RenderMesh >( entity );
				var mat								= new Material( renderMesh.material );
				mat.SetColor( "_Color", UnityEngine.Random.ColorHSV() );
				renderMesh.material					= mat;

				entityManager.SetSharedComponentData( entity, renderMesh );
			});
	}
}

