using Unity.Entities;
using Unity.Transforms;


[UpdateBefore( typeof(TransformSystemGroup) )]
public class DragSystemsGroup : ComponentSystemGroup
{
}

