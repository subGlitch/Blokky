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
		EntityQuery query						= GetEntityQuery( typeof(IsGrid) );
		using (NativeArray< Entity > grids		= query.ToEntityArray( Allocator.TempJob ))
		{
			if (grids.Length == 0)
				return;

			EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;

			Entities
				.WithAll< IsDraggable >()
				.ForEach( (Entity entity, ref Translation translation, ref BlockSize blockSize) =>
			{
				translation						= new Translation{ Value = translation.Value + (float3)(Vector3)shift };

				Vector2 size_w					= (float2)blockSize.size * Grid.LegoScale;
				Vector2 rectPos_w				= (Vector2)translation.Value.xy - size_w / 2;
				Rect rect						= new Rect( rectPos_w, size_w );

				bool interscects				= rect.yMin > 0;

				RenderMesh renderMesh			= entityManager.GetSharedComponentData< RenderMesh >( entity );
				renderMesh.material.color		= interscects ? Color.blue : Color.gray;
			});
		}
	}
}

