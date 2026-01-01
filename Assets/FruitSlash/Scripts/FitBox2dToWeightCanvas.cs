using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitBox2dToWeightCanvas : MonoBehaviour
{
    [SerializeField] RectTransform canvas;
    [SerializeField] BoxCollider2D box;
    [SerializeField] float height = 50;

    private void Start()
    {
        box.size = new Vector2(canvas.sizeDelta.x, height);
    }
}
