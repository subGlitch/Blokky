using UnityEngine;
using UnityEngine.UI;


public class ShapeSelector : MonoBehaviour
{
#pragma warning disable 0649

	[SerializeField] Vector2Int		_size;

#pragma warning restore 0649


	void Start()
	{
		CanvasScaler cs			= FindObjectOfType< CanvasScaler >();
		RectTransform rt		= GetComponent< RectTransform >();
		float screenWidth_w		= Camera.main.aspect * Camera.main.orthographicSize * 2;
		float shapeWidth_w		= _size.x * BlokkyEditor.UiScale;
		float shapeWidth		= shapeWidth_w * cs.referenceResolution.x / screenWidth_w;
		rt.sizeDelta			= new Vector2(
												rt.sizeDelta.x + shapeWidth,
												rt.sizeDelta.y
		);
		Canvas.ForceUpdateCanvases();

		Debug.Log( cs.scaleFactor );

        Factory.Instance.CreateBlock( transform.position, _size );
	}
}

