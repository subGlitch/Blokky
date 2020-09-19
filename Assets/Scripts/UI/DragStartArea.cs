using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragStartArea : MB_Singleton< DragStartArea >, IPointerDownHandler
{
	Entity		_selectedShape		= Entity.Null;
	Vector2Int	_selectedSize;


	public void SelectShape( Vector2Int size)
	{
		if (_selectedShape != Entity.Null)
			Utilities.DestroyHierarchy( _selectedShape );

		_selectedSize		= size;
		_selectedShape		= Factory.Instance.CreateBlock(
															transform.position,
															size
		);
	}


	public void OnPointerDown( PointerEventData eventData )
	=>
		Factory.Instance.CreateBlock(
			transform.position,
			_selectedSize,
			Flags.IsDraggable
		);
}

