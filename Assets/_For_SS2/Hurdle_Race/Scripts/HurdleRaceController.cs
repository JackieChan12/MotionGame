using nuitrack;
using PathCreation;
using PathCreation.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class HurdleRaceController : MonoBehaviour
{
    public int indexPlayer = 0;

    public List<Material> materials = new List<Material>();
    public SkinnedMeshRenderer skinnedMeshRenderer;

    public float xPlayer;
    private float prevBodyHeight = 0.0f; 
    private bool initialized = false; 
    private const float jumpThreshold = 0.15f; 
    private const float crouchThreshold = 0.15f;

    [SerializeField] PathCreator creatorPath;
    [SerializeField] PathFollower pathFollower;

    public TMP_Text textPoint;
    public Camera cam;
    public bool startGame = false;
    public bool isDead = false;
    public bool isJump = false;
    public float curSpeed = 0;
    public int indexMovement = 0;

    private int stepCount = 0;
    private float speedUpdateInterval = 1f;

    public float point;
    public Animator animator;
    float minMainZ = 2.5f, maxMainZ = 3.5f;
    bool isPreLeft = false, isPreRight = false;
    int detectAction = 0; //1: jump, 2: crouch, 3: run
    void Awake()
    {
        Material[] mats = skinnedMeshRenderer.materials;
        mats[0] = materials[UnityEngine.Random.Range(0, materials.Count)];
        skinnedMeshRenderer.materials = mats;
    }

    // Update is called once per frame 
    void Update()
    {
        if (isJump) return;
        if (isDead) return;
        textPoint.text = point.ToString("N0");
        point = pathFollower.distanceTravelled;
        if (startGame) {
            List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();
            userData = FilterSkeleton(userData);

            detectAction = DetectAction(userData.Count > 0 ? userData[indexPlayer] : null);
            if (detectAction == 1) // jump
            {
                curSpeed = 1f;
                animator.Play("jump");
                StartCoroutine(OnJump());
            } else if (detectAction == 2) // crouch
              {
                //animator.SetTrigger("Crouch");
            } else {
                Movement_Stepping(userData[indexPlayer]);
            }

            pathFollower.speed = curSpeed;
            if (curSpeed > 0) {
                animator.Play("run");
            } else {
                animator.Play("idle");
            }
            xPlayer = NuitrackManager.SkeletonTracker != null ? NuitrackManager.SkeletonTracker.GetSkeletonData().Skeletons[indexPlayer].GetJoint(JointType.Head).Real.X : 0;

        } else {
            pathFollower.speed = 0;
            animator.Play("idle");
        }
    }
    public int DetectAction(Skeleton skeleton) { 
        if (skeleton == null) return 0; 
        float headY = skeleton.Joints[(int)JointType.Head].Real.Y; 
        float leftFootY = skeleton.Joints[(int)JointType.LeftFoot].Real.Y; 
        float rightFootY = skeleton.Joints[(int)JointType.RightFoot].Real.Y; 
        float bodyHeight = headY - Math.Min(leftFootY, rightFootY); 
        if (!initialized) { 
            prevBodyHeight = bodyHeight; 
            initialized = true; return 0; 
        } 
        float deltaHeight = bodyHeight - prevBodyHeight; 
        if (deltaHeight > jumpThreshold) { 
            return 1; // Nhảy
        } else if (deltaHeight < -crouchThreshold) { 
            return 2; // Cúi xuống
        }
        prevBodyHeight = bodyHeight; 
        return 3;
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

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.layer == 13) {
            StartCoroutine(OnObstacle());
        }
    }
    IEnumerator OnObstacle()
    {
        curSpeed = 0;
        pathFollower.speed = 0;
        isDead = true;
        animator.Play("death");
        yield return new WaitForSeconds(3.5f);
        pathFollower.distanceTravelled = 0;
        isDead = false;
        animator.Play("idle");
    }

    IEnumerator OnJump() {
        isJump = true;
        yield return new WaitForSeconds(0.9f);
        isJump = false;
        animator.Play("idle");
    }
}
