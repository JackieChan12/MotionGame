using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ImageWithName
{
    public Sprite _image;
    public string name;
}

public class ImageInfor : MonoBehaviour
{
    public int index = 0;
    public List<ImageWithName> spriteObjectsEasy = new List<ImageWithName>();
    public List<ImageWithName> spriteObjectsNormal = new List<ImageWithName>();
    public List<ImageWithName> spriteObjectsHard = new List<ImageWithName>();
    public List<Sprite>  spriteWords = new List<Sprite>(); // TO random list
    List<int> curListIndexSprites = new List<int>();
    public List<A_Word> words = new List<A_Word>();
    public List<Transform> listWordPos = new List<Transform>();
    public GameObject wordPrefab;
    public Transform spawnPos;
    List<ImageWithName> spriteObjectsUsed = new List<ImageWithName>();
    int curImage;
    public AudioController audioController;
    public Image uiImage; //show Object

    Dictionary<char, int> alphabetOrder = new Dictionary<char, int>();
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    List<int> fullIndexChar = new List<int>();
    public Difficulty difficulty;
    //----Checking----////
    public TMP_Text textRight;
    int curChar = 0;
    public int point = 0;

    void Start()
    {
        for (int i = 0; i < alphabet.Length; i++)
        {
            alphabetOrder[alphabet[i]] = i;
            fullIndexChar.Add(i);
        }
    }

    public void StartGame()
    {
        for (int i = 0; i < alphabet.Length; i++)
        {
            alphabetOrder[alphabet[i]] = i;
            fullIndexChar.Add(i);
        }
        curChar = 0;
        textRight.text = "";
        switch (difficulty)
        {
            case Difficulty.Easy:
                spriteObjectsUsed = spriteObjectsEasy;
                break;
            case Difficulty.Normal:
                spriteObjectsUsed = spriteObjectsNormal;
                break;
            case Difficulty.Hard:
                spriteObjectsUsed = spriteObjectsHard;
                break;
        }

        curImage = Random.Range(0, spriteObjectsUsed.Count);
        uiImage.sprite = spriteObjectsUsed[curImage]._image;
        curListIndexSprites.Clear();
        for (int i=0; i< spriteObjectsUsed[curImage].name.Length; i++)
        {
            curListIndexSprites.Add(alphabetOrder[spriteObjectsUsed[curImage].name[i]]);
        }

        curListIndexSprites = GetRandomList(fullIndexChar, curListIndexSprites, 7);
        foreach(var w in words)
        {
            Destroy(w.gameObject);
        }
        words.Clear();
        for (int i=0; i < listWordPos.Count; i++)
        {
            GameObject g = Instantiate(wordPrefab, spawnPos);
            g.SetActive(true);
            words.Add(g.GetComponent<A_Word>());
            words[i].posTarget = listWordPos[i];
            //words[i].posRight = listWordPos[i];
            //words[i]._image.sprite = spriteWords[curListIndexSprites[i]];
            words[i]._word = alphabetOrder.Keys.ElementAt(curListIndexSprites[i]);
            words[i]._textChar.text = words[i]._word.ToString();
            words[i].Begin();
            words[i].choosen = CheckWord;
        }
    }

    public void CheckWord(A_Word aw)
    {
        if(spriteObjectsUsed[curImage].name[curChar] == aw._word)
        {
            curChar++;
            string wd = aw._word.ToString();
            aw.mySequence.Kill();
            aw.RightChoose();
            aw.mySequence.OnComplete(() =>
            {
                textRight.text = textRight.text + wd;
            });
            if(curChar == spriteObjectsUsed[curImage].name.Length)
            {
                audioController.PlaySplatDone();
                aw.mySequence.Complete();
                foreach (var w in words)
                {
                    w.ResetWord();
                }
                point++;
                StartCoroutine(RestartWord());
            }
            else
            {
                audioController.PlaySplat();
            }
        }
    }

    IEnumerator RestartWord()
    {
        yield return new WaitForSeconds(1);
        StartGame();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetGame()
    {
        foreach(var w in words)
        {
            w.ResetWord();
        }
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
