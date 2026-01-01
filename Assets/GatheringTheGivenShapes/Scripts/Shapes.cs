using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum typeJointShape
{
    head,
    wirst,
    ankle
}

public class Shapes : MonoBehaviour
{
    public float minSpeed = 3f;
    public float maxSpeed = 6f;
    public typeJointShape type;
    public int point =1;
    public Image image;
    public Collider2D colliderF;
    //public TMP_Text textPoint;
    public float heightKnee;
    public delegate void choosenEvent(int point, int indexteam = 0);
    public choosenEvent choosen;
    void Start()
    {
        Fall();
    }

    void Fall()
    {
        float endY = -Screen.height / 2;
        float speed = Random.Range(minSpeed, maxSpeed);
        transform.DOMoveY(endY, speed).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.name.Contains("Knee"))
        {
            //int pointR = pointFruits;
            //if (collision.gameObject.name.Contains("Wrist")) pointR = 2 * pointFruits;
            //else if (collision.gameObject.name.Contains("Ankle")) pointR = 3 * pointFruits;
            //choosen?.Invoke(pointR);
            if((collision.gameObject.name.Contains("Head") && type == typeJointShape.head)
                || (collision.gameObject.name.Contains("Wrist") && type == typeJointShape.wirst)
                || (collision.gameObject.name.Contains("Ankle") && type == typeJointShape.ankle)
                || point ==-1)
            {
                choosen?.Invoke(point);
                Hide();
            }
        }
    }

    public void Hide()
    {
        image.enabled = false;
        colliderF.enabled = false;
        //textPoint.enabled = false;
    }
}
