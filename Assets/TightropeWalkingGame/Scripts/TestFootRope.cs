using nuitrack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestFootRope : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OnSkeletonUpdate(NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList());
    }

    private void OnSkeletonUpdate(List<Skeleton> skeletonData)
    {
        float zLeftAnkle = (float)Math.Floor(skeletonData[0].GetJoint(JointType.LeftAnkle).Real.X/100);
        float zRightAnkle = (float)Math.Floor(skeletonData[0].GetJoint(JointType.RightAnkle).Real.X/100);
        float yLeftAnkle = (float)Math.Floor(skeletonData[0].GetJoint(JointType.LeftAnkle).Real.Y / 10);
        float yRightAnkle = (float)Math.Floor(skeletonData[0].GetJoint(JointType.RightAnkle).Real.Y / 10);

        Debug.LogWarning("Left foot :" + zLeftAnkle + "; " + yLeftAnkle + "; Right foot : " + zRightAnkle + "; " + yRightAnkle);
    }
}
