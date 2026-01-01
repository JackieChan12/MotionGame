using nuitrack;
using PathCreation;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AnimalRace_Movement : MonoBehaviour
{
    public int indexPlayer=0;

    public float xPlayer;
    [SerializeField] List<GameObject> animals = new List<GameObject>();

    [SerializeField] PathCreator creatorPath;
    [SerializeField] PathFollower pathFollower;

    public TMP_Text textPoint;
    public Camera cam;
    public bool startGame = false;
    public float curSpeed = 0;
    public int indexMovement = 0;

    private int stepCount = 0;
    private float speedUpdateInterval = 1f;

    public float point;

    float minMainZ = 2.5f, maxMainZ = 3.5f;

    /// <summary>
    bool isPreLeft = false, isPreRight=false;

    private float lastLeftY;
    private float lastRightY; 
    private int flapCount = 0;

    private string lastLean = "";
    //private float lastLeanTime = 0f;
    private int leanCount = 0;
    /// </summary>
    Animator animator;
    int indexAnimal;

    void Start()
    {
        NuitrackManager.SkeletonTracker.SetNumActiveUsers(3);
        pathFollower.speed = curSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        textPoint.text = point.ToString("N0");
        point = pathFollower.distanceTravelled;
        if (startGame)
        {
            List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();
            userData = FilterSkeleton(userData);
            //Movement_Stepping(userData[indexPlayer]);
            //Movement_Flapping(userData[indexPlayer]);
            //Movement_Leanning(userData[indexPlayer]);
            //Movement_LeanningPenguin(userData[indexPlayer]);
            //Movement_SteppingAndUpTwoHand(userData[indexPlayer]);
            //Movement_Swimming(userData[indexPlayer]);   // có thể áp dụng cho bò

            switch (indexAnimal)
            {
                case 0:
                    Movement_Stepping(userData[indexPlayer]);
                    break;
                case 01:
                    Movement_Flapping(userData[indexPlayer]);
                    break;
                case 02:
                    Movement_Leanning(userData[indexPlayer]);
                    break;
                case 03:
                    Movement_Swimming(userData[indexPlayer]);
                    break;
                case 04:
                    Movement_LeanningPenguin(userData[indexPlayer]);
                    break;
                case 05:
                    Movement_SteppingAndUpTwoHand(userData[indexPlayer]);
                    break;
            }

            pathFollower.speed = curSpeed;
            if(curSpeed > 0)
            {
                animator.SetBool("Run", true);
            }
            else
            {
                animator.SetBool("Run", false);
            }
            xPlayer = NuitrackManager.SkeletonTracker != null ? NuitrackManager.SkeletonTracker.GetSkeletonData().Skeletons[indexPlayer].GetJoint(JointType.Head).Real.X : 0;

        }
        else
        {
            animator.SetBool("Run", false);
        }
    }

    public void RandomAnimal(Difficulty d)
    {
        switch (d)
        {
            case Difficulty.Easy:
                indexAnimal = Random.Range(0, 2);
                break;
            case Difficulty.Normal:
                indexAnimal = Random.Range(2, 4);
                break;
            case Difficulty.Hard:
                indexAnimal = Random.Range(4, 6);
                break;
        }
        foreach(var v in animals)
        {
            v.SetActive(false);
        }
        animals[indexAnimal].SetActive(true);
        animator = animals[indexAnimal].GetComponent<Animator>();
    }

    public void Movement_Stepping(Skeleton userData)
    {
        if (IsStepping(userData) /*&& Time.time - lastStepTime > 0.5f*/)
        {
            stepCount++;
        }

        // Cập nhật tốc độ mỗi giây
        if (Time.frameCount % (int)(speedUpdateInterval / Time.deltaTime) == 0)
        {
            curSpeed = Mathf.Clamp(stepCount / 2f, 0f, 2f); // Giới hạn từ 0 đến 2
            stepCount = 0; // reset sau mỗi chu kỳ
        }
    } // 0
    bool IsStepping(Skeleton skeleton)
    {
        nuitrack.Vector3 leftFoot = skeleton.GetJoint(JointType.LeftKnee).Real;
        nuitrack.Vector3 rightFoot = skeleton.GetJoint(JointType.RightKnee).Real;
        nuitrack.Vector3 torso = skeleton.GetJoint(JointType.Torso).Real;

        float baseFootHeight = torso.Y * 0.1f;

        bool leftStep = leftFoot.Z - rightFoot.Z > 5f;
        bool rightStep = rightFoot.Z - leftFoot.Z > 5f;

        Debug.LogWarning("AAAAAAAAAAAAAAAA" + (leftFoot.Z - rightFoot.Z) + " ; " + (rightFoot.Z - leftFoot.Z));

        bool r = leftStep && !isPreLeft || rightStep && !isPreRight;

        isPreLeft = leftStep;
        isPreRight = rightStep;
        return r;

    }

    public void Movement_Flapping(Skeleton skeleton)
    {
        nuitrack.Vector3 leftHand = skeleton.GetJoint(JointType.LeftWrist).Real;
        nuitrack.Vector3 rightHand = skeleton.GetJoint(JointType.RightWrist).Real;
        nuitrack.Vector3 leftShoulder = skeleton.GetJoint(JointType.LeftShoulder).Real;
        nuitrack.Vector3 rightShoulder = skeleton.GetJoint(JointType.RightShoulder).Real;

        float shoulderHeight = (leftShoulder.Y + rightShoulder.Y) / 2f;
        float flapThreshold = 0.15f; // biên độ vỗ

        // Kiểm tra nếu tay vượt qua vai theo chiều dọc
        bool leftFlap = Mathf.Abs(leftHand.Y - lastLeftY) > flapThreshold && ( leftHand.Y - shoulderHeight) > 0.1f;
        bool rightFlap = Mathf.Abs(rightHand.Y - lastRightY) > flapThreshold && (rightHand.Y - shoulderHeight) > 0.1f;

        if ((leftFlap || rightFlap) /*&& Time.time - lastFlapTime > 0.2f*/)
        {
            flapCount++;

        }

        lastLeftY = leftHand.Y;
        lastRightY = rightHand.Y;
        

        // Cập nhật tốc độ mỗi giây
        if (Time.frameCount % (int)(speedUpdateInterval / Time.deltaTime) == 0)
        {
            curSpeed = Mathf.Clamp(flapCount / 2f, 0f, 2f); // scale về 0–2
            flapCount = 0;
        }
    } //1

    public void Movement_Leanning(Skeleton skeleton)
    {
        nuitrack.Vector3 leftShoulder = skeleton.GetJoint(JointType.LeftShoulder).Real;
        nuitrack.Vector3 rightShoulder = skeleton.GetJoint(JointType.RightShoulder).Real;
        nuitrack.Vector3 leftHip = skeleton.GetJoint(JointType.LeftHip).Real;
        nuitrack.Vector3 rightHip = skeleton.GetJoint(JointType.RightHip).Real;

        float shoulderCenterX = (leftShoulder.X + rightShoulder.X) / 2f;
        float hipCenterX = (leftHip.X + rightHip.X) / 2f;
        float leanOffset = shoulderCenterX - hipCenterX;

        string currentLean = "";
        if (leanOffset > 0.1f) currentLean = "right";
        else if (leanOffset < -0.1f) currentLean = "left";

        // Nếu đổi hướng nghiêng → tính là 1 lần nghiêng
        if (currentLean != "" && currentLean != lastLean /*& Time.time - lastLeanTime > 0.3f*/)
        {
            leanCount++;
            lastLean = currentLean;
            //lastLeanTime = Time.time;
        }

        // Cập nhật tốc độ mỗi giây
        if (Time.frameCount % (int)(speedUpdateInterval / Time.deltaTime) == 0)
        {
            curSpeed = Mathf.Clamp(leanCount / 2f, 0f, 2f);
            leanCount = 0;
        }
    }//2
    public void Movement_Swimming(Skeleton skeleton) 
    {
        nuitrack.Vector3 leftHand = skeleton.GetJoint(JointType.LeftWrist).Real;
        nuitrack.Vector3 rightHand = skeleton.GetJoint(JointType.RightWrist).Real;
        nuitrack.Vector3 leftShoulder = skeleton.GetJoint(JointType.LeftShoulder).Real;
        nuitrack.Vector3 rightShoulder = skeleton.GetJoint(JointType.RightShoulder).Real;

        float shoulderHeight = (leftShoulder.Y + rightShoulder.Y) / 2f;
        float flapThreshold = 0.15f; // biên độ vỗ

        // Kiểm tra nếu tay vượt qua vai theo chiều dọc
        bool leftFlap = Mathf.Abs(leftHand.Y - lastLeftY) > flapThreshold && (leftHand.Y - shoulderHeight) > 0.1f;
        bool rightFlap = Mathf.Abs(rightHand.Y - lastRightY) > flapThreshold && (rightHand.Y - shoulderHeight) > 0.1f;

        if ((leftFlap || rightFlap) && (leftHand.Z<leftShoulder.Z || rightHand.Z<rightShoulder.Z)/*&& Time.time - lastFlapTime > 0.2f*/)
        {
            flapCount++;

        }

        lastLeftY = leftHand.Y;
        lastRightY = rightHand.Y;


        // Cập nhật tốc độ mỗi giây
        if (Time.frameCount % (int)(speedUpdateInterval / Time.deltaTime) == 0)
        {
            curSpeed = Mathf.Clamp(flapCount / 2f, 0f, 2f); // scale về 0–2
            flapCount = 0;
        }
    }//3

    public void Movement_LeanningPenguin(Skeleton skeleton)
    {
        nuitrack.Vector3 leftShoulder = skeleton.GetJoint(JointType.LeftShoulder).Real;
        nuitrack.Vector3 rightShoulder = skeleton.GetJoint(JointType.RightShoulder).Real;
        nuitrack.Vector3 leftHip = skeleton.GetJoint(JointType.LeftHip).Real;
        nuitrack.Vector3 rightHip = skeleton.GetJoint(JointType.RightHip).Real;

        float shoulderCenterX = (leftShoulder.X + rightShoulder.X) / 2f;
        float hipCenterX = (leftHip.X + rightHip.X) / 2f;
        float leanOffset = shoulderCenterX - hipCenterX;

        string currentLean = "";
        if (leanOffset > 0.1f) currentLean = "right";
        else if (leanOffset < -0.1f) currentLean = "left";

        // Nếu đổi hướng nghiêng → tính là 1 lần nghiêng
        if (currentLean != "" && currentLean != lastLean && Mathf.Abs(skeleton.GetJoint(JointType.LeftShoulder).Real.X - skeleton.GetJoint(JointType.RightShoulder).Real.X) < Mathf.Abs(skeleton.GetJoint(JointType.LeftWrist).Real.X - skeleton.GetJoint(JointType.RightWrist).Real.X) /*& Time.time - lastLeanTime > 0.3f*/)
        {
            leanCount++;
            lastLean = currentLean;
            //lastLeanTime = Time.time;
        }

        // Cập nhật tốc độ mỗi giây
        if (Time.frameCount % (int)(speedUpdateInterval / Time.deltaTime) == 0)
        {
            curSpeed = Mathf.Clamp(leanCount / 2f, 0f, 2f);
            leanCount = 0;
        }
    }//4

    public void Movement_SteppingAndUpTwoHand(Skeleton userData)
    {
        float yHeight = userData.GetJoint(JointType.Head).Real.Y;
        if (IsStepping(userData) && userData.GetJoint(JointType.RightWrist).Real.Y > yHeight && userData.GetJoint(JointType.LeftWrist).Real.Y > yHeight /*&& Time.time - lastStepTime > 0.5f*/)
        {
            stepCount++;
        }

        // Cập nhật tốc độ mỗi giây
        if (Time.frameCount % (int)(speedUpdateInterval / Time.deltaTime) == 0)
        {
            curSpeed = Mathf.Clamp(stepCount / 2f, 0f, 2f); // Giới hạn từ 0 đến 2
            stepCount = 0; // reset sau mỗi chu kỳ
        }
    }//5


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
