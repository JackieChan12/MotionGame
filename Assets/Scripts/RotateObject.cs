using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationDuration = 2.0f;
    public Vector3 rotationAxis = Vector3.forward;
    void OnEnable()
    {
        transform.DORotate(rotationAxis * -360f, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }
}
