using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class GivenShape : MonoBehaviour
{
    public float minSpeed = 3f;
    public float maxSpeed = 6f;
    public int indexImage = 1;
    public int indexGiven = 1;
    public Image image;
    public Collider2D colliderF;
    public delegate void choosenEvent(int index, bool addpoint, int pointN, Vector3 pos, bool isLeft);
    public choosenEvent choosen;
    public RectTransform canvas;
    public ParticleSystem particleSystem;

    void Start()
    {
        //Fall();
    }
    public void ArcMove(Vector3 toPoint)
    {
        colliderF.enabled = false;
        Vector3 t = toPoint;
        t.y=transform.position.y;
        toPoint = t;

        Vector3 midPoint = (transform.position + toPoint) / 2;
        midPoint.y += Mathf.Abs(midPoint.x/3);
        Vector3[] path = new Vector3[3]; 
        path[0] = transform.position; 
        path[1] = midPoint;
        path[2] = toPoint;

        particleSystem.Play();
        transform.DOPath(path, 1.5f, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
        {
            colliderF.enabled = true;
            particleSystem.Stop();
            Fall();
        });
    }

    public void Fall()
    {
        float endY = -canvas.sizeDelta.y / 2;
        float speed = Random.Range(minSpeed, maxSpeed);
        transform.DOMoveY(endY, speed).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Eaten"))
        {
            Hide();
            choosen?.Invoke(indexImage, true, indexGiven == indexImage ? 5 : -1, transform.localPosition, false);
        }
        else if(collision.gameObject.name.Contains("Wrist"))
        {
            Hide();
            choosen?.Invoke(indexImage, false, 5, transform.position, collision.gameObject.transform.position.x > transform.position.x);         
        }
    }

    public void Hide()
    {
        image.enabled = false;
        colliderF.enabled = false;
        //textPoint.enabled = false;
    }
}
