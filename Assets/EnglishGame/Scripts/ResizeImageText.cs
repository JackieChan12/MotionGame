using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeImageText : MonoBehaviour
{
    [SerializeField] float _ratio;
    [SerializeField] RectTransform rectCanvas;
    public RectTransform _rectTransform;
    public BoxCollider2D _boxCollider;
    [SerializeField] bool resizeHor = false;
    void Start()
    {
        Resize();
    }

    public void Resize()
    {
        _ratio = rectCanvas.sizeDelta.x / 1920;
        _rectTransform = GetComponent<RectTransform>();
        Vector2 newSize = _rectTransform.sizeDelta;
        if (resizeHor)
        {
            newSize.x *=_ratio;
        }
        else
        {
            newSize *= _ratio;
        }
        _rectTransform.sizeDelta = newSize;
        if (_boxCollider != null) _boxCollider.size = newSize;
    }
}
