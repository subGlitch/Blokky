using UnityEngine;


public class ShapeSelectors : MonoBehaviour
{
	void Start()
	{
		foreach (Transform child in transform)
			child.GetComponent< ShapeSelector >().Resize();

		Canvas.ForceUpdateCanvases();

		foreach (Transform child in transform)
			child.GetComponent< ShapeSelector >().CreateShape();
	}
}

