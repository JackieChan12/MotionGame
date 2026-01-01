using DG.Tweening;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TightropeController : MonoBehaviour
{
    [Header("Player Control")]
    public CharacterMoveOnRope player01;
    public CharacterMoveOnRope player02;

    public Camera cameraPlayer01;
    public Camera cameraPlayer02;
    public bool team01Done = false;
    public bool team02Done = false;
    public GameObject point01;
    public GameObject point02;

    public GameObject lands01;
    public GameObject lands02;

    [Header("Tight Rope")]
    public GenPathRope rope_30s;
    public GenPathRope rope_60s;
    public GenPathRope rope_90s;
    public GenPathRope rope_120s;

    public GenPathRope rope_current;

    public List<Trap> listTrap = new List<Trap>();
    public List<GameObject> listTrapInstance = new List<GameObject>();

    [Header("Game")]

    public float timeCount = 0;
    public float countDown = 5;
    bool countdownFirst = true;
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
        audioController.audioSourceBGM.pitch = 1.49f;
        player01.haveFall = difficulty != Difficulty.Easy;
        player02.haveFall = difficulty != Difficulty.Easy;
        SetupGame(mode);
        SetPosPlayer();

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

        audioController.audioSourceBGM.pitch = 1f;
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
            if((mode == Mode.Scenario && player01.finish && player02.finish) || (mode == Mode.EachGame && player01.finish))
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
        RandomTrap();
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

    [System.Obsolete]
    public void SetPosPlayer()
    {
        switch (playTime)
        {
            case 30:
                rope_current = rope_30s;
                break;
            case 60:
                rope_current = rope_60s;
                break;
            case 90:
                rope_current = rope_90s;
                break;
            case 120:
                rope_current = rope_120s;
                break;
            case 10:
                rope_current = rope_30s;
                break;
        }
        player01.pathFollower.pathCreator = rope_current.GetComponent<PathCreator>();
        player02.pathFollower.pathCreator = rope_current.GetComponent<PathCreator>();
        player01.transform.position = rope_current.ST.position;
        player02.transform.position = rope_current.ST.position;
        player01.transform.LookAt(rope_current.END);
        player02.transform.LookAt(rope_current.END);

        RandomTrap();
    }

    [System.Obsolete]
    public void RandomTrap()
    {
        foreach(var p in listTrapInstance)
        {
            Destroy(p.gameObject); 
        }
        listTrapInstance.Clear();

        int disTrap = 1;
        switch (difficulty)
        {
            case Difficulty.Hard:
                disTrap = 2;
                break;
            case Difficulty.Normal:
                disTrap = 3;
                break;
            case Difficulty.Easy:
                disTrap = 4;
                break;
        }

        for (int i = 2; i < rope_current.waypoints.Length-1; i += disTrap)
        {
            GameObject obj = Instantiate(listTrap[Random.RandomRange(0, listTrap.Count)].gameObject, rope_current.waypoints[i].position, Quaternion.identity).gameObject;
            obj.SetActive(true);
            listTrapInstance.Add(obj);
        }
    }

    public void EndGame()
    {
        nuitrack.Nuitrack.Release();
        InputManager.Instance?.SavePoint(pointTeam1, pointTeam2);
        SceneManager.LoadSceneAsync(_nextScene);
    }
}
