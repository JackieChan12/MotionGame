using nuitrack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionController 
{
    [SerializeField] List<JointType> listJoint;
    [SerializeField] JointType mappedPoint;
    [SerializeField] JointType blockPoint;
    [SerializeField] float thresholdDistance;
    [SerializeField] bool isActionToSide;
    [SerializeField] public AudioClip soundNameAction;
    [SerializeField] public string strNameAction;
    bool check;
    float yPos;
    float yPosMapped;

    public bool CheckAction(Skeleton skeletonPlayer, float oldPosY = 0)
    {
        Debug.Log(skeletonPlayer.Joints.Length);
        check = true;
        if (mappedPoint == JointType.None)
        {
            yPosMapped = oldPosY;
            foreach(JointType jointType in listJoint)
            {
                yPos = skeletonPlayer.GetJoint(jointType).ToVector3().y;
                if(yPos- (yPosMapped - thresholdDistance) < thresholdDistance)
                {
                    check = false; break;
                }
            }
        }
        else
        {
            yPosMapped = skeletonPlayer.GetJoint(mappedPoint).ToVector3().y;
            foreach (JointType jointType in listJoint)
            {
                yPos = skeletonPlayer.GetJoint(jointType).ToVector3().y;
                Debug.Log($"yPos : {yPos} ; yMapped : {yPosMapped} ; ");
                if (yPos - (yPosMapped- thresholdDistance) < thresholdDistance)
                {
                    check = false; break;
                }
            }
        }
        if (isActionToSide && check)
        {
            yPosMapped = skeletonPlayer.GetJoint(JointType.Torso).ToVector3().z;
            foreach (JointType jointType in listJoint)
            {
                yPos = skeletonPlayer.GetJoint(jointType).ToVector3().z;
                if (yPos - yPosMapped > thresholdDistance)
                {
                    check = false; break;
                }
            }
        }
        if(blockPoint != JointType.None && check)
        {
            yPosMapped = skeletonPlayer.GetJoint(mappedPoint).ToVector3().y;
            yPos = skeletonPlayer.GetJoint(blockPoint).ToVector3().y;
            Debug.Log($"yPos : {yPos} ; yMapped : {yPosMapped} ; ");
            if (yPos - yPosMapped >= thresholdDistance)
            {
                check = false;
            }
            if (isActionToSide && check)
            {
                yPosMapped = skeletonPlayer.GetJoint(mappedPoint).ToVector3().z;
                yPos = skeletonPlayer.GetJoint(blockPoint).ToVector3().z;
                if (yPos - yPosMapped <= thresholdDistance)
                {
                    check = false;
                }
            }
        }

        return check;
    }
}
