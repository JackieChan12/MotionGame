using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InputController : MonoBehaviour
{
    [SerializeField] List<Sprite> listImageNumPlayer = new List<Sprite>();
    [SerializeField] List<Toggle> listToggleMode = new List<Toggle>();
    //[SerializeField] List<Toggle> listTogglePlayer = new List<Toggle>();
    //[SerializeField] List<Toggle> listTogglePlayTime = new List<Toggle>();
    //[SerializeField] List<Toggle> listToggleDifficulty = new List<Toggle>();
    //[SerializeField] Button btn_Play;
    [SerializeField] Button btn_IncreasePlayer;
    [SerializeField] Button btn_DecreasePlayer;
    [SerializeField] Button btn_IncreaseTime;
    [SerializeField] Button btn_DecreaseTime;
    [SerializeField] Button btn_IncreaseDif;
    [SerializeField] Button btn_DecreaseDif;

    [SerializeField] TMP_Text txtPlayers;
    [SerializeField] TMP_Text txtTime;
    [SerializeField] TMP_Text txtDifficull;
    [SerializeField] Image numPlayer;


    public Mode mode;
    public Difficulty difficulty;
    public int players;
    public float playTime;
    public bool explanation;
    public bool photoTime;

    void Start()
    {
        foreach (var toggle in listToggleMode)
        {
            toggle.isOn = false;
            toggle.onValueChanged.AddListener((isOn) => OnToggle(isOn, listToggleMode,toggle));
        }

        //foreach (var toggle in listTogglePlayer)
        //{
        //    toggle.isOn = false;
        //    toggle.onValueChanged.AddListener((isOn) => OnToggle(isOn, listTogglePlayer, toggle));
        //}
        //foreach (var toggle in listTogglePlayTime)
        //{
        //    toggle.isOn = false;
        //    toggle.onValueChanged.AddListener((isOn) => OnToggle(isOn, listTogglePlayTime, toggle));
        //}
        //foreach (var toggle in listToggleDifficulty)
        //{
        //    toggle.isOn = false;
        //    toggle.onValueChanged.AddListener((isOn) => OnToggle(isOn, listToggleDifficulty, toggle));
        //}

        listToggleMode[0].isOn = true;
        ////listTogglePlayer[0].isOn = true;
        //listToggleDifficulty[0].isOn = true;
        //listTogglePlayTime[0].isOn = true;


        //btn_Play.onClick.AddListener(() =>
        //{
            
        //    //SceneManager.LoadScene("MainFruitSlash");
        //    //gameObject.SetActive(false);
        //    //mainGameContent.StartGame();
        //});

        btn_IncreasePlayer.onClick.AddListener(() =>
        {
            if (players>=5) return;
            else
            {
                players++;
                numPlayer.sprite = listImageNumPlayer[players - 1];
                txtPlayers.text = players.ToString();
            }
        });
        btn_DecreasePlayer.onClick.AddListener(() =>
        {
            if (players <=1) return;
            else
            {
                players--;
                numPlayer.sprite = listImageNumPlayer[players - 1];
                txtPlayers.text = players.ToString();
            }
        });
        btn_IncreaseTime.onClick.AddListener(() =>
        {
            if (playTime >= 120) return;
            else
            {
                playTime += 30;
                string txt = "30s";
                if (playTime == 60) txt = "1min";
                else if (playTime == 90) txt = "1.5min";
                else if (playTime == 120) txt = "2min";
                txtTime.text = txt; 
            }
        });
        btn_DecreaseTime.onClick.AddListener(() =>
        {
            if (playTime <= 30) return;
            else
            {
                playTime-=30;
                string txt = "2min";
                if (playTime == 60) txt = "1min";
                else if (playTime == 90) txt = "1.5min";
                else if (playTime == 30) txt = "30s";
                txtTime.text = txt;
            }
        });
        btn_IncreaseDif.onClick.AddListener(() =>
        {
            if (difficulty == Difficulty.Hard) return;
            else
            {
                if (difficulty == Difficulty.Normal)
                {
                    difficulty = Difficulty.Hard;
                    txtDifficull.text = "HARD";
                }
                else if (difficulty == Difficulty.Easy)
                {
                    difficulty = Difficulty.Normal;
                    txtDifficull.text = "NORMAL";
                }
            }
        });
        btn_DecreaseDif.onClick.AddListener(() =>
        {
            if (difficulty == Difficulty.Easy) return;
            else
            {
                if (difficulty == Difficulty.Normal)
                {
                    difficulty = Difficulty.Easy;
                    txtDifficull.text = "EASY";
                }
                else if (difficulty == Difficulty.Hard)
                {
                    difficulty = Difficulty.Normal;
                    txtDifficull.text = "NORMAL";
                }
            }
        });
    }

    public void SaveInfoToPlay()
    {
        InputManager.Instance.ChangeInput(mode, difficulty, players, playTime, explanation, photoTime);
    }

    void SetEachGame(bool isEachGame)
    {
        players = 1;
        numPlayer.sprite = listImageNumPlayer[players - 1];
        txtPlayers.text = players.ToString();
    }

    void OnToggle(bool b, List<Toggle> listToggle, Toggle curToggle)
    {
        foreach (Toggle toggle in listToggle)
        {
            if(toggle != curToggle) toggle.isOn = !b;
        }
        curToggle.isOn = b;

        string text = curToggle.name;
        if (text == null) return;
        if(text == "Scenario")
        {
            mode = Mode.Scenario;
            SetEachGame(false);
        }
        else if(text == "Each game")
        {
            mode = Mode.EachGame;
            SetEachGame(true);
        }
    }
}
