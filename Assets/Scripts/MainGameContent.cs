using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using nuitrack;

public enum Mode
{
    Scenario,
    EachGame
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public class MainGameContent : MonoBehaviour
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
    public float pointTeam1=0;
    public float pointTeam2=0;
    public bool updatePoint1 = false;
    public bool updatePoint2 = false;

    [Header("Scene")]
    public GameObject canvas_One_Player;
    public GameObject canvas_Two_Player;
    public string _nextScene = "OutputScene";
    public Transform transformCountTimeObj;
    public Transform posSingleCountTime;

    [Header("Nuitrack")]
    [SerializeField] TestNuitrack nuitrackController;
    [SerializeField] ShowColorFR show;

    [SerializeField] bool isTest;

    void Start()
    {
        //pointTeam1 = 0;
        //pointTeam2 = 0;
        //StartGame();
    }

    private void OnEnable()
    {
        pointTeam1 = 0;
        pointTeam2 = 0;
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        //StartGame();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            show.Stop();
            nuitrack.Nuitrack.Release();
            SceneManager.LoadSceneAsync("InputScene");
          //Application.Quit(); 
        }
        
    }

    public void EndGame()
    {
        if(finish) return;
        finish = true;
        show.Stop();
        nuitrack.Nuitrack.Release();
        InputManager.Instance.SavePoint(pointTeam1, pointTeam2);
        SceneManager.LoadSceneAsync(_nextScene);
        updatePoint1 = false;
        updatePoint2 = false;
        //endController.gameObject.SetActive(true);
    }

    //public void AddPoint(int pointT1, int pointT2 = 0)
    //{
    //    pointTeam1 += pointT1;
    //    pointTeam2 += pointT2;
    //}

    public void StartGame()
    {
        if(!isTest) SetupInput(InputManager.Instance.mode, InputManager.Instance.difficulty, InputManager.Instance.players, InputManager.Instance.playTime, InputManager.Instance.explanation, InputManager.Instance.photoTime);
        if (mode == Mode.EachGame)
        {
            canvas_One_Player.SetActive(true);
            canvas_Two_Player.SetActive(false);
            if(transformCountTimeObj != null) transformCountTimeObj.position = posSingleCountTime.position;
            nuitrackController.OffTeam2();
        }
        else
        {
            canvas_One_Player.SetActive(false);
            canvas_Two_Player.SetActive(true);
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

    
}
