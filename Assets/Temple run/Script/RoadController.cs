using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RoadController : MonoBehaviour
{
    public List<GameObject> listStage;
    public List<StageMove> listInstaceStage;

    public float distanceStage = 18f;
    public Transform transformParent;
    public Transform transformBegin;
    public Transform transformEnd;

    public float zDefault = 4.5f;
    public float zEnd = 0;

    public float speedStageMove = 5f;
    public bool pause = false;
    public Difficulty difficulty;
    Transform curStage = null;
    float z = 0;
    Vector3 newPos = Vector3.zero;
    int index = -1;
    int startRan = 0; int endRan = 12; int numObstacle = 2;
    [System.Obsolete]
    void Start()
    {
        SettingLevel();
        zEnd = transformEnd.position.z;
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

    public void SetStageSpeed(float s)
    {
        foreach (var stage in listInstaceStage) {
            speedStageMove = s;
            stage.speed = s;
        }
    }

    public void SettingLevel()
    {
        switch (InputManager.Instance.difficulty)
        {
            case Difficulty.Easy:
                endRan = 5;
                numObstacle = 1;
                break;
            case Difficulty.Normal:
                endRan = 11;
                numObstacle = 2;
                break;
            case Difficulty.Hard:
                endRan = 12;
                numObstacle = 2;
                break;
        }
    }

    [System.Obsolete]
    public void Begin()
    {
        zEnd = transformEnd.position.z;
        z = 0; index = -1;
        newPos = Vector3.zero;
        while (curStage == null || newPos.z < transformBegin.position.z)
        {
            z= index*distanceStage + zDefault;
            newPos.z = z;
            curStage = Instantiate(listStage[Random.RandomRange(0, listStage.Count)], newPos, Quaternion.identity, transformParent).transform;
            curStage.localPosition = newPos;
            StageMove stMove = curStage.GetComponent<StageMove>();
            listInstaceStage.Add(stMove);
            stMove.toEnd = InstanceStage;
            //stMove.SetupStage(speedStageMove, zEnd);
            curStage.gameObject.SetActive(true);
            index++;
            if (index >= 2)
            {
                stMove.GenerateObstacle(startRan,endRan,numObstacle);
            }
        }
    }

    [System.Obsolete]
    public void InstanceStage()
    {
        listInstaceStage.Remove(listInstaceStage[0]);
        Debug.Log(curStage.localPosition.z);
        Transform curStageN = Instantiate(listStage[Random.RandomRange(0, listStage.Count)], newPos, Quaternion.identity, transformParent).transform;
        z = curStage.localPosition.z + distanceStage-0.1f;
        newPos.z = z;
        curStageN.localPosition = newPos;   
        StageMove stMove = curStageN.GetComponent<StageMove>();
        listInstaceStage.Add(stMove);
        stMove.GenerateObstacle();
        stMove.toEnd = InstanceStage;
        stMove.SetupStage(speedStageMove, zEnd);
        curStageN.gameObject.SetActive(true);
        curStage = curStageN;
    }

    [System.Obsolete]
    public void Pause()
    {
        foreach(var st in listInstaceStage)
        {
            st.isMoving = false;
        }
        //Restart();
    }

    public void AcceptMove()
    {
        foreach (var st in listInstaceStage)
        {
            st.SetupStage(speedStageMove, zEnd);
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
