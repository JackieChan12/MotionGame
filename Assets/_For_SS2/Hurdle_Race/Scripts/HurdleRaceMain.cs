using DG.Tweening;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HurdleRaceMain : MonoBehaviour
{
    [Header("Player Control")]
    public HurdleRaceNew player01;
    public HurdleRaceNew player02;

    public Camera cameraPlayer01;
    public Camera cameraPlayer02;
    public GameObject point01;
    public GameObject point02;

    public List<GameObject> listTrap = new List<GameObject>();

    [Header("Game")]

    public float timeCount = 0;
    public float countDown = 5;
    bool countdownFirst = true;

    [Header("Instruction")]
    public TMP_Text textInstruction;
    public GameObject objInstruction;
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

    [System.Obsolete]
    void Start()
    {
        if (InputManager.Instance != null) SetupInput(InputManager.Instance.mode, InputManager.Instance.difficulty, InputManager.Instance.players, InputManager.Instance.playTime, InputManager.Instance.explanation, InputManager.Instance.photoTime);
        //textTime.text = "0";
        if(audioController) audioController.audioSourceBGM.pitch = 1.49f;
        foreach (GameObject trap in listTrap) trap.SetActive(false);
        listTrap[(int)difficulty].SetActive(true);
        SetupGame(mode);

    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            nuitrack.Nuitrack.Release();
            SceneManager.LoadSceneAsync("InputScene");
            //Application.Quit(); 
        }
        pointTeam1 = player01.pointDistance;
        pointTeam2 = player02.pointDistance;
        textPoint01.text = pointTeam1.ToString("F0") + "M";
        textPoint02.text = pointTeam2.ToString("F0") + "M";

        if (isShowingInstruction)
        {
            if (textInstruction != null && !objInstruction.activeSelf)
                objInstruction.SetActive(true);
            if (objectCountDown != null && objectCountDown.activeSelf)
                objectCountDown.SetActive(false);
            textInstruction.text = Command.Instance.CommmandTutorialHurdleRace;
            instructionTime -= Time.deltaTime;
            if (instructionTime <= 0)
            {
                isShowingInstruction = false;
                if (textInstruction != null) objInstruction.SetActive(false);
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
                //audioController?.PlayAudioStartGame();
                countdownFirst = false;
                countDown = 5;
                player01.turnPlayer = true;
                player02.turnPlayer = true;
                player01.StartRun();
                player02.StartRun();
            }
            return;
        }

        if (audioController) audioController.audioSourceBGM.pitch = 1f;
        if (textTime != null) textTime.text = (playTime - timeCount).ToString("N0");//FormatTime(timeCount);
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
            if ((mode == Mode.Scenario && player01.finish && player02.finish) || (mode == Mode.EachGame && player01.finish))
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
            else
            {

                //audioController?.PlayAudioOut();
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

                player01.turnPlayer = true;
                player02.turnPlayer = true;
                player01.StartRun();
                player02.StartRun();
            }
            timeCount = 0;
            countDown = 5;
        }
    }

    [System.Obsolete]
    void NextPlayer()
    {
        if (noticeTimeOut != null) noticeTimeOut?.SetActive(true);
        countPlayers++;
        player01.ChangePlayer();
        player02.ChangePlayer();
    }


    public void SetupGame(Mode mode = Mode.EachGame)
    {
        if (mode == Mode.EachGame)
        {
            player02.gameObject.SetActive(false);
            cameraPlayer01.rect = new Rect(0, 0, 1, 1);
            point02.SetActive(false);
            //lands02.SetActive(false);
        }
        else if (mode == Mode.Scenario)
        {
            player02.gameObject.SetActive(true);
            cameraPlayer01.rect = new Rect(0, 0, 0.5f, 1);
            cameraPlayer02.rect = new Rect(0.5f, 0, 0.5f, 1);
            point02.SetActive(true);
            //lands02.SetActive(true);
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
        nuitrack.Nuitrack.Release();
        InputManager.Instance?.SavePoint(pointTeam1, pointTeam2);
        SceneManager.LoadSceneAsync(_nextScene);
    }
}
