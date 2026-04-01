using nuitrack;
using PathCreation;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AnimalRace_MovementNew : MonoBehaviour
{
    public int indexPlayer = 0;
    public float xPlayer;
    [SerializeField] List<GameObject> animals = new List<GameObject>();
    [SerializeField] PathCreator creatorPath;
    [SerializeField] PathFollower pathFollower;

    public TMP_Text textPoint;
    public Camera cam;
    public bool startGame = false;

    // ----- Speed & Movement Calculation (Học từ HurdleRaceNew) -----
    public float curSpeed = 0f;
    private float targetSpeed = 0f;

    [Header("Running Detection")]
    public int minRepsPerSecond = 1;         // Số lần làm thao tác tối thiểu trong 1s để bắt đầu đi
    public float speedLerpUp = 5f;           // Gia tốc tăng tốc
    public float speedLerpDown = 10f;        // Gia tốc giảm tốc đà
    private float speedUpdateTimer = 0f;
    private float speedUpdateInterval = 1f;  // Chu kỳ tính tốc độ (1 giây)
    private int rep_count = 0;               // Đếm số lần động tác hoàn thành

    // ----- Joint Tracking Variables ----- //
    private float previous_z_left = 0f;
    private float previous_z_right = 0f;
    private float previous_y_leftArm = 0f;
    private float previous_y_rightArm = 0f;
    private string previous_lean = "";

    public float point;
    Animator animator;
    int indexAnimal;

    void Start()
    {
        if(NuitrackManager.SkeletonTracker != null)
            NuitrackManager.SkeletonTracker.SetNumActiveUsers(3);
        pathFollower.speed = curSpeed;
    }

    void Update()
    {
        if(textPoint) textPoint.text = point.ToString("N0");
        point = pathFollower.distanceTravelled;

        if (startGame)
        {
            List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();
            if(userData != null && userData.Count > indexPlayer && userData[indexPlayer] != null)
            {
                Skeleton skeleton = userData[indexPlayer];

                switch (indexAnimal)
                {
                    case 0:
                        Movement_Stepping(skeleton);
                        break;
                    case 1:
                        Movement_Flapping(skeleton);
                        break;
                    case 2:
                        Movement_Leanning(skeleton);
                        break;
                    case 3:
                        Movement_Swimming(skeleton);
                        break;
                    case 4:
                        Movement_LeanningPenguin(skeleton);
                        break;
                    case 5:
                        Movement_SteppingAndUpTwoHand(skeleton);
                        break;
                }

                // --------- TÍNH TỐC ĐỘ VÀ LERP (Từ HurdleRaceNew) ---------
                speedUpdateTimer += Time.deltaTime;
                if (speedUpdateTimer >= speedUpdateInterval)
                {
                    targetSpeed = (rep_count >= minRepsPerSecond) ? Mathf.Clamp(rep_count / 2f, 0f, 3f) : 0f;
                    rep_count = 0;
                    speedUpdateTimer = 0f;
                }

                float lerpRate = (targetSpeed > curSpeed) ? speedLerpUp : speedLerpDown;
                curSpeed = Mathf.Lerp(curSpeed, targetSpeed, Time.deltaTime * lerpRate);

                pathFollower.speed = curSpeed;

                if (curSpeed > 0.05f)
                {
                    if(animator) animator.SetBool("Run", true);
                }
                else
                {
                    if(animator) animator.SetBool("Run", false);
                }

                xPlayer = skeleton.GetJoint(JointType.Head).Real.X;
            }
        }
        else
        {
            curSpeed = 0f;
            targetSpeed = 0f;
            rep_count = 0;
            pathFollower.speed = 0f;
            if(animator) animator.SetBool("Run", false);
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
        foreach (var v in animals)
        {
            v.SetActive(false);
        }
        if(animals.Count > indexAnimal && animals[indexAnimal] != null)
        {
            animals[indexAnimal].SetActive(true);
            animator = animals[indexAnimal].GetComponent<Animator>();
        }
    }

    // 0. Stepping (Chạy tại chỗ - Tréo chân trục Z)
    public void Movement_Stepping(Skeleton skeleton)
    {
        float zLeftKnee = Mathf.Floor(skeleton.GetJoint(JointType.LeftKnee).Real.Z / 100f);
        float zRightKnee = Mathf.Floor(skeleton.GetJoint(JointType.RightKnee).Real.Z / 100f);

        if ((previous_z_left >= previous_z_right && zLeftKnee < zRightKnee) || 
            (previous_z_right >= previous_z_left && zLeftKnee > zRightKnee))
        {
            rep_count++;
        }

        previous_z_left = zLeftKnee;
        previous_z_right = zRightKnee;
    }

    // 1. Flapping (Vỗ tay lên xuống qua vai theo trục Y)
    public void Movement_Flapping(Skeleton skeleton)
    {
        float yLeftHand = Mathf.Floor(skeleton.GetJoint(JointType.LeftWrist).Real.Y / 100f);
        float yRightHand = Mathf.Floor(skeleton.GetJoint(JointType.RightWrist).Real.Y / 100f);
        float yShoulder = Mathf.Floor((skeleton.GetJoint(JointType.LeftShoulder).Real.Y + skeleton.GetJoint(JointType.RightShoulder).Real.Y) / 200f);

        bool isLeftUp = yLeftHand > yShoulder;
        bool isRightUp = yRightHand > yShoulder;
        bool wasLeftUp = previous_y_leftArm > yShoulder;
        bool wasRightUp = previous_y_rightArm > yShoulder;

        if ((!wasLeftUp && isLeftUp) || (!wasRightUp && isRightUp))
        {
            rep_count++;
        }

        previous_y_leftArm = yLeftHand;
        previous_y_rightArm = yRightHand;
    }

    // 2. Leaning (Nghiêng trái phải)
    public void Movement_Leanning(Skeleton skeleton)
    {
        float xLeftShoulder = skeleton.GetJoint(JointType.LeftShoulder).Real.X;
        float xRightShoulder = skeleton.GetJoint(JointType.RightShoulder).Real.X;
        float xLeftHip = skeleton.GetJoint(JointType.LeftHip).Real.X;
        float xRightHip = skeleton.GetJoint(JointType.RightHip).Real.X;

        float shoulderCenterX = Mathf.Floor((xLeftShoulder + xRightShoulder) / 2f / 100f);
        float hipCenterX = Mathf.Floor((xLeftHip + xRightHip) / 2f / 100f);
        
        float leanOffset = shoulderCenterX - hipCenterX;

        string currentLean = "center";
        if (leanOffset >= 1f) currentLean = "right";
        else if (leanOffset <= -1f) currentLean = "left";

        if (currentLean != "center" && currentLean != previous_lean)
        {
            rep_count++;
            previous_lean = currentLean;
        }
        else if (currentLean == "center" && currentLean != previous_lean)
        {
             previous_lean = currentLean; 
        }
    }

    // 3. Swimming (Vỗ tay VÀ đưa về phía trước - Bơi / Bò)
    public void Movement_Swimming(Skeleton skeleton)
    {
        float yLeftHand = Mathf.Floor(skeleton.GetJoint(JointType.LeftWrist).Real.Y / 100f);
        float yRightHand = Mathf.Floor(skeleton.GetJoint(JointType.RightWrist).Real.Y / 100f);
        float yShoulder = Mathf.Floor((skeleton.GetJoint(JointType.LeftShoulder).Real.Y + skeleton.GetJoint(JointType.RightShoulder).Real.Y) / 200f);

        float zLeftHand = skeleton.GetJoint(JointType.LeftWrist).Real.Z;
        float zLeftShoulder = skeleton.GetJoint(JointType.LeftShoulder).Real.Z;
        float zRightHand = skeleton.GetJoint(JointType.RightWrist).Real.Z;
        float zRightShoulder = skeleton.GetJoint(JointType.RightShoulder).Real.Z;

        bool isLeftUp = yLeftHand > yShoulder;
        bool isRightUp = yRightHand > yShoulder;
        bool wasLeftUp = previous_y_leftArm > yShoulder;
        bool wasRightUp = previous_y_rightArm > yShoulder;

        bool isLeftForward = zLeftHand < zLeftShoulder - 150f; 
        bool isRightForward = zRightHand < zRightShoulder - 150f;

        if (((!wasLeftUp && isLeftUp) && isLeftForward) || ((!wasRightUp && isRightUp) && isRightForward))
        {
            rep_count++;
        }

        previous_y_leftArm = yLeftHand;
        previous_y_rightArm = yRightHand;
    }

    // 4. Leanning Penguin (Nghiêng trái phải VÀ dang tay)
    public void Movement_LeanningPenguin(Skeleton skeleton)
    {
        float xLeftShoulder = skeleton.GetJoint(JointType.LeftShoulder).Real.X;
        float xRightShoulder = skeleton.GetJoint(JointType.RightShoulder).Real.X;
        float xLeftWrist = skeleton.GetJoint(JointType.LeftWrist).Real.X;
        float xRightWrist = skeleton.GetJoint(JointType.RightWrist).Real.X;
        float xLeftHip = skeleton.GetJoint(JointType.LeftHip).Real.X;
        float xRightHip = skeleton.GetJoint(JointType.RightHip).Real.X;

        float shoulderCenterX = Mathf.Floor((xLeftShoulder + xRightShoulder) / 2f / 100f);
        float hipCenterX = Mathf.Floor((xLeftHip + xRightHip) / 2f / 100f);
        float leanOffset = shoulderCenterX - hipCenterX;

        string currentLean = "center";
        if (leanOffset >= 1f) currentLean = "right";
        else if (leanOffset <= -1f) currentLean = "left";

        bool isArmsWide = Mathf.Abs(xLeftShoulder - xRightShoulder) + 200f < Mathf.Abs(xLeftWrist - xRightWrist);

        if (isArmsWide)
        {
            if (currentLean != "center" && currentLean != previous_lean)
            {
                rep_count++;
                previous_lean = currentLean;
            }
            else if (currentLean == "center" && currentLean != previous_lean)
            {
                previous_lean = currentLean;
            }
        }
    }

    // 5. Stepping with Hands Up (Chạy VÀ Giơ tay)
    public void Movement_SteppingAndUpTwoHand(Skeleton skeleton)
    {
        float zLeftKnee = Mathf.Floor(skeleton.GetJoint(JointType.LeftKnee).Real.Z / 100f);
        float zRightKnee = Mathf.Floor(skeleton.GetJoint(JointType.RightKnee).Real.Z / 100f);

        float yHead = skeleton.GetJoint(JointType.Head).Real.Y;
        float yLeftWrist = skeleton.GetJoint(JointType.LeftWrist).Real.Y;
        float yRightWrist = skeleton.GetJoint(JointType.RightWrist).Real.Y;

        bool areHandsUp = yLeftWrist > yHead && yRightWrist > yHead;

        if (areHandsUp)
        {
            if ((previous_z_left >= previous_z_right && zLeftKnee < zRightKnee) || 
                (previous_z_right >= previous_z_left && zLeftKnee > zRightKnee))
            {
                rep_count++;
            }
        }

        previous_z_left = zLeftKnee;
        previous_z_right = zRightKnee;
    }
}
