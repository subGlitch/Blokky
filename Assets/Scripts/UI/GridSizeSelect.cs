using UnityEngine;


public class GridSizeSelect : MonoBehaviour
{
	public void OnSelect( int size )
	{
		Vector2Int gridSize		= Vector2Int.one * size;

		gameObject.SetActive( false );
		BlokkyEditor.Instance.Init( gridSize );
	}
}

