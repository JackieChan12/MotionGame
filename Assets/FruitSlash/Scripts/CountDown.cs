using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDown : MonoBehaviour
{
    public TMP_Text countdownText;
    public int countdown = 5;
    int cd;
    private void Awake()
    {
        cd = countdown;
    }
    void OnEnable() 
    {
        countdown = cd;
        StartCoroutine(CountdownCoroutine()); 
    } 
    IEnumerator CountdownCoroutine() 
    { 
         
        while (countdown > 0) 
        { 
            countdownText.text = countdown.ToString(); 
            yield return new WaitForSeconds(1f);
            countdown--; 
        } 
        countdownText.text = "Go!";
        gameObject.SetActive(false);
    }
}
