﻿using UnityEngine;


public static class Grid
{
	public static float	LegoScale;
	

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

