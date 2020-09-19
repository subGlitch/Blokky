using UnityEngine.EventSystems;


public class DragStartArea : MB_Singleton< DragStartArea >, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
		Utilities.Log();
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

