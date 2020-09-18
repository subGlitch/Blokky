using Unity.Mathematics;
using UnityEngine;


static class GridExtMethods
{
	public static float2 ToWorldSize( this int2 blockSize )		=> (float2)blockSize * Grid.LegoSize;
}


public static class Grid
{
	public static float		LegoScale;
	public static float		LegoSize				=> LegoPrefabSize * LegoScale;


	static float			LegoPrefabSize			=> 1;		// Lego prefab has size of 1 unit
	

	public static void SetGridSize( Vector2Int gridSize )
	{
		Vector2 gridSize_w			= CalcGridWorldSize( gridSize );

		float legoSize_w			= gridSize_w.x / gridSize.x;
		float legoPrefabSize_w		= 1;
		LegoScale					= legoSize_w / legoPrefabSize_w;
	}


	static Vector2 CalcGridWorldSize( Vector2Int gridSize )
	{
		Camera mainCamera			= Camera.main;

		float screenWorldHeight		= mainCamera.orthographicSize * 2;
		float screenWorldWidth		= screenWorldHeight * mainCamera.aspect;

		float girdWorldWidth		= screenWorldWidth;
		float girdWorldHeight		= girdWorldWidth * gridSize.y / gridSize.x;

		return new Vector2( girdWorldWidth, girdWorldHeight );
	}
}

