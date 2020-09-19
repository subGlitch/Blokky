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
		_selectedShape		= CreateShape();
	}


	public void OnPointerDown( PointerEventData eventData )
	{
		if (_selectedShape != Entity.Null)
			CreateShape( Flags.IsDraggable );
	}


	Entity CreateShape( Flags flags = Flags.None )
	=>
		Factory.Instance.CreateBlock( transform.position, _selectedSize, flags );
}

