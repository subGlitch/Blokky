using UnityEngine.UI;


// http://answers.unity.com/answers/851816/view.html

 
public class Touchable : Text
{
	protected override void Awake()
	{
		base.Awake();
	}
}


/*
// http://answers.unity.com/answers/1165070/view.html

 
namespace UnityEngine.UI
{
	public class Touchable : Graphic
	{
		public override bool Raycast( Vector2 sp, Camera eventCamera )
		{
			// return base.Raycast( sp, eventCamera );
			return true;
		}

 
		protected override void OnPopulateMesh( VertexHelper vh )
		{
			vh.Clear();
		}
	}
}
*/

