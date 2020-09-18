using Unity.Entities;


[UpdateInGroup( typeof(DragSystemsGroup) )]
public abstract class DragSystemBase : ComponentSystem
{
	protected EntityManager		_entityManager;


	protected override void OnCreate()		=> _entityManager		= World.DefaultGameObjectInjectionWorld.EntityManager;
}

