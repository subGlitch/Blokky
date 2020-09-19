using UnityEngine;


public static class Grid
{
	public static float CalcScale( Vector2Int gridSize, RectTransform space )
	{
		Rect availableSpace_w		= space.GetWorldRect();
		Rect rect_w					= CalcGridWorldRect( gridSize, availableSpace_w );
		float legoSize_w			= rect_w.size.x / gridSize.x;

		float legoPrefabSize		= 1;
		float scale					= legoSize_w / legoPrefabSize;

		return scale;
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

