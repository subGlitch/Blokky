using UnityEngine;
using UnityEngine.UI;


public class ShapeSelector : MonoBehaviour
{
#pragma warning disable 0649

	[SerializeField] Vector2Int		_size;

#pragma warning restore 0649


	public void Resize()
	{
		CanvasScaler cs			= FindObjectOfType< CanvasScaler >();
		RectTransform rt		= GetComponent< RectTransform >();
		float screenWidth_w		= Camera.main.aspect * Camera.main.orthographicSize * 2;
		Vector2 shapeSize_w		= (Vector2)_size * BlokkyEditor.UiScale;
		Vector2 shapeSize		= shapeSize_w * cs.referenceResolution.x / screenWidth_w;

		rt.sizeDelta			= rt.sizeDelta + shapeSize;
	}


	public void CreateShape()
	{
        Factory.Instance.CreateBlock( transform.position, _size );
	}


	public void OnSelect()		=> DragStartArea.Instance.SelectShape( _size );
}

