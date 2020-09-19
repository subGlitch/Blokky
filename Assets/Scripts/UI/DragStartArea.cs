using UnityEngine;
using UnityEngine.EventSystems;


public class DragStartArea : MB_Singleton< DragStartArea >, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
		Factory.Instance.CreateBlock(
			transform.position,
			new Vector2Int( 3, 2 ),
			Flags.IsDraggable
		);
	}


	public void OnDrag(PointerEventData eventData)
	{
		Utilities.Log();
	}


	public void OnPointerUp(PointerEventData eventData)
	{
		Utilities.Log();
	}
}

