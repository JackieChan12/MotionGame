using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;
using System.Text;

[System.Serializable]
public class InforCell
{
    public string name;
    public string nameScene;
    public Sprite sprite;
    public string tutorialSingle;
    public string tutorialTeam;
    public bool istance;
    public Sprite imageTutorial;
    public Sprite imageTutorialVN;
    public Sprite imageTutorialENG;
};

public class ListGameController : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] GridLayoutGroup grid;
    [SerializeField] RectTransform gridRect;
    [SerializeField] int col = 5;
    [SerializeField] int row = 2;
    public Vector2 spacing = new Vector2(10, 10);
    public Vector2 r = new Vector2(10, 10);
    [SerializeField] RectTransform canvas;

    [Header("\nCell Infor")]
    [SerializeField] List<InforCell> cells = new List<InforCell>();
    [SerializeField] GameObject prefabCell;
    //[SerializeField] CellController curCell;
    [SerializeField] List<UnityEngine.UI.Toggle> listToggle= new List<UnityEngine.UI.Toggle>();
    List<CellController> cellControllers = new List<CellController>();
    List<CellController> cellChoosen = new List<CellController>();

    [Header("Play")]
    [SerializeField] UnityEngine.UI.Button _play;
    [SerializeField] UnityEngine.UI.Button _quit;
    [SerializeField] InputController _input;
    [SerializeField] UnityEngine.UI.Toggle toggleRepetition;
    [SerializeField] bool repetition;
    [SerializeField] GameObject loading;


    [Header("Canvas Tutorial")]
    [SerializeField] GameObject objTutorial;
    [SerializeField] TMP_Text txtTutorialSingle;
    [SerializeField] TMP_Text txtTutorialTeam;
    [SerializeField] TMP_Text txtNameGame;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] UnityEngine.UI.Image bgImage;
    [SerializeField] TMP_Text allText;
    [SerializeField] UnityEngine.UI.Button btnChangeLanguage;
    [SerializeField] TMP_Text txtChangeLanguage;
    int statusLanguage = 0; //0 : Korean; 1: VN; 2: ENG
    int indexTutorial;

    void Start()
    {
        statusLanguage = PlayerPrefs.GetInt("indexLanguage", 0);
        spacing = grid.spacing;
        CaculateSpacing();
        ResizeCell();
        InstanceCell();
        ResizeText();
        GetAllString();
        _play.onClick.AddListener(() =>
        {
            InputManager.Instance.repetition = repetition;
            _input.SaveInfoToPlay();
            InputManager.Instance.listGameScene.Clear();
            foreach(var cell in cellChoosen)
            {
                InputManager.Instance.listGameScene.Add(cell.nameScene);
            }
            if (InputManager.Instance.listGameScene.Count > 0)
            {
                loading.SetActive(true);    
                SceneManager.LoadSceneAsync(InputManager.Instance.listGameScene[0]);
            }
        });
        _quit.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        toggleRepetition.onValueChanged.AddListener((isOn) =>
        {
            repetition = isOn;
        });
        scrollRect.verticalNormalizedPosition = 1;

        btnChangeLanguage.onClick.AddListener(() =>
        {
            statusLanguage++;
            if (statusLanguage > 2) statusLanguage = 0;
            switch (statusLanguage)
            {
                case 0:
                    txtChangeLanguage.text = "한";
                    bgImage.sprite = cells[indexTutorial].imageTutorial;
                    break;
                case 1:
                    txtChangeLanguage.text = "VN";
                    bgImage.sprite = cells[indexTutorial].imageTutorialVN;
                    break;
                case 2:
                    txtChangeLanguage.text = "EN";
                    bgImage.sprite = cells[indexTutorial].imageTutorialENG;
                    break;
            }
            PlayerPrefs.SetInt("indexLanguage", statusLanguage);
        });
    }

    void GetAllString()
    {
        foreach(var c in cells)
        {
            allText.text = allText.text+ c.tutorialSingle + c.tutorialTeam;
        }
    }

    void Update()
    {
        //StartGame();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (objTutorial.activeSelf)
            {
                objTutorial.SetActive(false);
            }
            else
            {
                Application.Quit();
            }
        }

    }

    void InstanceCell()
    {
        int i = 0;
        foreach (var cell in cells)
        {
            int index = i;
            if (cell.istance)
            {
                CellController cellController = Instantiate(prefabCell, gridRect).GetComponent<CellController>();
                cellController.SetInfor(cell.name, cell.nameScene, cell.sprite,cell.imageTutorial);
                cellController.tutorialSingle = cell.tutorialSingle;
                cellController.tutorialTeam = cell.tutorialTeam;
                CaculateTutorialButton(cellController.btnShowTutorial.GetComponent<RectTransform>());
                cellController.toggle.onValueChanged.AddListener((isOn) =>
                {
                    //HideAllCell(isOn, cellController.toggle);
                    //curCell = isOn ? cellController : null;
                    if (isOn)
                    {
                        cellChoosen.Add(cellController);
                    }
                    else
                    {
                        cellChoosen.Remove(cellController);
                    }
                });
                cellController.btnShowTutorial.onClick.AddListener(() =>
                {
                    objTutorial.SetActive(true);
                    indexTutorial = index;
                    //txtTutorial.text = $"{cellController.tutorialGame}";
                    switch (statusLanguage)
                    {
                        case 0:
                            txtChangeLanguage.text = "한";
                            bgImage.sprite = cells[indexTutorial].imageTutorial;
                            break;
                        case 1:
                            txtChangeLanguage.text = "VN";
                            bgImage.sprite = cells[indexTutorial].imageTutorialVN;
                            break;
                        case 2:
                            txtChangeLanguage.text = "EN";
                            bgImage.sprite = cells[indexTutorial].imageTutorialENG;
                            break;
                    }
                    //bgImage.sprite = cellController.imageTutorial;
                    txtTutorialSingle.text = cellController.tutorialSingle; txtTutorialTeam.text = cellController.tutorialTeam;
                    txtNameGame.text = cellController.txtName.text;
                    scrollRect.verticalNormalizedPosition = 1f;

                });
                cellControllers.Add(cellController);
                listToggle.Add(cellController.toggle);
            }
            i++;
        }
    }

    string ConvertText(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        return Encoding.UTF8.GetString(bytes);
    }

    void CaculateTutorialButton(RectTransform rect)
    {
        Vector2 sp = rect.sizeDelta;
        sp.x = sp.x / (1920 / canvas.sizeDelta.x);
        sp.y = sp.x;
        rect.sizeDelta = sp;
        sp = sp / 2;
        rect.anchoredPosition = sp;

    }

    void ResizeText() 
    {
        float f = (1080 / canvas.sizeDelta.y);
        foreach (var cell in cellControllers) 
        {
            cell.txtName.fontSize /= f;
        } 
    }

    void ResizeCell()
    {
        r = new Vector2(gridRect.rect.width, gridRect.rect.height);
        float panelWidth = gridRect.rect.width; 
        float panelHeight = gridRect.rect.height;

        float cellWidth = (panelWidth - (col - 1) * spacing.x) / col;
        float cellHeight = (panelHeight - (row - 1) * spacing.y) / row;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }



    void CaculateSpacing()
    {
        Vector2 sp = spacing;
        sp.x = sp.x / (1920 / canvas.sizeDelta.x);
        sp.y = sp.y / (1080 / canvas.sizeDelta.y);
        grid.spacing = sp;
        spacing = sp;

    }

    void HideAllCell(bool b,UnityEngine.UI.Toggle curToggle)
    {
        foreach (UnityEngine.UI.Toggle toggle in listToggle)
        {
            if (toggle != curToggle) toggle.isOn = false;
        }
        curToggle.isOn = b;
    }
}
