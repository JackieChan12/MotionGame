using nuitrack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RacketController : MonoBehaviour
{

    [SerializeField] int indexPlayer;
    float minMainZ = 2.5f, maxMainZ = 3.5f;
    float defaultz = 0;
    public int addDirec = 0;
    public float minMove, maxMove;
    public float minMoveCanvas, maxMoveCanvas, midCanvas;
    public float ratio;
    public RectTransform mainCanvas;
    public RectTransform areaPlayer;
    public ResizeImageCanvas resizeCanvas;
    public Mode mode;
    public Difficulty difficulty;
    public float direct;
    public Camera cameraPlayer;
    [SerializeField] RectTransform baseRect;
    [SerializeField] RectTransform imageRect;
    public Transform cube;

    UnityEngine.Vector3 position;

    void Start()
    {
        //resizeCanvas.Resize();
        //CaculateCanvas();
    }

    public void PrepareStart()
    {
        defaultz = transform.localPosition.z;
        resizeCanvas.Resize();
        CaculateCanvas();
    }


    void CaculateCanvas()
    {
        if(mode == Mode.Scenario)
        {
            midCanvas = mainCanvas.sizeDelta.x / 4;
            minMoveCanvas = midCanvas - areaPlayer.sizeDelta.x / 2;
            maxMoveCanvas = midCanvas + areaPlayer.sizeDelta.x / 2;
            if (indexPlayer == 1)
            {
                midCanvas = -midCanvas;
                float min = minMoveCanvas;
                minMoveCanvas = -maxMoveCanvas;
                maxMoveCanvas = -min;

                cameraPlayer.rect = new Rect(0.505f, 0, 0.495f, 1);
            }
            else
            {
                cameraPlayer.rect = new Rect(0, 0, 0.495f, 1);
            }
        }
        else
        {
            cameraPlayer.rect = new Rect(0, 0, 1, 1);
            midCanvas = 0;
            minMoveCanvas = -areaPlayer.sizeDelta.x / 2;
            maxMoveCanvas = areaPlayer.sizeDelta.x / 2;
        }
        

        minMoveCanvas = Mathf.Floor(minMoveCanvas * 1000) / 1000;
        maxMoveCanvas = Mathf.Floor(maxMoveCanvas * 1000) / 1000;

        ratio = Mathf.Round(( Mathf.Abs(maxMove - minMove) / Mathf.Abs(maxMoveCanvas - minMoveCanvas))*1000000)/1000000;
        //ratio = Mathf.Abs(maxMove - minMove) / Mathf.Abs(maxMoveCanvas - minMoveCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();

        var sortedUsers = userData.OrderByDescending(user => user.GetJoint(nuitrack.JointType.Waist).Proj.X).ToList();
       // sortedUsers = FilterSkeleton(sortedUsers);
        OnSkeletonUpdate(sortedUsers);
    }

    private void OnSkeletonUpdate(List<Skeleton> skeletonData)
    {
        Debug.Log("Joint" + skeletonData.Count);
        if (skeletonData.Count > indexPlayer)
        {
            nuitrack.Joint j = skeletonData[indexPlayer].GetJoint(JointType.Torso);
            float xCanvas = AnchoredPosition(j.Proj, baseRect.rect, imageRect).x;
            if (xCanvas > maxMoveCanvas || xCanvas < minMoveCanvas) return;
            position = transform.localPosition;
            position.z = (xCanvas-midCanvas) * ratio * direct + defaultz;
            Debug.Log("X Canvas " + position);
            transform.localPosition = position;

            //float zLeftShoulder = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.LeftShoulder).ToVector3().z / 100);
            //float zRightShoulder = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.RightShoulder).ToVector3().z / 100);

            //float distanceShoulder = zLeftShoulder - zRightShoulder;

            //if(Math.Abs(distanceShoulder) > 2f)
            //{
            //    if(distanceShoulder < 0)
            //    {
            //        addDirec = 1;
            //        Debug.LogWarning("Join Left");
            //    }
            //    else
            //    {
            //        addDirec = -1;
            //        Debug.LogWarning("Join Right");
            //    }
            //}
            //else
            //{
            //    addDirec = 0;
            //    Debug.LogWarning("Join Mid");
            //}

        }
    }

    public Vector2 AnchoredPosition(nuitrack.Vector3 proj, Rect parentRect, RectTransform rectTransform)
    {
        Vector2 vector2 = new Vector2(Mathf.Clamp01(proj.X), Mathf.Clamp01(1 - proj.Y));
        return Vector2.Scale(vector2 - rectTransform.anchorMin, parentRect.size);
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
}
