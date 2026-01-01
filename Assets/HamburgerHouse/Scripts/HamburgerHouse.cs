using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HamburgerHouse : MonoBehaviour
{
    public int indexTeam;

    public List<Sprite> listImageIngredients = new List<Sprite>();
    public List<int> listIntIngredients = new List<int>();
    public List<GameObject> listBurgerPrefabs = new List<GameObject>();
    public MainGameContent mainGameContent;
    public int countPlayers =0;
    public float timeCount = 0;
    public float countDown = 5;
    public int countBurger = 0;
    public A_Burger A_Burger = null;
    public A_Burger A_BurgerTargetMove = null;
    public Gena generatePath;
    public Transform parrentBurger;
    public Transform parrentBurgerTarget;
    [SerializeField] bool started = false;
    [SerializeField] bool st = false;
    bool countdownFirst = true;
    bool finishGame = false;

    int curIndexIngredient=0;


    [Header("\nUI")]
    public TMP_Text textPoint;
    public TMP_Text textTime;
    public Image imageTime;
    public GameObject objectCountDown;
    public GameObject noticeTimeOut;

    [Header("\nAudio")]
    public AudioController audioController;

    void Start()
    {
        for (int i = 0; i < listImageIngredients.Count; i++)
        {
            listIntIngredients.Add(i);
        }
        //generatePath = FindObjectOfType<Gena>();
        //generatePath.main = this;
    }

    // Update is called once per frame
    void Update()
    {
        //int teams = NuitrackManager.SkeletonTracker.GetSkeletonData().Skeletons.Length;
        //if (started == false && st == false && teams < 1)
        //{
        //    return;
        //}
        //else
        //{
            st = true;
        //}
        if (st && countdownFirst)
        {
            objectCountDown?.SetActive(true);
            countDown -= Time.deltaTime;
            if (countDown <= 0)
            {
                audioController?.PlayAudioStartGame();
                countdownFirst = false;
                countDown = 5;
            }
            return;
        }
        textPoint.text = countBurger.ToString();
        if (textTime != null) textTime.text = (mainGameContent.playTime - timeCount).ToString("N0");//FormatTime(timeCount);
        if (imageTime != null) imageTime.fillAmount = (float)((mainGameContent.playTime - timeCount) / mainGameContent.playTime);

        if (countPlayers == mainGameContent.players)
        {
            EndGame();
            return;
        }
        if (timeCount < mainGameContent.playTime)
        {
            timeCount += Time.deltaTime;
        }
        if(timeCount >= mainGameContent.playTime && countDown == 5)
        {
            NextPlayer();
            if (countPlayers < mainGameContent.players)
            {
                objectCountDown?.SetActive(true);
            }
            else
            {
                audioController?.PlayAudioOut();
            }
            countDown -= Time.deltaTime;
            return;
        }
        else if(timeCount>= mainGameContent.playTime && countDown > 0)
        {
            countDown -= Time.deltaTime;
        }
        else if (countDown <= 0 )
        {
            if (countPlayers < mainGameContent.players)
            {
                noticeTimeOut?.SetActive(false);
            }
            timeCount = 0;
            countDown = 5;
            CreateBurger(false);
        }
        if(A_Burger == null && !started)
        {
            started = true;
            st = false;
            CreateBurger();
        }

    }

    void CreateBurger(bool first=true)
    {
        int burgerIndex = RandomBurger();
        // instance a_burger
        A_Burger = Instantiate(listBurgerPrefabs[burgerIndex], parrentBurger).GetComponent<A_Burger>();
        A_BurgerTargetMove = Instantiate(listBurgerPrefabs[burgerIndex], parrentBurgerTarget).GetComponent<A_Burger>();
        A_BurgerTargetMove.GetImage();
        curIndexIngredient = A_Burger.ingredients.Count-1;
        //generatePath.sprites = GetRandomList(listImageIngredients, A_Burger.ingredients,7);
        generatePath.listIndexSprite = GetRandomList(listIntIngredients,A_Burger.ints,7);
        generatePath.StartGame(first);
    }

    List<Sprite> GetRandomList(List<Sprite> fullList, List<Sprite> selectedItems, int resultCount) 
    { 
        List<Sprite> randomList = new List<Sprite>(selectedItems); 
        List<Sprite> availableItems = new List<Sprite>(fullList); 
        foreach (Sprite item in selectedItems) 
        { 
            availableItems.Remove(item); 
        }

        while (randomList.Count < resultCount && availableItems.Count > 0) 
        { 
            int randomIndex = Random.Range(0, availableItems.Count);
            randomList.Add(availableItems[randomIndex]); 
            availableItems.RemoveAt(randomIndex); 
        }

        Shuffle(randomList);

        return randomList; 
    }
    List<int> GetRandomList(List<int> fullList, List<int> selectedItems, int resultCount)
    {
        List<int> randomList = new List<int>(selectedItems);
        List<int> availableItems = new List<int>(fullList);
        foreach (int item in selectedItems)
        {
            availableItems.Remove(item);
        }

        while (randomList.Count < resultCount && availableItems.Count > 0)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            randomList.Add(availableItems[randomIndex]);
            availableItems.RemoveAt(randomIndex);
        }

        Shuffle(randomList);

        return randomList;
    }

    public void CheckIngredients(Ingredient ing)
    {
        //for (int i = 0; i < A_Burger.ingredients.Count; i++)
        //{
            //Debug.Log("Touch " + (A_Burger.ingredients[i] == ing.imageChoosen.sprite));
            //if (A_Burger.ingredients[i] == ing.imageChoosen.sprite && !A_Burger.choosenIngredients[i])
            //{
            //    Debug.Log("");
            //    ing.HideIngredient();
            //    A_Burger.choosenIngredients[i] = true;
            //    CheckBurgerStatus();
            //    break;
            //}
        if (A_Burger.ints[curIndexIngredient] == ing.stt && !A_Burger.choosenIngredients[curIndexIngredient])
        {
            audioController.PlaySplat();
            MoveIngredientToCreateBurger(ing.transform.position, curIndexIngredient);
            Debug.Log("Yes");
            ing.HideIngredient();
            A_Burger.choosenIngredients[curIndexIngredient] = true;
            curIndexIngredient--;
            
            //break;
        }
        //}
    }

    void MoveIngredientToCreateBurger(Vector3 posInstance, int indexIngredient)
    {
        A_BurgerTargetMove.images[indexIngredient].enabled = true;
        Vector3 toPos = A_BurgerTargetMove.images[indexIngredient].gameObject.transform.position;
        GameObject ingred = Instantiate(A_BurgerTargetMove.images[indexIngredient].gameObject, posInstance, Quaternion.identity, A_BurgerTargetMove.transform);
        ingred.transform.DOMove(toPos, 0.6f).SetEase(Ease.Linear).OnComplete(() => CheckBurgerStatus());
        A_BurgerTargetMove.images[indexIngredient].enabled = false;
    }

    int RandomBurger()
    {
        switch (mainGameContent.difficulty)
        {
            case Difficulty.Easy:
                return Random.Range(0, 4);
                //break;
            case Difficulty.Normal:
                if(timeCount <= mainGameContent.playTime/3) return Random.Range(0, 4);
                else if ((timeCount <= mainGameContent.playTime*2/3)) return Random.Range(4, 8);
                else if ((timeCount <= mainGameContent.playTime)) return Random.Range(8, 12);
                break;
            case Difficulty.Hard:
                if ((timeCount <= mainGameContent.playTime/2)) return Random.Range(4, 8);
                else if ((timeCount <= mainGameContent.playTime)) return Random.Range(8, 12);
                break;
        }
        return 0;
    }

    void CheckBurgerStatus()
    {
        bool complete = true;
        foreach(bool st in A_Burger.choosenIngredients)
        {
            if (!st)
            {
                complete = false;
                break;
            }
        }
        if (complete)
        {
            if(audioController != null) audioController?.PlaySplatDone();
            countBurger++;
            Destroy(A_Burger.gameObject);
            Destroy(A_BurgerTargetMove.gameObject);
            A_Burger = null;
            A_BurgerTargetMove = null;
            CreateBurger(false);
        }
    }

    void NextPlayer()
    {
        if (noticeTimeOut != null) noticeTimeOut?.SetActive(true);
        countPlayers++;
        Destroy(A_Burger.gameObject);
        Destroy(A_BurgerTargetMove.gameObject);
        A_Burger = null;
        A_BurgerTargetMove = null;
        generatePath.EndTurnPlayer();
        
    }

    void EndGame()
    {
        if (finishGame)
        {
            return;
        }
        if (indexTeam == 0)
        {
            Debug.Log("Join Team 1 " + mainGameContent.updatePoint1.ToString());
            if (!mainGameContent.updatePoint1) mainGameContent.pointTeam1 += countBurger;
            mainGameContent.updatePoint1 = true;
            if (mainGameContent.mode == Mode.EachGame)
            {
                mainGameContent.updatePoint2 = true;
            }
        }
        else if (indexTeam == 1)
        {
            Debug.Log("Join Team 2 " + mainGameContent.updatePoint2.ToString());
            if (!mainGameContent.updatePoint2) mainGameContent.pointTeam2 += countBurger;
            mainGameContent.updatePoint2 = true;
        }
        if (mainGameContent.updatePoint1 && mainGameContent.updatePoint2)
        {
            mainGameContent.EndGame();
            finishGame = true;
        }
    }

    public void Shuffle<T>(List<T> list) 
    { 
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1) 
        { 
            n--;
            int k = rng.Next(n + 1);
            T value = list[k]; 
            list[k] = list[n]; 
            list[n] = value; 
        }
    }
}
