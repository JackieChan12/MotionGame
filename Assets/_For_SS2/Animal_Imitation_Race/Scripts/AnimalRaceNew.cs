using nuitrack;
using PathCreation.Examples;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalRaceNew : MonoBehaviour
{
    public int indexPlayer;

    public Transform character;
    public float walkSpeed = 2f;        
    public float characterSpeed = 0f;  
    
    [Header("Running Detection")]
    public int minStepsPerSecond = 1;
    public float speedLerpUp = 5f;
    public float speedLerpDown = 10f;
    private float speedUpdateTimer = 0f;
    private float speedUpdateInterval = 1f;
    private float targetSpeed = 0f;

    public bool startRun = false;
    public Animator animator;

    public UnityEngine.Vector3 beginPos;

    public float pointDistance = 0;
    public float pointPrePlayer = 0;

    public PathFollower pathFollower;
    public TMPro.TMP_Text textInstruction;
    public GameObject objInstruction;

    [Header("Test")]
    [SerializeField] bool isTest = false;
    
    float minMainZ = 2.5f, maxMainZ = 3.5f;
    public bool finish = false;

    [Header("Animal Race Config")]
    public int indexAnimal;
    [SerializeField] List<GameObject> animals = new List<GameObject>();
    
    // --- Animal Movement Tracking Variables ---
    private int rep_count = 0;
    private float previous_z_left = 0f;
    private float previous_z_right = 0f;
    private float previous_y_leftArm = 0f;
    private float previous_y_rightArm = 0f;
    private string previous_lean = "center";

    void Awake()
    {
        // Removed material randomization in favor of animal species randomization
    }

    private void Reset()
    {
        if (pathFollower == null) pathFollower = GetComponent<PathFollower>();
        if (character == null && transform.childCount > 0) character = transform.GetChild(0);

        if (animals == null || animals.Count == 0 || animals.Any(a => a == null))
        {
            animals = new List<GameObject>();
            if (character != null)
            {
                foreach (Transform child in character)
                {
                    animals.Add(child.gameObject);
                }
            }
        }

        if (textInstruction == null) textInstruction = GetComponentInChildren<TMPro.TMP_Text>();
        if (objInstruction == null) {
             var panel = transform.Find("InstructionPanel") ?? transform.Find("objInstruction") ?? transform.Find("InstructionBoard");
             if (panel != null) objInstruction = panel.gameObject;
        }
    }

    void Start()
    {
        //if(character) character.localRotation = Quaternion.Euler(0, 180, 0);
        
        rep_count = 0;
        beginPos = transform.position;
        
        if(pathFollower) pathFollower.speed = 0;
        if(animator) animator.Play("idle");
    }

    void Update()
    {
        if (finish && startRun)
        {
            pointDistance += 0.01f;
            pointDistance = Mathf.Ceil(pointDistance);
        }
        if (finish) return;

        if (isTest && startRun)
        {
            if(pathFollower) pathFollower.speed = .5f;
        }

        if(NuitrackManager.SkeletonTracker == null) return;
        List<Skeleton> userData = NuitrackManager.SkeletonTracker.GetSkeletonData().Skeletons.ToList();

        var sortedUsers = userData.OrderByDescending(user => user.GetJoint(nuitrack.JointType.Waist).Proj.X).ToList();
        sortedUsers = FilterSkeleton(sortedUsers);
        OnSkeletonUpdate(sortedUsers);

        if (startRun) 
        {
            if(pathFollower) pathFollower.speed = characterSpeed;
            pointDistance = Mathf.Ceil(pathFollower.distanceTravelled + pointPrePlayer);
        }
        else 
        {
            if(pathFollower) pathFollower.speed = 0;
        }
    }

    public List<Skeleton> FilterSkeleton(List<Skeleton> user)
    {
        List<Skeleton> newSkeleton = new List<Skeleton>();
        foreach (Skeleton s in user)
        {
            float z = s.GetJoint(JointType.Torso).Real.Z / 1000f;
            if (z >= minMainZ && z <= maxMainZ)
            {
                newSkeleton.Add(s);
            }
        }
        return newSkeleton;
    }

    public void StartRun()
    {
        startRun = true;
        beginPos = transform.position;
    }

    private void OnSkeletonUpdate(List<Skeleton> skeletonData)
    {
        if (skeletonData.Count > indexPlayer && skeletonData[indexPlayer] != null)
        {
            Skeleton skeleton = skeletonData[indexPlayer];

            // 6 Animals logic
            switch (indexAnimal)
            {
                case 0: Movement_Stepping(skeleton); break;
                case 1: Movement_Flapping(skeleton); break;
                case 2: Movement_Leanning(skeleton); break;
                case 3: Movement_Swimming(skeleton); break;
                case 4: Movement_LeanningPenguin(skeleton); break;
                case 5: Movement_SteppingAndUpTwoHand(skeleton); break;
            }

            speedUpdateTimer += Time.deltaTime;
            if (speedUpdateTimer >= speedUpdateInterval)
            {
                targetSpeed = (rep_count >= minStepsPerSecond) ? Mathf.Clamp(rep_count / 2f, 0f, 4f) : 0f;
                rep_count = 0;
                speedUpdateTimer = 0f;
            }

            if (!startRun)
            {
                characterSpeed = 0;
                rep_count = 0;
                return;
            }

            float lerpRate = (targetSpeed > characterSpeed) ? speedLerpUp : speedLerpDown;
            characterSpeed = Mathf.Lerp(characterSpeed, targetSpeed, Time.deltaTime * lerpRate);

            if (characterSpeed > 0.05f)
            {
                if(animator) animator.Play("run");
            }
            else
            {
                if(animator) animator.Play("idle");
            }
        }
        else
        {
            characterSpeed = 0;
            rep_count = 0;
            if(animator) animator.Play("idle");
        }
    }

    public void ChangePlayer()
    {
        if(character) character.localRotation = Quaternion.Euler(0, 180, 0);
        pointPrePlayer = pointDistance;
        startRun = false;
        finish = false;
        if(animator) animator.Play("idle");
        transform.position = beginPos;
        characterSpeed = 0;
        if(pathFollower) pathFollower.speed = characterSpeed / 2;
        if(pathFollower) pathFollower.distanceTravelled = 0;
    }

    // ---------------------------------------------------------
    // ANIMAL MOVEMENT LOGIC
    // ---------------------------------------------------------

    public void RandomAnimal(Difficulty d)
    {
        // Seed randomization uniquely for each player instance based on their index and time
        Random.InitState(System.DateTime.Now.Millisecond + (indexPlayer * 777));

        switch (d)
        {
            case Difficulty.Easy: indexAnimal = Random.Range(0, 2); break;
            case Difficulty.Normal: indexAnimal = Random.Range(2, 4); break;
            case Difficulty.Hard: indexAnimal = Random.Range(4, 6); break;
        }

        // Deactivate all animals first
        for (int i = 0; i < animals.Count; i++)
        {
            if (animals[i] != null)
                animals[i].SetActive(false);
        }

        // Activate the selected animal and get its animator
        if (indexAnimal >= 0 && indexAnimal < animals.Count && animals[indexAnimal] != null)
        {
            animals[indexAnimal].SetActive(true);
            animator = animals[indexAnimal].GetComponent<Animator>();
            character = animals[indexAnimal].transform; 
            
            // Set individual instruction
            if (textInstruction != null && Command.Instance != null && Command.Instance.CommandTutorialAnimalRace != null)
            {
                if (indexAnimal < Command.Instance.CommandTutorialAnimalRace.Length)
                {
                    textInstruction.text = Command.Instance.CommandTutorialAnimalRace[indexAnimal];
                }
            }
            if (objInstruction != null) objInstruction.SetActive(true);
        }
    }

    public void HideInstruction()
    {
        if (objInstruction != null) objInstruction.SetActive(false);
    }

    // 0. Stepping (Chạy tại chỗ - Tréo chân trục Z)
    void Movement_Stepping(Skeleton skeleton)
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
    void Movement_Flapping(Skeleton skeleton)
    {
        float yLeftHand = Mathf.Floor(skeleton.GetJoint(JointType.LeftWrist).Real.Y / 100f);
        float yRightHand = Mathf.Floor(skeleton.GetJoint(JointType.RightWrist).Real.Y / 100f);
        float yShoulder = Mathf.Floor((skeleton.GetJoint(JointType.LeftShoulder).Real.Y + skeleton.GetJoint(JointType.RightShoulder).Real.Y) / 200f);

        bool isLeftUp = yLeftHand > yShoulder;
        bool isRightUp = yRightHand > yShoulder;
        bool wasLeftUp = previous_y_leftArm > yShoulder;
        bool wasRightUp = previous_y_rightArm > yShoulder;

        if ((!wasLeftUp && isLeftUp) || (!wasRightUp && isRightUp)) rep_count++;

        previous_y_leftArm = yLeftHand;
        previous_y_rightArm = yRightHand;
    }

    // 2. Leaning (Nghiêng trái phải)
    void Movement_Leanning(Skeleton skeleton)
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

    // 3. Swimming (Vỗ tay VÀ đưa về phía trước)
    void Movement_Swimming(Skeleton skeleton)
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

        if (((!wasLeftUp && isLeftUp) && isLeftForward) || ((!wasRightUp && isRightUp) && isRightForward)) rep_count++;

        previous_y_leftArm = yLeftHand;
        previous_y_rightArm = yRightHand;
    }

    // 4. Leanning Penguin (Nghiêng trái phải VÀ dang tay rộng)
    void Movement_LeanningPenguin(Skeleton skeleton)
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

    // 5. Stepping with Hands Up
    void Movement_SteppingAndUpTwoHand(Skeleton skeleton)
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
