using Unity.Entities;


[UpdateInGroup( typeof(DragSystemsGroup) )]
[UpdateAfter( typeof(DragSystem) )]
public class SnapToGridSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
	}
}

