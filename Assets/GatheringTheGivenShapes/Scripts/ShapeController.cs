using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShapeController : MonoBehaviour
{
    [Header("Index player")]
    public int indexTeam;
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
    int index = 0;
    [SerializeField] float heightKnee = 0;

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
    public Image imageTrash;

    [Header("\nAudio")]
    public AudioController audioController;

    bool started = false;
    bool countdownFirst = true;

    int indexTrash;
    List<int> selectedShape = new List<int>();

    private void OnEnable()
    {
        audioController?.PlayAudioOut();
        point = 0;
        AdjustDifficulty();

        RandomShapeGame();
    }

    void RandomShapeGame()
    {
        selectedShape.Clear();
        while (selectedShape.Count < 4)
        {
            int randomIndex = Random.Range(0, shapes.Length);
            if (!selectedShape.Contains(randomIndex))
            {
                selectedShape.Add(randomIndex);
            }
        }
        List<Sprite> spriteShapes = new List<Sprite>();
        foreach (int index in selectedShape)
        {
            spriteShapes.Add(shapes[index].GetComponent<Image>().sprite);
        }
        indexTrash = selectedShape[selectedShape.Count - 1];
        if (indexTeam == 0)
        {
            FindObjectOfType<TestNuitrack>()?.SetSpriteTeam1(spriteShapes[0], spriteShapes[1], spriteShapes[2]);
        }
        else
        {
            FindObjectOfType<TestNuitrack>()?.SetSpriteTeam2(spriteShapes[0], spriteShapes[1], spriteShapes[2]);
        }
        imageTrash.sprite = spriteShapes[spriteShapes.Count - 1];
    }

    public string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    void Update()
    {
        int teams = NuitrackManager.SkeletonTracker.GetSkeletonData().Skeletons.Length;
        if (started == false && teams < 1)
        {
            return;
        }
        else
        {
            started = true;
        }
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

        //---------------------------------

        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + 1f / spawnRate;

            int randomFruit = Random.Range(0, shapes.Length);
            Vector3 randomSpawnPoint = GenerateRandomSpawnPoint();
            index++;
            Shapes fruit = Instantiate(shapes[randomFruit], randomSpawnPoint, Quaternion.identity, spawnPoint).GetComponent<Shapes>();
            fruit.minSpeed = minSpeedAD;
            fruit.maxSpeed = maxSpeedAD;
            fruit.heightKnee = heightKnee;
            if(randomFruit == indexTrash)
            {
                fruit.point = -1;
            }
            else if(randomFruit == selectedShape[0])
            {
                fruit.type = typeJointShape.head;
            }
            else if (randomFruit == selectedShape[1])
            {
                fruit.type = typeJointShape.wirst;
            }
            else if (randomFruit == selectedShape[2])
            {
                fruit.type = typeJointShape.ankle;
            }

            //fruit.textPoint.text = (randomFruit + 1).ToString();
            fruit.choosen = AddPoint;
        }
    }

    void EndGame()
    {
        if(indexTeam == 0)
        {
            gameContent.pointTeam1 += point;
        }
        else if(indexTeam == 1)
        {
            gameContent.pointTeam2 += point;
        }
        gameContent.EndGame();
    }
    void NextPlayer()
    {
        if(noticeTimeOut != null) noticeTimeOut?.SetActive(true);
        countPlayers++;
        for (int i = 2; i < spawnPoint.childCount; i++)
        {
            spawnPoint.GetChild(i).GetComponent<Shapes>().Hide();
        }
    }

    public void AddPoint(int pTeam1, int pTeam2 = 0)
    {
        audioController?.PlaySplat();
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
        //Fruit[] shapes = FindObjectsOfType<Fruit>();
        //foreach (Fruit fruit in shapes)
        //{
        //    fruit.minSpeed = minSpeed;
        //    fruit.maxSpeed = maxSpeed;
        //}

        minSpeedAD = minSpeed;
        maxSpeedAD = maxSpeed;
    }
}
