using UnityEngine;
using UnityEngine.EventSystems;


public class DragStartArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
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

