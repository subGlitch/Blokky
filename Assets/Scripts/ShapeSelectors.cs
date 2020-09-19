using UnityEngine;


public class ShapeSelectors : MB_Singleton< ShapeSelectors >
{
	public void Init()
	{
		foreach (Transform child in transform)
			child.GetComponent< ShapeSelector >().Resize();

		Canvas.ForceUpdateCanvases();

		foreach (Transform child in transform)
			child.GetComponent< ShapeSelector >().CreateShape();
	}
}

