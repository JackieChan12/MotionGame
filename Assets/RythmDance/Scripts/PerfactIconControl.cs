using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PerfactIconControl : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    private void OnEnable()
    {
        rectTransform.DOScale(Vector3.one * 1.5f, 1.3f)
            .SetEase(Ease.OutBack);

        canvasGroup.DOFade(0, 0.5f)
            .SetDelay(0.8f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Destroy(this.gameObject);
            });
    }
}
