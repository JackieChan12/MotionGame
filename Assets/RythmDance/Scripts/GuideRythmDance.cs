using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Square;

public class GuideRythmDance : MonoBehaviour
{
    public float countDown ;
    public TMP_Text countdownText;

    public Image imageTime;
    public float defaultTimeEnd = 1.5f;
    public GameObject square;
    float timeCount = 0;
    float fill = 0;

    public delegate void EndHintEvent();
    public EndHintEvent endHintEvent;

    private void OnEnable()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private void Update()
    {
        if (timeCount < defaultTimeEnd)
        {
            timeCount += Time.deltaTime;
            fill = (defaultTimeEnd - timeCount) / defaultTimeEnd;
            imageTime.fillAmount = fill;
            imageTime.color = Color.Lerp(Color.green, Color.red, 1 - fill);
        }
        else if (timeCount < defaultTimeEnd + 0.3f)
        {
            timeCount += Time.deltaTime;
            square.SetActive(false);
        }
        else
        {
            timeCount = 0;
            square.SetActive(true);
        }
    }

    IEnumerator CountdownCoroutine()
    {
        int countdown = 10;
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        endHintEvent?.Invoke();
        gameObject.SetActive(false);
    }
}
