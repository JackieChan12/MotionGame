using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MainRobotController : MonoBehaviour
{
    [Header("Player Control")]
    public RobotGameController player01;
    public RobotGameController player02;

    public GameObject point01;
    public GameObject point02;

    public RectTransform canvas;
    public float minXP1 = 0, maxXP1 = 0;
    public float minXP2 = 0, maxXP2 = 0;

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

    


    public List<ActionController> actionControllers = new List<ActionController>();
    public float start_time = 0;
    public float end_time = 5;
    ActionController choosenActionController;
    ActionController choosenActionController_Fake;
    int indexAction;

    [Header("Input")]
    public Mode mode;
    public Difficulty difficulty;
    public int players;
    public float playTime;
    public bool explanation;
    public bool photoTime;

    bool finish = false;
    bool startAction = false;
    [Header("Output")]
    public float pointTeam1 = 0;
    public float pointTeam2 = 0;

    [SerializeField] AudioClip activeClip;
    [SerializeField] AudioClip countdownClip;
    [SerializeField] AudioClip realActionClip;
    [SerializeField] AudioClip fakeActionClip;
    [SerializeField] AudioClip comboActionClip;


    [SerializeField] AudioSource robotAudio;
    List<AudioClip> soundsPlay = new List<AudioClip>();
    private Coroutine audioCoroutine;

    bool isFake = false;
    bool isCombo = false;

    [Header("Show Command")]
    public TMP_Text txtCommand01;
    public TMP_Text txtCommand02;
    public Image bgCommand01;
    public Image bgCommand02;


    public Transform transformCountTimeObj;
    public Transform posSingleCountTime;

    void Start()
    {
        if (InputManager.Instance != null) SetupInput(InputManager.Instance.mode, InputManager.Instance.difficulty, InputManager.Instance.players, InputManager.Instance.playTime, InputManager.Instance.explanation, InputManager.Instance.photoTime);
        
        player01.PrepareStart();
        player02.PrepareStart();

        //player01.mode = mode;
        //player02.mode = mode;
        //DivideCanvas();

        SetupGame(mode);
    }

    IEnumerator PlaySequentialClips()
    {
        foreach (AudioClip clip in soundsPlay)
        {
            robotAudio.clip = clip;
            robotAudio.Play();
            yield return new WaitForSeconds(clip.length);
        }
    }

    //// Update is called once per frame
    [System.Obsolete]
    void Update()
    {


        if (startAction)
        {
            RandomAction();
            if (Time.time - start_time >= end_time && choosenActionController != null)
            {
                choosenActionController = null;
                return;
            }
            if ((mode == Mode.EachGame && player01.doneAllAction) || (mode == Mode.Scenario && player01.doneAllAction && player02.doneAllAction))
            {
                choosenActionController = null;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            nuitrack.Nuitrack.Release();
            SceneManager.LoadSceneAsync("InputScene");
            //Application.Quit(); 
        }
        pointTeam1 = player01.pointDistance;
        pointTeam2 = player02.pointDistance;
        textPoint01.text = pointTeam1.ToString("F0");
        textPoint02.text = pointTeam2.ToString("F0");

        if (countdownFirst)
        {
            if (countDown == 10)
            {
                if(audioCoroutine != null) StopCoroutine(audioCoroutine);
                soundsPlay.Clear();
                soundsPlay.Add(activeClip);
                soundsPlay.Add(countdownClip);
                audioCoroutine = StartCoroutine(PlaySequentialClips());
            }
            objectCountDown?.SetActive(true);
            countDown -= Time.deltaTime;
            
            if (countDown <= 0)
            {
                //audioController?.PlayAudioStartGame();
                countdownFirst = false;
                countDown = 5;
                player01.StartRun();
                player02.StartRun();
                startAction = true;
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

                player01.StartRun();
                player02.StartRun();
                startAction = true;
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
        startAction = false;

        if (audioCoroutine != null) StopCoroutine(audioCoroutine);
        soundsPlay.Clear();
        soundsPlay.Add(activeClip);
        soundsPlay.Add(countdownClip);
        audioCoroutine = StartCoroutine(PlaySequentialClips());
    }

    [Obsolete]
    void RandomAction()
    {
        if (choosenActionController != null) return;
        else
        {
            indexAction = UnityEngine.Random.RandomRange(0, actionControllers.Count);
            choosenActionController = actionControllers[indexAction];

            indexAction = RandomExcluding(0, actionControllers.Count, indexAction); 
            choosenActionController_Fake = actionControllers[indexAction];
            start_time = Time.time;

            isFake = difficulty == Difficulty.Easy ? false : UnityEngine.Random.RandomRange(0, 2) == 1;
            isCombo = difficulty == Difficulty.Easy || difficulty == Difficulty.Normal ? false : UnityEngine.Random.RandomRange(0, 3) == 1;
            if (isCombo) isFake = false;

            

            player01.timeStartAction = start_time; 
            player02.timeStartAction = start_time;

            player01.doneAllAction = false;
            player01.doneFirstAction = false;

            player02.doneAllAction = false;
            player02.doneFirstAction = false;

            player01.isCombo = isCombo;
            player02.isCombo = isCombo;

            ShowCommand();

            player01.choosenActionController = choosenActionController;
            player02.choosenActionController = choosenActionController;
            player01.choosenActionController_Fake = choosenActionController_Fake;
            player02.choosenActionController_Fake = choosenActionController_Fake;
        }
    }

    void ShowCommand()
    {
        if (audioCoroutine != null) StopCoroutine(audioCoroutine);
        soundsPlay.Clear();
        if (isCombo)
        {
            soundsPlay.Add(comboActionClip);
            soundsPlay.Add(choosenActionController.soundNameAction);
            soundsPlay.Add(choosenActionController_Fake.soundNameAction);
            audioCoroutine = StartCoroutine(PlaySequentialClips());

            txtCommand01.text = choosenActionController.strNameAction;
            txtCommand02.text = choosenActionController_Fake.strNameAction;
            bgCommand02.gameObject.SetActive(true);
            bgCommand01.gameObject.SetActive(true);
        }
        else if (isFake)
        {
            bgCommand01.gameObject.SetActive(true);
            StartCoroutine(ShowFakeCommad());
            bgCommand02.gameObject.SetActive(false);
        }
        else
        {
            soundsPlay.Add(realActionClip);
            soundsPlay.Add(choosenActionController.soundNameAction);
            audioCoroutine = StartCoroutine(PlaySequentialClips());

            txtCommand01.text = choosenActionController.strNameAction;

            bgCommand01.gameObject.SetActive(true);
            bgCommand02.gameObject.SetActive(false);
        }
    }

    IEnumerator ShowFakeCommad()
    {
        txtCommand01.text = choosenActionController_Fake.strNameAction;   
        soundsPlay.Clear();
        soundsPlay.Add(fakeActionClip);
        soundsPlay.Add(choosenActionController_Fake.soundNameAction);
        audioCoroutine = StartCoroutine(PlaySequentialClips());

        yield return new WaitForSeconds(3);

        txtCommand01.text = choosenActionController.strNameAction;
        if (audioCoroutine != null) StopCoroutine(audioCoroutine);
        soundsPlay.Clear();
        soundsPlay.Add(realActionClip);
        soundsPlay.Add(choosenActionController.soundNameAction);
        audioCoroutine = StartCoroutine(PlaySequentialClips());
    }

    int RandomExcluding(int min, int max, int excluded)
    {
        int result;
        do
        {
            result = UnityEngine.Random.Range(min, max);
        } while (result == excluded);
        return result;
    }

    public void SetupGame(Mode mode = Mode.EachGame)
    {
        if (mode == Mode.EachGame)
        {
            player02.gameObject.SetActive(false);
            point02.SetActive(false);

            if (transformCountTimeObj != null) transformCountTimeObj.position = posSingleCountTime.position;
        }
        else if (mode == Mode.Scenario)
        {
            player02.gameObject.SetActive(true);
            point02.SetActive(true);
        }

        if(difficulty == Difficulty.Easy)
        {
            end_time = 10;
        }
        else
        {
            end_time = 7;
        }
        player01.timeA_Action = end_time;
        player02.timeA_Action = end_time;
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
