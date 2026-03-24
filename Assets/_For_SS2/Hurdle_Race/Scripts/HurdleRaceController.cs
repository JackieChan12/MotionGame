using nuitrack;
using PathCreation;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HurdleRaceController : MonoBehaviour
{
    public int indexPlayer = 0;

    public List<Material> materials = new List<Material>();
    public SkinnedMeshRenderer skinnedMeshRenderer;

    public float xPlayer;

    [SerializeField] PathCreator creatorPath;
    [SerializeField] PathFollower pathFollower;

    public TMP_Text textPoint;
    public Camera cam;
    public bool startGame = false;
    public bool isDead = false;
    public bool isJump = false;
    public float curSpeed = 0;
    public int indexMovement = 0;

    public float point;
    public Animator animator;

    // ─── Running Detection ────────────────────────────────────────────
    [Header("Running Detection")]
    [Tooltip("Số bước tối thiểu trong 1 giây để nhân vật chạy (dưới mức này = đứng yên)")]
    public int minStepsPerSecond = 2;
    [Tooltip("Tốc độ tăng speed (lerp)")]
    public float speedLerpUp = 5f;
    [Tooltip("Tốc độ giảm speed (lerp) — nên nhanh hơn tăng")]
    public float speedLerpDown = 10f;

    private int   stepCount        = 0;
    private float speedUpdateTimer = 0f;
    private float speedUpdateInterval = 1f;
    private float previous_z_left  = 0f;
    private float previous_z_right = 0f;

    // ─── Jump Detection ───────────────────────────────────────────────
    [Header("Jump Detection")]
    private float defaultHeightTorso = 0f;
    private float heightJump       = 0f;
    private bool  isInitialized    = false;

    // ─── Misc ─────────────────────────────────────────────────────────
    float minMainZ = 2.5f, maxMainZ = 3.5f;

    // ─────────────────────────────────────────────────────────────────

    void Awake()
    {
        Material[] mats = skinnedMeshRenderer.materials;
        mats[0] = materials[UnityEngine.Random.Range(0, materials.Count)];
        skinnedMeshRenderer.materials = mats;
    }

    void Update()
    {
        if (isJump) return;
        if (isDead) return;

        textPoint.text = point.ToString("N0");
        point = pathFollower.distanceTravelled;

        if (startGame)
        {
            List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();

            if (userData == null || userData.Count <= indexPlayer)
            {
                pathFollower.speed = 0;
                animator.Play("idle");
                return;
            }

            Skeleton skeleton = userData[indexPlayer];

            // 1. Kiểm tra nhảy trước (ưu tiên cao hơn chạy)
            if (DetectJump(skeleton))
            {
                curSpeed = 1f;
                animator.Play("jump");
                StartCoroutine(OnJump());
            }
            else
            {
                // 2. Nhận diện chạy / đứng yên
                DetectRunning(skeleton);

                if (curSpeed > 0.05f)
                    animator.Play("run");
                else
                    animator.Play("idle");
            }

            pathFollower.speed = curSpeed;

            xPlayer = skeleton.GetJoint(JointType.Head).Real.X;
        }
        else
        {
            pathFollower.speed = 0;
            animator.Play("idle");
        }
    }

    // ──────────────────────────────────────────────────────────────────
    // NHẢY: Dựa vào logic HurdleRaceNew (Tính từ khoảng cách cổ đến bụng)
    // ──────────────────────────────────────────────────────────────────
    bool DetectJump(Skeleton skeleton)
    {
        float neckY = Mathf.Floor(skeleton.GetJoint(JointType.Neck).Real.Y / 10f);
        float torsoY = Mathf.Floor(skeleton.GetJoint(JointType.Torso).Real.Y / 10f);

        if (!isInitialized)
        {
            defaultHeightTorso = torsoY;
            heightJump = Mathf.Abs(neckY - torsoY) / 2f;
            
            previous_z_left = Mathf.Floor(skeleton.GetJoint(JointType.LeftKnee).Real.Z / 100f);
            previous_z_right = Mathf.Floor(skeleton.GetJoint(JointType.RightKnee).Real.Z / 100f);

            isInitialized = true;
            return false;
        }

        float distance = torsoY - defaultHeightTorso;
        
        return distance >= heightJump;
    }

    // ──────────────────────────────────────────────────────────────────
    // CHẠY: Thay đổi từ việc check Y sang check Z theo HurdleRaceNew
    // ──────────────────────────────────────────────────────────────────
    void DetectRunning(Skeleton skeleton)
    {
        float zLeftKnee = Mathf.Floor(skeleton.GetJoint(JointType.LeftKnee).Real.Z / 100f);
        float zRightKnee = Mathf.Floor(skeleton.GetJoint(JointType.RightKnee).Real.Z / 100f);

        if ((previous_z_left >= previous_z_right && zLeftKnee < zRightKnee) || 
            (previous_z_right >= previous_z_left && zLeftKnee > zRightKnee))
        {
            stepCount++;
        }

        previous_z_left = zLeftKnee;
        previous_z_right = zRightKnee;

        // ── Lớp 2: Đủ số bước tối thiểu mới tính là chạy ─────────────
        speedUpdateTimer += Time.deltaTime;
        if (speedUpdateTimer >= speedUpdateInterval)
        {
            float targetSpeed = (stepCount >= minStepsPerSecond)
                                ? Mathf.Clamp(stepCount / 2f, 0f, 2f)
                                : 0f;

            // ── Lớp 3: Lerp mượt (không dừng/tăng đột ngột) ──────────
            float lerpRate = (targetSpeed > curSpeed) ? speedLerpUp : speedLerpDown;
            curSpeed = Mathf.Lerp(curSpeed, targetSpeed, Time.deltaTime * lerpRate);

            stepCount        = 0;
            speedUpdateTimer = 0f;
        }
    }

    // ──────────────────────────────────────────────────────────────────
    // LỌC SKELETON theo khoảng cách Z (dùng khi cần)
    // ──────────────────────────────────────────────────────────────────
    public List<Skeleton> FilterSkeleton(List<Skeleton> user)
    {
        List<Skeleton> newSkeleton = new List<Skeleton>();
        foreach (Skeleton s in user)
        {
            float z = s.GetJoint(JointType.Torso).Real.Z / 1000f;
            if (z >= minMainZ && z <= maxMainZ)
                newSkeleton.Add(s);
        }
        return newSkeleton;
    }

    // ──────────────────────────────────────────────────────────────────
    // VA CHẠM VỚI CHƯỚNG NGẠI VẬT (Layer 13)
    // ──────────────────────────────────────────────────────────────────
    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead && !isJump && collision.gameObject.layer == 13)
            StartCoroutine(OnObstacle());
    }

    IEnumerator OnObstacle()
    {
        curSpeed              = 0;
        pathFollower.speed    = 0;
        isDead                = true;
        animator.Play("death");
        yield return new WaitForSeconds(3.5f);
        pathFollower.distanceTravelled = 0;
        isDead = false;
        animator.Play("idle");
    }

    // ──────────────────────────────────────────────────────────────────
    // NHẢY COROUTINE
    // ──────────────────────────────────────────────────────────────────
    IEnumerator OnJump()
    {
        isJump = true;
        yield return new WaitForSeconds(0.9f);
        isJump = false;
        animator.Play("idle");
    }
}
