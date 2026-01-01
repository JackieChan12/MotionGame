using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RythmDanceController : MonoBehaviour
{

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

    [Header("Game")]

    public float timeCount = 0;
    public float countDown = 5;
    bool countdownFirst = true;
    public TMP_Text textPoint01;
    public TMP_Text textPoint02;
    public TMP_Text textTime;
    public Image imageTime;
    public GameObject objectCountDown;
    public GameObject objectCountTime;
    public Transform posCountDownATeam;
    public GameObject noticeTimeOut;
    public AudioController audioController;
    public string _nextScene = "OutputScene";
    int countPlayers = 0;

    [Header("Player")]
    public RhombController player01;
    public RhombController player02;
    public List<AudioClip> listAudioEasy;
    public List<AudioClip> listAudioNormal;
    public List<AudioClip> listAudioHard;
    public GameObject objMid;
    public GuideRythmDance guide;


    void Start()
    {
        if (InputManager.Instance != null) SetupInput(InputManager.Instance.mode, InputManager.Instance.difficulty, InputManager.Instance.players, InputManager.Instance.playTime, InputManager.Instance.explanation, InputManager.Instance.photoTime);
        SetupGame();
        if(PlayerPrefs.GetInt("HintRythmDance",0) == 0)
        {
            guide.gameObject.SetActive(true);
            PlayerPrefs.SetInt("HintRythmDance", 1);
        }
        else
        {
            guide.gameObject.SetActive(false);
        }
    }

    [System.Obsolete]
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            nuitrack.Nuitrack.Release();
            SceneManager.LoadSceneAsync("InputScene");
            //Application.Quit(); 
        }

        if (guide.gameObject.active) return;

        pointTeam1 = player01.point;
        pointTeam2 = player02.point;
        textPoint01.text = pointTeam1.ToString();
        textPoint02.text = pointTeam2.ToString();

        if (countdownFirst)
        {
            objectCountDown?.SetActive(true);
            countDown -= Time.deltaTime;
            if (countDown <= 0)
            {
                //audioController?.PlayAudioStartGame();
                countdownFirst = false;
                countDown = 5;
                player01.StartGame();
                player02.StartGame();
            }
            return;
        }

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

                player01.StartGame();
                player02.StartGame();
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
        audioController.audioSourceBGM.clip = RandomAudio();
        audioController.audioSourceBGM.Play();
        player01.ResetGame();
        player02.ResetGame();
    }

    public void EndGame()
    {
        nuitrack.Nuitrack.Release();
        InputManager.Instance?.SavePoint(pointTeam1, pointTeam2);
        SceneManager.LoadSceneAsync(_nextScene);
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

    public void SetupGame()
    {
        if (mode == Mode.EachGame)
        {
            player01.difficulty = difficulty;
            player01.audioSource = audioController.audioSourceBGM;
            player01.audioController = audioController;
            player02.gameObject.SetActive(false);
            noticeTimeOut.transform.localPosition = posCountDownATeam.localPosition;
            objectCountTime.transform.localPosition = posCountDownATeam.localPosition;
            objMid.SetActive(false);
            player01.player2 = null;
        }
        else
        {
            player01.difficulty = difficulty;
            player02.difficulty = difficulty;
            player01.audioSource = audioController.audioSourceBGM;
            player02.audioSource = audioController.audioSourceBGM;
            player01.audioController = audioController;
            player02.audioController = audioController;
            player01.player2 = player02;
            player02.player2 = player01;
        }
        audioController.audioSourceBGM.clip = RandomAudio();
        audioController.audioSourceBGM.Play();



    }

    AudioClip RandomAudio()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return listAudioEasy[Random.Range(0, listAudioEasy.Count)];
            case Difficulty.Normal:
                return listAudioNormal[Random.Range(0, listAudioNormal.Count)];
            case Difficulty.Hard:
                return listAudioHard[Random.Range(0, listAudioHard.Count)];
            default:
                return listAudioEasy[Random.Range(0, listAudioEasy.Count)];
        }
    }
}
