using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeImageCanvas : MonoBehaviour
{
    [SerializeField] float _ratio;
    [SerializeField] RectTransform rectCanvas;
    public RectTransform _rectTransform;
    void Start()
    {
        Resize();
    }

    public void Resize()
    {
        _ratio = rectCanvas.sizeDelta.x / 1920;
        _rectTransform = GetComponent<RectTransform>();
        Vector2 newSize = _rectTransform.sizeDelta;
        newSize.x *= _ratio;
        _rectTransform.sizeDelta = newSize;
    }
}
