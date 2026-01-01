using nuitrack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class TestNuitrack : MonoBehaviour
{
    [SerializeField] RectTransform baseRect;
    [SerializeField] GameObject prefabJoint;
    [SerializeField] public List<List<Image>> listObjectImage = new List<List<Image>>();
    public List<nuitrack.JointType> listJointType;
    [SerializeField] RectTransform knee;

    [SerializeField] Sprite spriteHead;
    [SerializeField] Sprite spriteWrist;
    [SerializeField] Sprite spriteAnkle;
    [SerializeField] Sprite spriteHead1;
    [SerializeField] Sprite spriteWrist1;
    [SerializeField] Sprite spriteAnkle1;

    [SerializeField] Color colorTransparent;

    public bool stopCheckKnee = false;
    // Start is called before the first frame update
    void Awake()
    {
        for(int i=0; i < 2; i++)
        {
            List<Image> list = new List<Image>();
            for (int j = 0; j < listJointType.Count; j++)
            {
                GameObject joint = Instantiate(prefabJoint, baseRect);
                joint.name = listJointType[j].ToString() + (i == 0 ? "" : "1");
                list.Add(joint.GetComponent<Image>());
            }
            listObjectImage.Add(list);
        }
    }

    nuitrack.Vector3 NuitrackVector(UnityEngine.Vector3 vector)
    {
        return new nuitrack.Vector3(vector.x, vector.y, vector.z);
    }

    [System.Obsolete]
    UnityEngine.Vector3 FrameSpaceProjPoint(UnityEngine.Vector3 realPoint, nuitrack.DepthFrame frame)
    {
        realPoint *= 1000;
        nuitrack.Vector3 nuitrackPoint = NuitrackVector(realPoint);
        nuitrack.Vector3 point = NuitrackManager.DepthSensor.ConvertRealToProjCoords(nuitrackPoint);

        point.X /= frame.Cols;
        point.Y /= frame.Rows;

        return point.ToVector3();
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {

        List<Skeleton> userData = NuitrackManager.SkeletonTracker.GetSkeletonData().Skeletons.ToList();

        var sortedUsers = userData.OrderBy(user => user.GetJoint(nuitrack.JointType.Waist).Proj.X).ToList();

        if(sortedUsers.Count < 2)
        {
            OffTeam2();
        }
        else
        {
            OffTeam2(false);
        }

        for (int ji = 0; ji < sortedUsers.Count; ji++)
        {
            Debug.Log("?????????????????????????????????????");
            if (sortedUsers[ji] != null)
            {
                for (int i = 0; i < listJointType.Count; i++)
                {
                    nuitrack.Joint j = sortedUsers[ji].GetJoint(listJointType[i]);

                    listObjectImage[ji][i].rectTransform.anchoredPosition = AnchoredPosition(j.Proj, baseRect.rect, listObjectImage[ji][i].rectTransform);
                    if (listJointType[i]== nuitrack.JointType.LeftKnee && !stopCheckKnee)
                    {
                        Vector2 anchorKnee = listObjectImage[ji][i].rectTransform.anchoredPosition;
                        anchorKnee.x = 0;
                        anchorKnee.y -=50;
                        knee.anchoredPosition = anchorKnee;
                    }
                    if (listJointType[i] == nuitrack.JointType.LeftKnee || listJointType[i] == nuitrack.JointType.RightKnee)
                    {
                        listObjectImage[ji][i].gameObject.SetActive(false);
                        listObjectImage[ji][i].color = colorTransparent;
                    }
                    else if (listJointType[i] == nuitrack.JointType.Head)
                    {
                        listObjectImage[ji][i].sprite = ji % 2 == 0 ? spriteHead : spriteHead1;
                    }
                    else if (listJointType[i] == nuitrack.JointType.LeftWrist || listJointType[i] == nuitrack.JointType.RightWrist)
                    {
                        listObjectImage[ji][i].sprite = ji % 2 == 0 ? spriteWrist : spriteWrist1;
                    }
                    else if (listJointType[i] == nuitrack.JointType.LeftAnkle || listJointType[i] == nuitrack.JointType.RightAnkle)
                    {
                        listObjectImage[ji][i].sprite = ji % 2 == 0 ? spriteAnkle : spriteAnkle1;
                    }
                    //else if (listJointType[i] == nuitrack.JointType.Head)
                    //{
                    //    listObjectImage[ji][i].sprite = spriteHead;
                    //}
                    //else if (listJointType[i] == nuitrack.JointType.LeftWrist || listJointType[i] == nuitrack.JointType.RightWrist)
                    //{
                    //    listObjectImage[ji][i].sprite = spriteWrist;
                    //}
                    //else if (listJointType[i] == nuitrack.JointType.LeftAnkle || listJointType[i] == nuitrack.JointType.RightAnkle)
                    //{
                    //    listObjectImage[ji][i].sprite = spriteAnkle;
                    //}
                }
                

            }
        }
    }

    public void SetSpriteTeam1(Sprite h1, Sprite w1, Sprite a1)
    {
        spriteHead = h1;
        spriteWrist = w1;
        spriteAnkle = a1;
    }

    public void SetSpriteTeam2(Sprite h2, Sprite w2, Sprite a2)
    {
        spriteHead1 = h2;
        spriteWrist1 = w2;
        spriteAnkle1 = a2;
    }

    public Vector2 AnchoredPosition(nuitrack.Vector3 proj, Rect parentRect, RectTransform rectTransform)
    {
        Vector2 vector2 = new Vector2(Mathf.Clamp01(proj.X), Mathf.Clamp01(1 - proj.Y));
        return Vector2.Scale(vector2 - rectTransform.anchorMin, parentRect.size);
    }

    //public void ProcessSkeleton(nuitrack.Skeleton skeleton)
    //{
    //    if (skeleton == null)
    //        return;

    //    for (int i = 0; i < jointsInfo.Length; i++)
    //    {
    //        nuitrack.Joint j = skeleton.GetJoint(jointsInfo[i]);
    //        if (j.Confidence > 0.5f)
    //        {
    //            joints[jointsInfo[i]].SetActive(true);
    //            joints[jointsInfo[i]].transform.position = new Vector2(j.Proj.X * Screen.width, Screen.height - j.Proj.Y * Screen.height);
    //        }
    //        else
    //        {
    //            joints[jointsInfo[i]].SetActive(false);
    //        }
    //    }
    //}

    public void OffTeam2(bool off = true)
    {
        foreach(var j in listObjectImage[1])
        {
            j.gameObject.SetActive(!off);
        }
    }
}
