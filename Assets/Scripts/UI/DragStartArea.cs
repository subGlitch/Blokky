using UnityEngine;
using UnityEngine.EventSystems;


public class DragStartArea : MB_Singleton< DragStartArea >, IPointerDownHandler
{
	bool	_isDrag;


	public void OnPointerDown( PointerEventData eventData )
	{
		_isDrag		= true;

		Factory.Instance.CreateBlock(
			transform.position,
			new Vector2Int( 3, 2 ),
			Flags.IsDraggable
		);
	}
}

