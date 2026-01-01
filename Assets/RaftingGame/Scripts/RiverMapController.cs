using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverMapController : MonoBehaviour
{
    public Transform player;
    public List<GameObject> listStage;
    public List<GameObject> listInstaceStage;
    public GameObject prefabPlusPoint;
    public GameObject prefabPlusPoint2;

    public float distanceStage = 18f;
    public Transform transformParent;
    public Transform transformBegin;
    public Transform transformBegin_2;
    public Transform transformEnd;

    public float zDefault = 4.5f;
    public Difficulty difficulty;
    public bool pause = false;
    float z = 0;
    Vector3 newPos = Vector3.zero;
    int index = 0; int endRan = 12;
    [System.Obsolete]
    void Start()
    {
        SettingLevel();
        Begin();
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if (pause)
        {
            Pause();
            pause = false;
        }
    }

    //public void SetStageSpeed(float s)
    //{
    //    foreach (var stage in listInstaceStage)
    //    {
    //        speedStageMove = s;
    //        stage.speed = s;
    //    }
    //}

    public void SettingLevel()
    {
        switch (/*InputManager.Instance.*/difficulty)
        {
            case Difficulty.Easy:
                distanceStage = 22f;
                endRan = 4;
                break;
            case Difficulty.Normal:
                distanceStage = 17f;
                endRan = 6;
                break;
            case Difficulty.Hard:
                distanceStage = 13f;
                endRan = 9;
                break;
        }
    }

    [System.Obsolete]
    public void Begin()
    {
        newPos = Vector3.zero;
        index = 0;
        while (newPos.z < transformEnd.position.z)
        {
            z = index * distanceStage + transformBegin.position.z;
            newPos.z = z;
            GameObject curStage = Instantiate(listStage[Random.RandomRange(0, endRan)], newPos, Quaternion.identity, transformParent);
            curStage.transform.localPosition = newPos;
            curStage.gameObject.SetActive(true);
            listInstaceStage.Add(curStage);

            newPos.x = Random.RandomRange(-18, 18);
            z = index * distanceStage + transformBegin_2.position.z;
            newPos.z = z;
            GameObject addPointObj = Instantiate(Random.Range(0,2) == 0 ? prefabPlusPoint : prefabPlusPoint2, newPos, Quaternion.identity, transformParent);

            plusPointController pl = addPointObj.GetComponent<plusPointController>();
            if ( pl != null)
            {
                pl.player = player;
            }

            addPointObj.SetActive(true);
            listInstaceStage.Add(addPointObj);
            index++;
        }
    }

    [System.Obsolete]
    //public void InstanceStage()
    //{
    //    listInstaceStage.Remove(listInstaceStage[0]);
    //    Debug.Log(curStage.localPosition.z);
    //    Transform curStageN = Instantiate(listStage[Random.RandomRange(0, listStage.Count)], newPos, Quaternion.identity, transformParent).transform;
    //    z = curStage.localPosition.z + distanceStage - 0.1f;
    //    newPos.z = z;
    //    curStageN.localPosition = newPos;
    //    StageMove stMove = curStageN.GetComponent<StageMove>();
    //    listInstaceStage.Add(stMove);
    //    stMove.GenerateObstacle();
    //    stMove.toEnd = InstanceStage;
    //    stMove.SetupStage(speedStageMove, zEnd);
    //    curStageN.gameObject.SetActive(true);
    //    curStage = curStageN;
    //}

    public void Pause()
    {
        foreach (var st in listInstaceStage)
        {
            //st.isMoving = false;
        }
        //Restart();
    }

    public void AcceptMove()
    {
        foreach (var st in listInstaceStage)
        {
            //st.SetupStage(speedStageMove, zEnd);
        }
    }

    [System.Obsolete]
    public void Restart()
    {
        foreach (var st in listInstaceStage)
        {
            Destroy(st.gameObject);
        }
        listInstaceStage.Clear();
        Begin();
    }
}
