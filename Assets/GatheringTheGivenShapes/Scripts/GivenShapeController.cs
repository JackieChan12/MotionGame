using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GivenShapeController : MonoBehaviour
{
    [Header("Index player")]
    public int indexTeam;
    public GivenShapeController otherPlayer;

    [Header("\nInfomation Game")]
    public GameObject[] shapes;
    public Transform spawnPoint;
    public Transform spawnPointL;
    public Transform spawnPointR;
    public float spawnRate = 1f;
    private float nextSpawn = 0f;
    public MainGameContent gameContent;
    public float minSpeedAD;
    public float maxSpeedAD;
    public int point = 0;
    public Transform outLeft;
    public Transform outRight;

    [Header("\nTime")]
    public int countPlayers = 0;
    public float timeCount = 0;
    public float countDown = 5;
    bool finishGame = false;

    [Header("\nUI")]
    public TMP_Text textPoint;
    public TMP_Text textTime;
    public Image imageTime;
    public GameObject objectCountDown;
    public GameObject noticeTimeOut;
    public Image imageTrash;

    int indexTrash;

    [Header("\nAudio")]
    public AudioController audioController;
    public RectTransform canvas;

    bool started = false;
    bool countdownFirst = true;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //int teams = NuitrackManager.SkeletonTracker.GetSkeletonData().Skeletons.Length;
        //if (started == false && teams < 1)
        //{
        //    return;
        //}
        //else
        //{
            started = true;
        //}
        if (started && countdownFirst)
        {
            objectCountDown?.SetActive(true);
            countDown -= Time.deltaTime;
            if (countDown <= 0)
            {
                FindObjectOfType<TestNuitrack>().stopCheckKnee = true;
                audioController?.PlayAudioStartGame();
                countdownFirst = false;
                countDown = 5;
            }
            return;
        }
        textPoint.text = point.ToString();
        if (textTime != null) textTime.text = (gameContent.playTime - timeCount).ToString("N0");//FormatTime(timeCount);
        if (imageTime != null) imageTime.fillAmount = (float)((gameContent.playTime - timeCount) / gameContent.playTime);

        if (countPlayers == gameContent.players)
        {
            EndGame();
            return;
        }
        if (timeCount < gameContent.playTime)
        {
            timeCount += Time.deltaTime;
        }
        if (timeCount >= gameContent.playTime && countDown == 5)
        {
            NextPlayer();
            if (countPlayers < gameContent.players)
            {
                RandomShapeGame();
                objectCountDown?.SetActive(true);
            }
            else
            {
                audioController?.PlayAudioOut();
            }
            countDown -= Time.deltaTime;
            return;
        }
        else if (timeCount >= gameContent.playTime && countDown > 0)
        {
            countDown -= Time.deltaTime;
            return;
        }
        else if (countDown <= 0)
        {
            if (countPlayers < gameContent.players)
            {
                if (noticeTimeOut != null) noticeTimeOut?.SetActive(false);
            }
            timeCount = 0;
            countDown = 5;
        }

        //-------------------------------------------------

        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + 1f / spawnRate;

            int randomFruit = UnityEngine.Random.Range(0, shapes.Length);
            InstanFigure(randomFruit);
        }
    }

    void EndGame()
    {
        if (finishGame)
        {
            return;
        }
        if (indexTeam == 0)
        {
            Debug.Log("Join Team 1 " + gameContent.updatePoint1.ToString());
            if (!gameContent.updatePoint1) gameContent.pointTeam1 += point;
            gameContent.updatePoint1 = true; 
            if (gameContent.mode == Mode.EachGame)
            {
                gameContent.updatePoint2 = true;
            }

        }
        else if (indexTeam == 1)
        {
            Debug.Log("Join Team 2 " + gameContent.updatePoint2.ToString());
            if (!gameContent.updatePoint2) gameContent.pointTeam2 += point;
            gameContent.updatePoint2 = true;
        }
        if(gameContent.updatePoint1 && gameContent.updatePoint2)
        {
            gameContent.EndGame();
            finishGame = true;
        }
    }

    private void OnEnable()
    {
        point = 0;
        AdjustDifficulty();

        RandomShapeGame();
    }

    public void InstanFigure(int randomIndex)
    {
        Vector3 randomSpawnPoint = GenerateRandomSpawnPoint();
        GivenShape sh = Instantiate(shapes[randomIndex], randomSpawnPoint, Quaternion.identity, spawnPoint).GetComponent<GivenShape>();
        randomSpawnPoint = sh.transform.localPosition;
        randomSpawnPoint.z = 0;
        sh.transform.localPosition = randomSpawnPoint;
        sh.canvas = canvas;
        sh.minSpeed = minSpeedAD;
        sh.maxSpeed = maxSpeedAD;
        sh.indexImage = randomIndex;
        sh.indexGiven = indexTrash;
        sh.choosen = ChargeFigureToOtherPlayer;
        sh.Fall();
    }

    public void InstanFigureArc(int randomIndex, Vector3 pos, bool isOut = false, Vector3 toPoint = new Vector3())
    {
        
        Vector3 randomSpawnPoint = GenerateRandomSpawnPoint();
        randomSpawnPoint.y = pos.y;
        randomSpawnPoint.z = pos.z;
        GivenShape sh = Instantiate(shapes[randomIndex], pos, Quaternion.identity, spawnPoint).GetComponent<GivenShape>();
        sh.canvas = canvas;
        sh.transform.position = pos;

        //Instantiate(shapes[randomIndex], randomSpawnPoint, Quaternion.identity, spawnPoint).GetComponent<GivenShape>();
        //EditorApplication.isPaused = true;
        sh.minSpeed = minSpeedAD;
        sh.maxSpeed = maxSpeedAD;
        sh.indexImage = randomIndex;
        sh.indexGiven = indexTrash;
        sh.choosen = ChargeFigureToOtherPlayer;
        sh.ArcMove(isOut?toPoint:randomSpawnPoint);
    }

    public void ChargeFigureToOtherPlayer(int indexShape, bool addpoint, int pointN, Vector3 pos, bool isLeft = false)
    {
        
        if (addpoint)
        {
            point += pointN;
        }
        else
        {
            audioController.PlaySplat();
            if (otherPlayer != null)
            {

                if (!isLeft) //hand touch shape in left
                {
                    if(indexTeam == 0)
                    {
                        otherPlayer.InstanFigureArc(indexShape, pos);
                    }
                    else
                    {
                        InstanFigureArc(indexShape, pos, true, outRight.position);
                    }
                }
                else
                {
                    if (indexTeam == 1)
                    {
                        otherPlayer.InstanFigureArc(indexShape, pos);
                    }
                    else
                    {

                        InstanFigureArc(indexShape, pos, true, outLeft.position);
                    }
                }
            }
            else
            {
                InstanFigureArc(indexShape, pos, true, isLeft ? outLeft.position : outRight.position);
            }
        }
    }

    void RandomShapeGame()
    {
        indexTrash = UnityEngine.Random.Range(0, shapes.Length);
        imageTrash.sprite = shapes[indexTrash].GetComponent<Image>().sprite;
    }

    void NextPlayer()
    {
        if (noticeTimeOut != null) noticeTimeOut?.SetActive(true);
        countPlayers++;
        for (int i = 2; i < spawnPoint.childCount; i++)
        {
            spawnPoint.GetChild(i).GetComponent<GivenShape>().Hide();
        }
    }

    Vector3 GenerateRandomSpawnPoint()
    {
        float randomX = UnityEngine.Random.Range(spawnPointL.position.x, spawnPointR.position.x);
        float randomY = spawnPoint.position.y;
        return new Vector3(randomX, randomY, 0);
    }


    void AdjustDifficulty()
    {
        switch (gameContent.difficulty)
        {
            case Difficulty.Easy:
                spawnRate = 0.5f;
                AdjustFruitSpeed(4, 6);
                break;
            case Difficulty.Normal:
                spawnRate = 1f;
                AdjustFruitSpeed(3, 5);
                break;
            case Difficulty.Hard:
                spawnRate = 1.5f;
                AdjustFruitSpeed(2, 4);
                break;
        }
    }
    void AdjustFruitSpeed(float minSpeed, float maxSpeed)
    {
        minSpeedAD = minSpeed;
        maxSpeedAD = maxSpeed;
    }
}
