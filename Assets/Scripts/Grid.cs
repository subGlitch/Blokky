using UnityEngine;


public static class Grid
{
	public static float		LegoScale;
	public static float		LegoSize				=> LegoPrefabSize * LegoScale;


	static float			LegoPrefabSize			=> 1;		// Lego prefab has size of 1 unit
	

	public static Vector2 SetGridSize( Vector2Int gridSize, Rect availableSpace_w )
	{
		Rect rect_w				= CalcGridWorldRect( gridSize, availableSpace_w );

		float legoSize_w		= rect_w.size.x / gridSize.x;
		LegoScale				= legoSize_w / LegoPrefabSize;

		return rect_w.center;
	}


	static Rect CalcGridWorldRect( Vector2Int gridSize, Rect availableSpace_w )
	{
		Vector2 size		= gridSize;
		Vector2 scaleXY		= availableSpace_w.size / size;
		float scale			= Mathf.Min( scaleXY.x, scaleXY.y );
		size				*= scale;

		return new Rect { size = size, center = availableSpace_w.center };		// Keep this order: size first
	}
}

