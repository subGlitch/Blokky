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
		_selectedShape		= CreateShape( transform.position  );
	}


	public void OnPointerDown( PointerEventData eventData )
	{
		/*
		Vector2 fingerOffset		=
										Application.isMobilePlatform	?
										new Vector2( 0, .5f )		:
										Vector2.zero
		;
		*/
		// This probably should be zero, if we are NOT on the mobile platform (see code above),
		// ... but for the sake of similarity with the reference app let's leave it.
		Vector2 fingerOffset		= new Vector2( 0, .5f );


		Vector2 position			= Utilities.Mouse_w + fingerOffset;

		if (_selectedShape != Entity.Null)
			CreateShape( position, Flags.IsDraggable );
	}


	Entity CreateShape( Vector2 position, Flags flags = Flags.None )
	=>
		Factory.Instance.CreateBlock( position, _selectedSize, flags );
}

