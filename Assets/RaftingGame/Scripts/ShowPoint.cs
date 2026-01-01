using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowPoint : MonoBehaviour
{
    public TMP_Text m_Text;
    Sequence mySequence;
    private void OnEnable()
    {
        mySequence = DOTween.Sequence();
        transform.localPosition = Vector3.zero;
        mySequence.Join(transform.DOLocalMoveY(2f, 3f).SetEase(Ease.OutQuad));
        mySequence.Join(transform.DOScale(2f, 3f));
        mySequence.OnComplete(() =>
        {
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        });
    }
}
