using DG.Tweening;
using nuitrack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RobotGameController : MonoBehaviour
{
    

    public int indexPlayer;

    [SerializeField] RectTransform baseRect;
    [SerializeField] RectTransform imageRect;


    public float heightJump = 0;
    //public float midPoint = 0;
    //public float distanceToMidDefault = 25;
    public bool saveHeight = false;
    //public bool countdownFirst = true;
    //public float countDown = 5;

    public bool saveHeightTorso = false;
    public float defaultHeightTorso = 0;

    //public AudioController audioController;
    public float pointDistance = 0;
    //public float pointPrePlayer = 0;


    public ActionController choosenActionController;
    public ActionController choosenActionController_Fake;
    public bool isCombo;
    public bool doneFirstAction = false;
    public bool doneAllAction = false;
    public float timeA_Action;
    public float timeStartAction;

    //[Header("\nCheck Canvas")]
    //public Mode mode;
    //public ResizeImageCanvas resizeCanvas;
    //public float minMove, maxMove;
    //public float minMoveCanvas, maxMoveCanvas, midCanvas;
    //public float ratio;
    //public RectTransform mainCanvas;
    //public RectTransform areaPlayer;
    ////public Camera cameraPlayer;
    //public GameObject NoticeWarning;

    float minMainZ = 2.5f, maxMainZ = 3.5f;

    public AudioSource audioCorrect;
    //[Header("\nAudioClip")]
    //[SerializeField] AudioClip clipEat;
    //[SerializeField] AudioClip clipBubble;

    public void PrepareStart()
    {
        //resizeCanvas.Resize();
        //CaculateCanvas();

    }


    //void CaculateCanvas()
    //{
    //    if (mode == Mode.Scenario)
    //    {
    //        areaPlayer.sizeDelta = new Vector2(areaPlayer.sizeDelta.x / 2, areaPlayer.sizeDelta.y);
    //        midCanvas = mainCanvas.sizeDelta.x / 4;
    //        minMoveCanvas = midCanvas - areaPlayer.sizeDelta.x / 2;
    //        maxMoveCanvas = midCanvas + areaPlayer.sizeDelta.x / 2;
    //        if (indexPlayer == 1)
    //        {
    //            midCanvas = -midCanvas;
    //            float min = minMoveCanvas;
    //            minMoveCanvas = -maxMoveCanvas;
    //            maxMoveCanvas = -min;

    //            //cameraPlayer.rect = new Rect(0.505f, 0, 0.495f, 1);
    //        }
    //        else
    //        {
    //            //cameraPlayer.rect = new Rect(0, 0, 0.495f, 1);
    //        }
    //    }
    //    else
    //    {
    //        cameraPlayer.rect = new Rect(0, 0, 1, 1);
    //        midCanvas = 0;
    //        minMoveCanvas = -areaPlayer.sizeDelta.x / 2;
    //        maxMoveCanvas = areaPlayer.sizeDelta.x / 2;
    //    }


    //    minMoveCanvas = Mathf.Floor(minMoveCanvas * 1000) / 1000;
    //    maxMoveCanvas = Mathf.Floor(maxMoveCanvas * 1000) / 1000;

    //    ratio = Mathf.Round((Mathf.Abs(maxMove - minMove) / Mathf.Abs(maxMoveCanvas - minMoveCanvas)) * 1000000) / 1000000;
    //    //ratio = Mathf.Abs(maxMove - minMove) / Mathf.Abs(maxMoveCanvas - minMoveCanvas);
    //}

    [Obsolete]
    void Start()
    {
        //audioController = FindObjectOfType<AudioController>();
        //start_time = Time.time;
        choosenActionController = null;
        choosenActionController_Fake = null;
    }

    [Obsolete]
    void Update()
    {
        List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();

        var sortedUsers = userData.OrderByDescending(user => user.GetJoint(nuitrack.JointType.Waist).Proj.X).ToList();
        //sortedUsers = FilterSkeleton(sortedUsers);
        OnSkeletonUpdate(sortedUsers);
    }

    public List<Skeleton> FilterSkeleton(List<Skeleton> user)
    {
        List<Skeleton> newSkeleton = new List<Skeleton>();

        foreach (Skeleton s in user)
        {
            float z = s.GetJoint(JointType.Torso).Real.Z / 1000;
            if (z >= minMainZ && z <= maxMainZ)
            {
                newSkeleton.Add(s);
            }
        }

        return newSkeleton;
    }

    public void StartRun()
    {
        saveHeightTorso = true;
        saveHeight = true;
        
    }


    [Obsolete]
    private void OnSkeletonUpdate(List<Skeleton> skeletonData)
    {
        if (skeletonData.Count > indexPlayer)
        {
            nuitrack.Joint j = skeletonData[indexPlayer].GetJoint(JointType.Torso);
            float heightTorso = (float)Math.Floor(j.ToVector3().y);
            if (!saveHeightTorso)
            {
                defaultHeightTorso = heightTorso;
                return;
            }
            if (choosenActionController == null) return;
            if (isCombo)
            {
                
                if (choosenActionController != null && choosenActionController.CheckAction(skeletonData[indexPlayer], defaultHeightTorso) && !doneFirstAction)
                {
                    choosenActionController = null;
                    doneFirstAction = true;
                }
                if (choosenActionController_Fake != null && choosenActionController_Fake.CheckAction(skeletonData[indexPlayer], defaultHeightTorso) && doneFirstAction)
                {
                    choosenActionController_Fake = null;
                    doneFirstAction = false;
                    pointDistance = pointDistance + (int)(timeA_Action - (Time.time-timeStartAction));
                    doneAllAction = true;
                    audioCorrect.Play();
                }

            }
            else
            {
                
                if(choosenActionController != null && choosenActionController.CheckAction(skeletonData[indexPlayer], defaultHeightTorso))
                {
                    //Debug.Break();
                    choosenActionController = null;
                    pointDistance = pointDistance + (int)(timeA_Action - (Time.time - timeStartAction));
                    doneAllAction = true;
                    audioCorrect.Play();
                }
            }
            
        }

    }


    public Vector2 AnchoredPosition(nuitrack.Vector3 proj, Rect parentRect, RectTransform rectTransform)
    {
        Vector2 vector2 = new Vector2(Mathf.Clamp01(proj.X), Mathf.Clamp01(1 - proj.Y));
        return Vector2.Scale(vector2 - rectTransform.anchorMin, parentRect.size);
    }

    [Obsolete]
    public IEnumerator Restart()
    {
        yield return new WaitForSeconds(1.8f);
        choosenActionController = null;
        //pointDistance = pointPrePlayer;
    }

    [Obsolete]
    public void ChangePlayer()
    {
        //pointPrePlayer = pointDistance;

        choosenActionController = null;
        saveHeightTorso = false;
    }

}
