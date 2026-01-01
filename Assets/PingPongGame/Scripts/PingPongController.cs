using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PingPongController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject prefab_PlayerP1;
    [SerializeField] GameObject prefab_PlayerP2;
    [SerializeField] GameObject prefab_AIPlayer;
    [SerializeField] GameObject prefab_BoardNumber;
    [SerializeField] GameObject prefab_Ball;


    [Header("Input")]
    public Mode mode;
    public Difficulty difficulty;
    public int players;
    public float playTime;
    public bool explanation;
    public bool photoTime;


    [Header("")]
    public BallMovement ballMovement1;
    public BallMovement ballMovement2;
    public BoardNumberController boardNumberController1;
    public BoardNumberController boardNumberController2;
    public AIPaddleController aiPaddleController1;
    public AIPaddleController aiPaddleController2;
    public RacketController racketController1;
    public RacketController racketController2;
    public Table table1;
    public Table table2;

    public float point01;
    public float point02;

    [Header("UI")]

    public GameObject objPoint2;
    public float timeCount = 0;
    public float countDown = 5;
    bool countdownFirst = true;
    public TMP_Text textPoint01;
    public TMP_Text textPoint02;
    public TMP_Text textTime;
    public Image imageTime;
    public GameObject objectCountDown;
    public RectTransform rectTransformCountDown;
    public RectTransform rectTransformNewCountDown;
    public GameObject noticeTimeOut;
    public AudioController audioController;
    public string _nextScene = "OutputScene";
    int countPlayers = 0;

    bool finish = false;

    void Start()
    {
        if (InputManager.Instance != null) SetupInput(InputManager.Instance.mode, InputManager.Instance.difficulty, InputManager.Instance.players, InputManager.Instance.playTime, InputManager.Instance.explanation, InputManager.Instance.photoTime);
        SetupGame();
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
        //point01 = player01.pointDistance;
        //point02 = player02.pointDistance;
        textPoint01.text = "Point: " + point01.ToString("F0");
        textPoint02.text = "Point: " + point02.ToString("F0");

        if (countdownFirst)
        {
            objectCountDown?.SetActive(true);
            countDown -= Time.deltaTime;
            if (countDown <= 0)
            {
                //audioController?.PlayAudioStartGame();
                countdownFirst = false;
                countDown = 5;
                ballMovement1.StartBall();
                if(mode == Mode.Scenario && difficulty == Difficulty.Easy) ballMovement2.StartBall();
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

                ballMovement1.StartBall();
                if (mode == Mode.Scenario && difficulty == Difficulty.Easy) ballMovement2.StartBall();

            }
            timeCount = 0;
            countDown = 5;
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

    public void SetupGame()
    {
        point01 = 0;
        point02 = 0;
        if (mode == Mode.EachGame)
        {
            rectTransformCountDown.localPosition = rectTransformNewCountDown.localPosition;
            objPoint2.SetActive(false);
            racketController1 = Instantiate(prefab_PlayerP1, table1.spawnPoint_P1.position, table1.spawnPoint_P1.rotation).GetComponent<RacketController>();
            racketController1.gameObject.SetActive(true);
            racketController1.mode = mode;
            racketController1.PrepareStart();
            ballMovement1 = Instantiate(prefab_Ball, table1.spawnBall.position, table1.spawnBall.rotation).GetComponent<BallMovement>();
            ballMovement1.gameObject.SetActive(true);
            ballMovement1.isTable0 = true;
            ballMovement1.indexBall = 0;
            ballMovement1.difficulty = difficulty;

            if (difficulty == Difficulty.Easy)
            {
                ballMovement1.speed = 4f;
                boardNumberController1 = Instantiate(prefab_BoardNumber, table1.spawnPoint_P2.position, table1.spawnPoint_P2.rotation).GetComponent<BoardNumberController>();
                boardNumberController1.gameObject.SetActive(true);
                ballMovement1.onNumber += OnBoard;
            }
            else
            {
                ballMovement1.speed = difficulty == Difficulty.Normal ? 5 : 7;
                aiPaddleController1 = Instantiate(prefab_AIPlayer, table1.spawnPoint_P2.position, table1.spawnPoint_P2.rotation).GetComponent<AIPaddleController>();
                aiPaddleController1.gameObject.SetActive(true);
                aiPaddleController1.ball = ballMovement1.transform;
                ballMovement1.onGoal += OnGoal;
            }
        }
        else if (mode == Mode.Scenario)
        {
            racketController1 = Instantiate(prefab_PlayerP1, table1.spawnPoint_P1.position, table1.spawnPoint_P1.rotation).GetComponent<RacketController>();
            racketController2 = Instantiate(prefab_PlayerP2, table2.spawnPoint_P2.position, table2.spawnPoint_P2.rotation).GetComponent<RacketController>();
            racketController1.gameObject.SetActive(true);
            racketController2.gameObject.SetActive(true);
            racketController1.mode = mode;
            racketController2.mode = mode;
            racketController1.PrepareStart();
            racketController2.PrepareStart();

            ballMovement1 = Instantiate(prefab_Ball, table1.spawnBall.position, table1.spawnBall.rotation).GetComponent<BallMovement>();
            ballMovement1.gameObject.SetActive(true);
            ballMovement1.indexBall = 0;
            ballMovement1.difficulty = difficulty;

            if (difficulty == Difficulty.Easy)
            {
                ballMovement2 = Instantiate(prefab_Ball, table2.spawnBall.position, table2.spawnBall.rotation).GetComponent<BallMovement>();
                ballMovement2.gameObject.SetActive(true);
                ballMovement2.indexBall = 1;
                ballMovement2.difficulty = difficulty;
                ballMovement1.speed = 4f;
                ballMovement2.speed = 4f;
                boardNumberController1 = Instantiate(prefab_BoardNumber, table1.spawnPoint_P2.position, table1.spawnPoint_P2.rotation).GetComponent<BoardNumberController>();
                boardNumberController1.gameObject.SetActive(true);
                boardNumberController2 = Instantiate(prefab_BoardNumber, table2.spawnPoint_P1.position, table2.spawnPoint_P1.rotation).GetComponent<BoardNumberController>();
                boardNumberController2.gameObject.SetActive(true);
                ballMovement1.onNumber += OnBoard;
                ballMovement2.onNumber += OnBoard;
            }
            else if( difficulty == Difficulty.Normal)
            {
                //ballMovement2 = Instantiate(prefab_Ball, table2.spawnBall.position, table2.spawnBall.rotation).GetComponent<BallMovement>();
                //ballMovement2.gameObject.SetActive(true);
                //ballMovement2.indexBall = 1;

                //ballMovement1.speed = 5f;
                //ballMovement2.speed = 5f;

                //aiPaddleController1 = Instantiate(prefab_AIPlayer, table1.spawnPoint_P2.position, table1.spawnPoint_P2.rotation).GetComponent<AIPaddleController>();
                //aiPaddleController1.gameObject.SetActive(true);
                //aiPaddleController1.ball = ballMovement1.transform;
                //ballMovement1.onGoal += OnGoal;

                //aiPaddleController2 = Instantiate(prefab_AIPlayer, table2.spawnPoint_P1.position, table2.spawnPoint_P1.rotation).GetComponent<AIPaddleController>();
                //aiPaddleController2.gameObject.SetActive(true);
                //aiPaddleController2.ball = ballMovement2.transform;
                //aiPaddleController2.direct = -1;
                //ballMovement2.onGoal += OnGoal;

                ballMovement1.speed = 5f;
                ballMovement1.onGoal += OnGoal;
                racketController2.transform.position = table1.spawnPoint_P2.position;
                racketController2.PrepareStart();


            }
            else if(difficulty == Difficulty.Hard)
            {
                ballMovement1.speed = 6f;
                ballMovement1.onGoal += OnGoal;
                racketController2.transform.position = table1.spawnPoint_P2.position;
                racketController2.PrepareStart();
            }
        }
    }

    private void OnDestroy()
    {
        ballMovement1.onNumber -= OnBoard;
    }

    public void OnBoard(int indexBoard, int index)
    {
        if(indexBoard == boardNumberController1.number+1 && index ==0)
        {
            point01+=1;
            //audioController.PlaySplatDone();
            //boardNumberController1.RandomNumber();
            //boardNumberController1.particle.Play();
            StartCoroutine(OnBoardAndWait(boardNumberController1));
        }
        else if (index == 1 && indexBoard == boardNumberController2.number+1 )
        {
            point02 += 1;
            //audioController.PlaySplatDone();
            //boardNumberController2.RandomNumber();
            //boardNumberController2.particle.Play();
            StartCoroutine(OnBoardAndWait(boardNumberController2));
        }
    }

    IEnumerator OnBoardAndWait(BoardNumberController b)
    {
        audioController.PlaySplatDone();
        b.particle.Play();
        yield return new WaitForSeconds(1);
        b.RandomNumber();
        b.particle.Stop();
    }

    public void OnGoal(int indexGoal, int index)
    {
        if ( index == 0)
        {
            if(indexGoal == 2)
            {
                point01 += 1;
            }
            if(indexGoal == 1)
            {
                if (mode == Mode.Scenario && difficulty != Difficulty.Easy){
                    point02 += 1;
                }
                else
                {
                    point01 -= 1;
                }
            }
        }
        else if ( index == 1)
        {
            if (indexGoal == 2)
            {
                point02 += 1;
            }
            if (indexGoal == 1)
            {
                point02 -= 1;
            }
        }
    }

    public void EndGame()
    {
        nuitrack.Nuitrack.Release();
        InputManager.Instance?.SavePoint(point01, point02);
        SceneManager.LoadSceneAsync(_nextScene);
    }

    [System.Obsolete]
    void NextPlayer()
    {
        if (noticeTimeOut != null) noticeTimeOut?.SetActive(true);
        countPlayers++;
        ballMovement1.ResetBall();
        if (mode == Mode.Scenario && difficulty == Difficulty.Easy) ballMovement2.ResetBall();
    }
}
