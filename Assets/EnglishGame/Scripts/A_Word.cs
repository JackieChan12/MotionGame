using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class A_Word : MonoBehaviour
{

    public Image _image;
    public TMP_Text _textChar;
    public char _word;

    public Transform posSpawn;
    public Transform posTarget;
    public Transform posRight;
    bool canChoose = false;
    public Sequence mySequence;
    public delegate void choosenEvent(A_Word w);
    public choosenEvent choosen;
    Vector3 targetPosition;
    Vector3 spawnPosition;
    Vector3 rightPosition;
    private void Start()
    {
        targetPosition = posTarget.localPosition;
        spawnPosition = posSpawn.localPosition;
    }
    public void Begin()
    {
        targetPosition = posTarget.localPosition;
        spawnPosition = posSpawn.localPosition;
        rightPosition = posRight.localPosition;
        transform.localPosition = spawnPosition;
        canChoose = false;
        //_word = w;
        //_image.sprite = s;
        mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOLocalMove(targetPosition, 1).SetEase(Ease.OutQuad));
        mySequence.Append(transform.DOScale(Vector3.one, 1).SetEase(Ease.OutQuad));
        mySequence.Play();
        mySequence.OnComplete(() =>
        {
            canChoose = true;
        });
    }

    public void ResetWord()
    {
        canChoose = false;
        mySequence.Kill();
        mySequence.Append(transform.DOLocalMove(spawnPosition, 1).SetEase(Ease.Linear));
        mySequence.Append(transform.DOScale(Vector3.zero, 1).SetEase(Ease.OutQuad));
        mySequence.Play();

    }

    public void RightChoose()
    {
        canChoose = false;
        //mySequence.Kill();
        if (mySequence == null || !mySequence.IsActive())
        {
            mySequence = DOTween.Sequence();
        }
        mySequence.Append(transform.DOLocalMove(rightPosition, .5f).SetEase(Ease.Linear));
        mySequence.Append(transform.DOScale(Vector3.zero, .5f).SetEase(Ease.OutQuad));
        mySequence.Play();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canChoose) choosen?.Invoke(this);
    }
}
