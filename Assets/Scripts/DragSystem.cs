using Unity.Entities;
using Unity.Mathematics;
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
			Entities.ForEach( (ref Translation translation, ref GridPositionComponent pos) =>
			{
				translation.Value		= new float3( (Vector3)(Vector2)Camera.main.ScreenToWorldPoint( Input.mousePosition ) );
			});
	}
}

