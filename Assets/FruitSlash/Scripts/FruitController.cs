using System.Threading;
using UnityEngine;

using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Collections;
public class FruitController : MonoBehaviour
{
    [Header("Index player")]
    public int indexTeam;
    [Header("\nInfomation Game")]
    public GameObject[] fruits;
    public GameObject prefabBomb;
    public Transform spawnPoint;
    public Transform spawnPointL;
    public Transform spawnPointR;
    public float spawnRate = 1f;
    private float nextSpawn = 0f;
    public MainGameContent gameContent;
    public float minSpeedAD;
    public float maxSpeedAD;
    public int point = 0;
    int index = 0;
    [SerializeField]float heightKnee = 0;

    [Header("\nTime")]
    public int countPlayers = 0;
    public float timeCount = 0;
    public float countDown = 5;

    [Header("\nUI")]
    public TMP_Text textPoint;
    public TMP_Text textTime;
    public Image imageTime;
    public GameObject objectCountDown;
    public GameObject noticeTimeOut;
    public ControllParticleSystem controllParticle;

    [Header("\nAudio")]
    public AudioController audioController;

    bool started = false;
    bool countdownFirst = true;
    bool finishGame = false;

    private void OnEnable()
    {
        audioController?.PlayAudioOut();
        point = 0;
        AdjustDifficulty();
        if(controllParticle == null)
        {
            controllParticle = FindObjectOfType<ControllParticleSystem>();

        }
    }

    public string FormatTime(float timeInSeconds) 
    { 
        int minutes = (int)(timeInSeconds / 60); 
        int seconds = (int)(timeInSeconds % 60); 
        return minutes.ToString("00") + ":" + seconds.ToString("00"); 
    }

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
        if(started && countdownFirst)
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
        if(textTime != null) textTime.text = (gameContent.playTime - timeCount).ToString("N0");//FormatTime(timeCount);
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
            if (countPlayers < gameContent.players) { 
                objectCountDown?.SetActive(true);
            }
            else
            {
                audioController.PlayAudioOut();
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
                if (noticeTimeOut != null) noticeTimeOut.SetActive(false);
            }
            timeCount = 0;
            countDown = 5;
        }

        //---------------------------------

        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + 1f / spawnRate;


            Vector3 randomSpawnPoint = GenerateRandomSpawnPoint();
            int randomFruit = Random.Range(0, fruits.Length);
            Fruit fruit = Instantiate(fruits[randomFruit], randomSpawnPoint, Quaternion.identity, spawnPoint).GetComponent<Fruit>();
            fruit.canvas = audioController.GetComponent<RectTransform>();
            randomSpawnPoint = fruit.transform.localPosition;
            randomSpawnPoint.z = 0;
            fruit.transform.localPosition = randomSpawnPoint;
            fruit.minSpeed = minSpeedAD;
            fruit.maxSpeed = maxSpeedAD;
            fruit.heightKnee = heightKnee;
            fruit.pointFruits = randomFruit+1;
            fruit.textPoint.text = (randomFruit + 1).ToString();

            fruit.choosen = AddPoint;

            //if (RandomBomb())
            //{
            //    randomSpawnPoint = GenerateRandomSpawnPoint();
            //    Fruit bomb = Instantiate(prefabBomb, randomSpawnPoint, Quaternion.identity, spawnPoint).GetComponent<Fruit>();
            //    bomb.canvas = audioController.GetComponent<RectTransform>();
            //    bomb.minSpeed = minSpeedAD;
            //    bomb.maxSpeed = maxSpeedAD;
            //    bomb.heightKnee = heightKnee;
            //    bomb.choosen = AddPoint;
            //}
        }
    }

    bool RandomBomb()
    {
        float randomPoint = Random.Range(0, 30);

        return randomPoint >= 30 || randomPoint<=3f;
    }

    void EndGame()
    {
        //if (finishGame)
        //{
        //    return;
        //}
        //if (indexTeam == 0)
        //{
        //    gameContent.pointTeam1 += point;
        //}
        //else if (indexTeam == 1)
        //{
        //    gameContent.pointTeam2 += point;
        //}
        //gameContent.EndGame();
        //finishGame = true;
        if (finishGame)
        {
            return;
        }
        if (indexTeam == 0)
        {
            Debug.Log("Join Team 1 " + gameContent.updatePoint1.ToString());
            if (!gameContent.updatePoint1) gameContent.pointTeam1 += point;
            gameContent.updatePoint1 = true;
            if(gameContent.mode == Mode.EachGame)
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
        if (gameContent.updatePoint1 && gameContent.updatePoint2)
        {
            gameContent.EndGame();
            finishGame = true;
        }
    }
    void NextPlayer()
    {
        if(noticeTimeOut !=null) noticeTimeOut.SetActive(true);
        countPlayers++;
        for(int i=2; i<spawnPoint.childCount; i++)
        {
            spawnPoint.GetChild(i).GetComponent<Fruit>().Hide();
        }
    }

    public void AddPoint(int pTeam1, Vector3 pos, Sprite s)
    {
        audioController.PlaySplat();
        ControllParticleSystem c = Instantiate(controllParticle.gameObject, pos, Quaternion.identity).GetComponent<ControllParticleSystem>();
        c.ChangeImageInShape(s);
        point += pTeam1;
    }

    Vector3 GenerateRandomSpawnPoint()
    {
        float randomX = Random.Range(spawnPointL.position.x, spawnPointR.position.x);
        float randomY = spawnPoint.position.y;
        return new Vector3(randomX, randomY, 0);
    }

    void AdjustDifficulty()
    {
        switch (gameContent.difficulty)
        {
            case Difficulty.Easy:
                spawnRate = 0.5f;
                AdjustFruitSpeed(8,10);
                break;
            case Difficulty.Normal:
                spawnRate = 1f;
                AdjustFruitSpeed(5,7);
                break;
            case Difficulty.Hard:
                spawnRate = 1.5f;
                AdjustFruitSpeed(3,5);
                break;
        }
    }

    void AdjustFruitSpeed(float minSpeed, float maxSpeed)
    {
        //Fruit[] fruits = FindObjectsOfType<Fruit>();
        //foreach (Fruit fruit in fruits)
        //{
        //    fruit.minSpeed = minSpeed;
        //    fruit.maxSpeed = maxSpeed;
        //}

        minSpeedAD = minSpeed;
        maxSpeedAD = maxSpeed;
    }
}
