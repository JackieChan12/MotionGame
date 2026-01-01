using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CellController : MonoBehaviour
{
    public TMP_Text txtName;
    public Image imageGame;
    public Image imageMark;
    public string nameScene;
    public Toggle toggle;
    public Button btnShowTutorial;
    public string tutorialSingle;
    public string tutorialTeam;
    public Sprite imageTutorial;
    
    public void SetInfor(string n, string scene, Sprite sprite, Sprite bgTutorial = null)
    {
        txtName.text = n;
        nameScene = scene;
        imageGame.sprite = sprite;
        imageMark.sprite = sprite;
        imageTutorial = bgTutorial;
    }
}
