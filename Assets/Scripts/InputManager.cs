using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("Input")]
    public Mode mode;
    public Difficulty difficulty;
    public int players;
    public float playTime;
    public bool explanation;
    public bool photoTime;
    public bool repetition;
    public List<string> listGameScene = new List<string>();
    public int curGameScene = 0;
    public string inputScene = "InputScene";
    [Header("Output")]
    public float pointTeam1 = 0;
    public float pointTeam2 = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ChangeInput(Mode m, Difficulty d, int p, float t, bool e, bool pT)
    {
        mode = m;
        difficulty = d;
        players = p;
        playTime = t;
        explanation = e;
        photoTime = pT;
        pointTeam1 = 0;
        pointTeam2 = 0;
    }

    public void SavePoint(float p1, float p2)
    {
        pointTeam1 += p1;
        pointTeam2 += p2;
    }
}
