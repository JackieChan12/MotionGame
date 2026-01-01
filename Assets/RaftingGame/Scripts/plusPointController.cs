using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plusPointController : MonoBehaviour
{
    public Transform sphereBoom;
    public Transform player;
    public float distanceZ;
    public bool startAnim = false;

    // Update is called once per frame
    void Update()
    {
        if (startAnim) return;
        if((sphereBoom.position.z - player.position.z ) < distanceZ)
        {
            startAnim = true;
            Sequence mySequence = DOTween.Sequence();
            mySequence.Join(sphereBoom.DOLocalMoveY(2.35f, 2f).SetEase(Ease.OutQuad));
            mySequence.Join(sphereBoom.DOScale(2f, 2f));
        }
    }
}
