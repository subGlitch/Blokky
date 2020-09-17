using UnityEngine;


public class Main : MonoBehaviour
{
	void Start()
	{
		Vector2Int gridSize		= new Vector2Int( 17, 17 );

		Grid.SetGridSize( gridSize );

		Factory.Instance.CreateBlock( Vector2.zero, gridSize, false );
		Factory.Instance.CreateBlock( Vector2.one, new Vector2Int( 3, 2 ), true );
	}
}

