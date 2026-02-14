using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateObstacleInRace : MonoBehaviour
{
    public GameObject levelObstacle_EASY;
    public GameObject levelObstacle_NORMAL;
    public GameObject levelObstacle_HARD;
    GameObject levelObstacleChoosen;
    List<Transform> listObstaclePos = new List<Transform>();
    public List<GameObject> listStage;
    public GameObject plusPoint;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Obsolete]
    public void SetupObstacle(Difficulty difficulty)
    {
        levelObstacle_EASY.SetActive(false);
        levelObstacle_NORMAL.SetActive(false);
        levelObstacle_HARD.SetActive(false);

        switch (difficulty) { 
            case Difficulty.Easy:
                levelObstacle_EASY.SetActive(true);
                levelObstacleChoosen = levelObstacle_EASY;
                break;
            case Difficulty.Normal:
                levelObstacle_NORMAL.SetActive(true);
                levelObstacleChoosen = levelObstacle_NORMAL;
                break;
            case Difficulty.Hard:
                levelObstacle_HARD.SetActive(true);
                levelObstacleChoosen = levelObstacle_HARD;
                break;
        }
        listObstaclePos = levelObstacleChoosen.GetComponentsInChildren<Transform>().ToList();
        foreach (var item in listObstaclePos)
        {
            //item.gameObject.SetActive(false);
            if (Random.RandomRange(0,5) == 1 )
                Instantiate(plusPoint, item.position, item.rotation, item);
            else
            {
                Instantiate(listStage[Random.RandomRange(0, listStage.Count)], item.position, item.rotation, item).SetActive(true);
            }
        }
    }
}
