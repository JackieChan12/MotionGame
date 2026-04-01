using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimalRaceMain : MonoBehaviour
{
    [Header("Player Control")]
    public AnimalRaceNew player01;
    public AnimalRaceNew player02;

    public Camera cameraPlayer01;
    public Camera cameraPlayer02;
    public GameObject point01;
    public GameObject point02;

    [Header("Game")]

    public float timeCount = 0;
    public float countDown = 5;
    bool countdownFirst = true;

    [Header("Instruction")]
    public float instructionTime = 5f;
    private bool isShowingInstruction = true;

    public TMP_Text textPoint01;
    public TMP_Text textPoint02;
    public TMP_Text textTime;
    public Image imageTime;
    public GameObject objectCountDown;
    public GameObject noticeTimeOut;
    public AudioController audioController;
    public string _nextScene = "OutputScene";
    int countPlayers = 0;


    [Header("Input")]
    public Mode mode;
    public Difficulty difficulty;
    public int players;
    public float playTime;
    public bool explanation;
    public bool photoTime;

    bool finish = false;

    [Header("Output")]
    public float pointTeam1 = 0;
    public float pointTeam2 = 0;

    private void Reset()
    {
        var allPlayers = FindObjectsOfType<AnimalRaceNew>();
        if (allPlayers.Length > 0 && player01 == null) { player01 = allPlayers[0]; player01.indexPlayer = 0; }
        if (allPlayers.Length > 1 && player02 == null) { player02 = allPlayers[1]; player02.indexPlayer = 1; }

        if (textPoint01 == null) textPoint01 = GameObject.Find("TextPoint01")?.GetComponent<TMP_Text>();
        if (textPoint02 == null) textPoint02 = GameObject.Find("TextPoint02")?.GetComponent<TMP_Text>();
        if (textTime == null) textTime = GameObject.Find("TextTime")?.GetComponent<TMP_Text>();
        if (imageTime == null) imageTime = GameObject.Find("ImageTime")?.GetComponent<Image>();
        if (objectCountDown == null) objectCountDown = GameObject.Find("ObjectCountDown");
        if (noticeTimeOut == null) noticeTimeOut = GameObject.Find("NoticeTimeOut");
        if (audioController == null) audioController = FindObjectOfType<AudioController>();
    }

    [System.Obsolete]
    void Start()
    {
        if (InputManager.Instance != null) SetupInput(InputManager.Instance.mode, InputManager.Instance.difficulty, InputManager.Instance.players, InputManager.Instance.playTime, InputManager.Instance.explanation, InputManager.Instance.photoTime);
        
        if(audioController) audioController.audioSourceBGM.pitch = 1.49f;
        
        if (player01 != null) { player01.indexPlayer = 0; player01.RandomAnimal(difficulty); }
        if (player02 != null) { player02.indexPlayer = 1; player02.RandomAnimal(difficulty); }

        SetupGame(mode);
    }

    [System.Obsolete]
    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (nuitrack.Nuitrack.GetModule(nuitrack.nuitrack_device_api.depth_sensor) != null) nuitrack.Nuitrack.Release();
        //    SceneManager.LoadSceneAsync("InputScene");
        //}
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            nuitrack.Nuitrack.Release();
            SceneManager.LoadSceneAsync("InputSceneSS2");
            //Application.Quit(); 
        }
        if (player01 != null) pointTeam1 = player01.pointDistance;
        if (player02 != null) pointTeam2 = player02.pointDistance;
        
        if (textPoint01 != null) textPoint01.text = pointTeam1.ToString("F0") + "M";
        if (textPoint02 != null) textPoint02.text = pointTeam2.ToString("F0") + "M";

        if (isShowingInstruction)
        {
            if (objectCountDown != null && objectCountDown.activeSelf)
                objectCountDown.SetActive(false);

            instructionTime -= Time.deltaTime;
            if (instructionTime <= 0)
            {
                isShowingInstruction = false;
                if (player01 != null) player01.HideInstruction();
                if (player02 != null) player02.HideInstruction();
                if (objectCountDown != null) objectCountDown.SetActive(true);
            }
            return;
        }

        if (countdownFirst)
        {
            objectCountDown?.SetActive(true);
            countDown -= Time.deltaTime;
            if (countDown <= 0)
            {
                countdownFirst = false;
                countDown = 5;
                if (player01 != null) player01.StartRun();
                if (player02 != null) player02.StartRun();
            }
            return;
        }

        if (audioController) audioController.audioSourceBGM.pitch = 1f;
        if (textTime != null) textTime.text = (playTime - timeCount).ToString("N0");
        if (imageTime != null) imageTime.fillAmount = (float)((playTime - timeCount) / playTime);


        if (countPlayers == players && finish == false)
        {
            finish = true;
            EndGame();
            return;
        }

        if (timeCount < playTime)
        {
            timeCount += Time.deltaTime;
            bool p1Finish = player01 != null && player01.finish;
            bool p2Finish = player02 != null && player02.finish;

            if ((mode == Mode.Scenario && p1Finish && p2Finish) || (mode == Mode.EachGame && p1Finish))
            {
                timeCount = playTime;
            }
        }
        if (timeCount >= playTime && countDown == 5)
        {
            if (countPlayers < players)
            {
                NextPlayer();
                objectCountDown?.SetActive(true);
            }
            
            countDown -= Time.deltaTime;
            return;
        }
        else if (timeCount >= playTime && countDown > 0)
        {
            countDown -= Time.deltaTime;
        }
        else if (countDown <= 0)
        {
            if (countPlayers < players)
            {
                noticeTimeOut?.SetActive(false);

                if (player01 != null) player01.StartRun();
                if (player02 != null) player02.StartRun();
            }
            timeCount = 0;
            countDown = 5;
        }
    }

    void NextPlayer()
    {
        if (noticeTimeOut != null) noticeTimeOut?.SetActive(true);
        countPlayers++;
        if (player01 != null) player01.ChangePlayer();
        if (player02 != null) player02.ChangePlayer();
    }

    public void SetupGame(Mode mode = Mode.EachGame)
    {
        if (mode == Mode.EachGame)
        {
            if (player02 != null) player02.gameObject.SetActive(false);
            if (cameraPlayer01 != null) cameraPlayer01.rect = new Rect(0, 0, 1, 1);
            if (point02 != null) point02.SetActive(false);
        }
        else if (mode == Mode.Scenario)
        {
            if (player02 != null) player02.gameObject.SetActive(true);
            if (cameraPlayer01 != null) cameraPlayer01.rect = new Rect(0, 0, 0.5f, 1);
            if (cameraPlayer02 != null) cameraPlayer02.rect = new Rect(0.5f, 0, 0.5f, 1);
            if (point02 != null) point02.SetActive(true);
        }
    }

    public void SetupInput(Mode m, Difficulty d, int p, float t, bool e, bool pT)
    {
        mode = m;
        difficulty = d;
        players = p;
        playTime = t;
        explanation = e;
        photoTime = pT;
    }

    public void EndGame()
    {
        //if (nuitrack.Nuitrack.GetModule(nuitrack.nuitrack_device_api.depth_sensor) != null) nuitrack.Nuitrack.Release();
        InputManager.Instance?.SavePoint(pointTeam1, pointTeam2);
        SceneManager.LoadSceneAsync(_nextScene);
    }
}
