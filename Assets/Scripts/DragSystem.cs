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
			Entities
				.WithAll< GridPositionComponent >()
				.ForEach( (Entity entity, ref Translation translation) =>
			{
				translation.Value		+= new float3( (Vector3)(Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition ) );
			});
	}
}

