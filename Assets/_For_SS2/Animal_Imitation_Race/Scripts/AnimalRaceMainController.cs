using nuitrack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AnimalRaceMainController : MonoBehaviour
{
    [Header("Player Control")]
    public AnimalRace_Movement player01;
    public AnimalRace_Movement player02;
    public AnimalRace_Movement player03;

    //public Camera cameraPlayer01;
    //public Camera cameraPlayer02;

    public GameObject point01;
    public GameObject point02;
    public GameObject point03;

    Rect r;
    List<Skeleton> newSkeleton = new List<Skeleton>();
    public List<AnimalRace_Movement> animals;

    [Header("Game")]

    public float timeCount = 0;
    public float countDown = 5;
    bool countdownFirst = true;
    public TMP_Text textPoint01;
    public TMP_Text textPoint02;
    public TMP_Text textPoint03;
    public TMP_Text textTime;
    public Image imageTime;
    public GameObject objectCountDown;
    public GameObject noticeTimeOut;
    public AudioController audioController;
    public string _nextScene = "OutputScene";
    int countPlayers = 0;
    public TMP_Text txtCountPlayer;

    [Header("Input")]
    public Mode mode;
    public Difficulty difficulty;
    public int players;
    public float playTime;
    public bool explanation;
    public bool photoTime;

    bool finish = false;
    bool isPlay = false;

    [Header("Output")]
    public float pointTeam1 = 0;
    public float pointTeam2 = 0;
    public int count =1 ;

    [System.Obsolete]
    void Start()
    {
        NuitrackManager.SkeletonTracker.SetNumActiveUsers(3);
        if (InputManager.Instance != null) SetupInput(InputManager.Instance.mode, InputManager.Instance.difficulty, InputManager.Instance.players, InputManager.Instance.playTime, InputManager.Instance.explanation, InputManager.Instance.photoTime);
        player01.RandomAnimal(difficulty);
        player02.RandomAnimal(difficulty);
        player03.RandomAnimal(difficulty);
    }

    //// Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if (NuitrackManager.SkeletonTracker != null)
        {
            List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();
            //userData = FilterSkeleton(userData);
            ShowPlayer(userData);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            nuitrack.Nuitrack.Release();
            SceneManager.LoadSceneAsync("InputSceneSS2");
            //Application.Quit(); 
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
                player01.startGame = true;
                player02.startGame = true;
                player03.startGame = true;
            }
            return;
        }
        if (textTime != null) textTime.text = (playTime - timeCount).ToString("N0");//FormatTime(timeCount);
        if (imageTime != null) imageTime.fillAmount = (float)((playTime - timeCount) / playTime);

        if (timeCount < playTime)
        {
            timeCount += Time.deltaTime;
        }
        if (timeCount >= playTime && countDown == 5)
        {
          
                countPlayers++;
        }
        if (timeCount >= playTime && countDown > 0)
        {
            objectCountDown?.SetActive(true);
            countDown -= Time.deltaTime;
            noticeTimeOut?.SetActive(true);
            player01.startGame = false;
            player02.startGame = false;
            player03.startGame = false;
        }

        else if (countDown <= 0)
        {
            EndGame();
            
        }
    }

    [System.Obsolete]


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
    float minMainZ = 2.5f, maxMainZ = 3.5f;
    public List<Skeleton> FilterSkeleton(List<Skeleton> user)
    {
        
        newSkeleton.Clear();
        foreach (Skeleton s in user)
        {
            float z = s.GetJoint(JointType.Torso).Real.Z / 1000;
            if (z >= minMainZ && z <= maxMainZ)
            {
                newSkeleton.Add(s);
            }
        }

        return newSkeleton;
    }

    public void ShowPlayer(List<Skeleton> user)
    {
        int c = user.Count;
        //c = count;
        txtCountPlayer.text = $"{c} players";
        if (c >= 3)
        {
            animals = animals.OrderByDescending(a => a.xPlayer).ToList();
            player01.gameObject.SetActive(true);
            player02.gameObject.SetActive(true);
            player03.gameObject.SetActive(true);

            point01.SetActive(true);
            point02.SetActive(true);
            point03.SetActive(true);

            r.xMin = 0;
            r.yMin = 0;
            r.width = .33f;
            r.height = 1;

            animals[0].cam.rect = r;
            animals[0].textPoint = textPoint01;

            r.xMin = 0.335f;
            r.yMin = 0;
            r.width = .33f;
            r.height = 1;

            animals[1].cam.rect = r;
            animals[1].textPoint = textPoint02;

            r.xMin = 0.67f;
            r.yMin = 0;
            r.width = .33f;
            r.height = 1;

            animals[2].cam.rect = r;
            animals[2].textPoint = textPoint03;
        }
        else if (c >= 2)
        {
            player01.gameObject.SetActive(true);
            player02.gameObject.SetActive(true);
            player03.gameObject.SetActive(false);

            point01.SetActive(true);
            point02.SetActive(true);
            point03.SetActive(false);

            r.xMin = 0;
            r.yMin = 0;
            r.width = .495f;
            r.height = 1;

            if (player01.xPlayer < player02.xPlayer) { player01.cam.rect = r; player01.textPoint = textPoint01; }
            else { player02.cam.rect = r; player02.textPoint = textPoint01; }

            r.xMin = .505f;
            r.yMin = 0;
            r.width = .495f;
            r.height = 1;

            if (player02.xPlayer < player01.xPlayer) {player02.cam.rect = r; player02.textPoint= textPoint02; }
            else { player01.cam.rect = r; player01.textPoint = textPoint02; }
        }
        else if (c >= 1)
        {
            player01.gameObject.SetActive(true);
            player02.gameObject.SetActive(false);
            player03.gameObject.SetActive(false);


            point01.SetActive(true);
            point02.SetActive(false);
            point03.SetActive(false) ;

            r.xMin = 0;
            r.yMin = 0;
            r.width = 1;
            r.height = 1;

            player01.cam.rect = r;
            player01.textPoint= textPoint01;
        }
        else if (c ==0)
        {
            player01.gameObject.SetActive(true);
            player02.gameObject.SetActive(false);
            player03.gameObject.SetActive(false);
            point01.SetActive(true);
            point02.SetActive(false);
            point03.SetActive(false);

            r.xMin = 0;
            r.yMin = 0;
            r.width = 1;
            r.height = 1;

            player01.cam.rect = r;
            player01.textPoint = textPoint01;
        }
    }
}
