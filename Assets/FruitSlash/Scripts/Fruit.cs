using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fruit : MonoBehaviour
{
    public float minSpeed = 3f; 
    public float maxSpeed = 6f;
    public int pointFruits;
    public Image image;
    public Collider2D colliderF;
    public TMP_Text textPoint;
    public float heightKnee;
    public delegate void choosenEvent(int point, Vector3 pos,Sprite s);
    public choosenEvent choosen;
    public RectTransform canvas;
    void Start() 
    {
        Fall(); 
    }

    void Fall() 
    { 
        float endY = -canvas.sizeDelta.y / 2; 
        float speed = Random.Range(minSpeed, maxSpeed); 
        transform.DOMoveY(endY, speed).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject)); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Contains("Knee"))
        {
            int pointR = pointFruits;
            if (collision.gameObject.name.Contains("Wrist")) pointR = 1 * pointFruits;
            else if (collision.gameObject.name.Contains("Ankle")) pointR = 2 * pointFruits;
            if (collision.gameObject.name.Contains("Wrist") || collision.gameObject.name.Contains("Ankle"))
            {
                Hide();
                choosen?.Invoke(pointR, transform.position, image.sprite);
            }
        }
        else
        {
            Hide();
        }
    }

    public void Hide()
    {
        image.enabled = false;
        colliderF.enabled = false;
        textPoint.enabled = false;
    }
}
