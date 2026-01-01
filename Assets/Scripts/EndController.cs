using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndController : MonoBehaviour
{
    [SerializeField] GameObject objScoreTeam2;
    [SerializeField] TMP_Text labelTeam1;    
    [SerializeField] TMP_Text labelTeam2;    
    [SerializeField] TMP_Text pointTeam1;    
    [SerializeField] TMP_Text pointTeam2;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] TMP_Text continueText;
    [SerializeField] string _nextScene = "InputScene";

    private void OnEnable()
    {
        objScoreTeam2.SetActive(InputManager.Instance.mode == Mode.Scenario);

        labelTeam1.text = InputManager.Instance.mode == Mode.Scenario ? "Score\nTeam1" : "Score";

        pointTeam1.text = InputManager.Instance.pointTeam1.ToString();
        pointTeam2.text = InputManager.Instance.pointTeam2.ToString();




        if (InputManager.Instance.curGameScene < (InputManager.Instance.listGameScene.Count - 1))
        {
            Debug.Log("gsl;djflskjflks;;;jfl;;à;là");
            InputManager.Instance.curGameScene += 1;
            _nextScene = InputManager.Instance.listGameScene[InputManager.Instance.curGameScene];
        }
        else
        {

            Debug.Log("11111111111111111111111111111111111111");
            if (InputManager.Instance.repetition)
            {
                InputManager.Instance.curGameScene = 0;
                _nextScene = InputManager.Instance.listGameScene[InputManager.Instance.curGameScene];
            }
            else
            {
                _nextScene = InputManager.Instance.inputScene;
            }
        }
        StartCoroutine(CountdownCoroutine());
        //StartCoroutine(TypeText());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            SceneManager.LoadSceneAsync("InputScene");
        }

    }


    IEnumerator CountdownCoroutine()
    {
        int countdown = 5;
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        countdownText.text = "0";
        InputManager.Instance.pointTeam1 = 0;
        InputManager.Instance.pointTeam2 = 0;
        SceneManager.LoadSceneAsync(_nextScene);
        StopAllCoroutines();
        //gameObject.SetActive(false);
    }

    IEnumerator TypeText()
    {
        string text;
        while (true)
        {
            text = continueText.text;
            continueText.text = "";
            for (int i = 0; i < text.Length; i++)
            {
                continueText.text += text[i];
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
