using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    public Image imageTime;
    public float defaultTimeEnd = 5;
    public TMP_Text text;

    public delegate void choosenEvent(Square s);
    public choosenEvent choosen; 
    
    public delegate void RemoveEvent(Square s);
    public choosenEvent removeEvent;

    public GameObject _iconPerfect;
    public GameObject _iconGood;
    public GameObject _iconMiss;
    float timeCount = 0;
    float fill = 0;
    bool haveMiss = false;

    private void OnEnable()
    {
        timeCount = 0;
        imageTime.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeCount <= defaultTimeEnd)
        {
            timeCount += Time.deltaTime;
            fill = (defaultTimeEnd - timeCount)/defaultTimeEnd;
            imageTime.fillAmount = fill;
            imageTime.color = Color.Lerp(Color.green, Color.red, 1 - fill);
        }
        else
        {
            removeEvent?.Invoke(this);
            if (!haveMiss)
            {
                Instantiate(_iconMiss, transform.position, Quaternion.identity, transform.parent).SetActive(true);
                haveMiss = true;
            }
            //Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Wrist"))
        {
            if(timeCount > defaultTimeEnd*0.4f && timeCount<=defaultTimeEnd*0.6f) Instantiate(_iconGood, transform.position, Quaternion.identity, transform.parent).SetActive(true);
            else if(timeCount >= defaultTimeEnd*0f && timeCount<=defaultTimeEnd*0.4f) Instantiate(_iconPerfect, transform.position, Quaternion.identity, transform.parent).SetActive(true);
            choosen?.Invoke(this);
        }
    }

}
