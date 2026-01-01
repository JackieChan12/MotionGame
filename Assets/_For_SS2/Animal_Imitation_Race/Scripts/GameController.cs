using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using nuitrack;
using System.Linq;

public class GameController : MonoBehaviour
{
    [Header("Player Control")]
    public AnimalRace_Movement player01;
    public AnimalRace_Movement player02;
    public AnimalRace_Movement player03;
    
    //public Camera cameraPlayer01;
    //public Camera cameraPlayer02;

    public GameObject point01;
    public GameObject point02;

    public GameObject lands01;
    public GameObject lands02;

    public RectTransform canvas;

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
    bool isPlay= false;

    [Header("Output")]
    public float pointTeam1 = 0;
    public float pointTeam2 = 0;

    [System.Obsolete]
    void Start()
    {
        if (InputManager.Instance != null) SetupInput(InputManager.Instance.mode, InputManager.Instance.difficulty, InputManager.Instance.players, InputManager.Instance.playTime, InputManager.Instance.explanation, InputManager.Instance.photoTime);
        
    }

    //// Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();
        userData = FilterSkeleton(userData);
        ShowPlayer(userData);
        
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
        List<Skeleton> newSkeleton = new List<Skeleton>();

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
        if(user.Count >= 3)
        {
            player01.gameObject.SetActive(true);
            player02.gameObject.SetActive(true);
            player03.gameObject.SetActive(true);
        }
        else if(user.Count >= 2)
        {
            player01.gameObject.SetActive(true);
            player02.gameObject.SetActive(true);
            player03.gameObject.SetActive(false);
        }
        else if (user.Count >= 1)
        {
            player01.gameObject.SetActive(true);
            player02.gameObject.SetActive(false);
            player03.gameObject.SetActive(false);
        }
    }
}
