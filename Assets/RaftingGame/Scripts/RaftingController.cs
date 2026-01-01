using DG.Tweening;
using nuitrack;
using PathCreation.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaftingController : MonoBehaviour
{
    public int indexPlayer;

    public Transform character;
    [SerializeField] RectTransform baseRect;
    [SerializeField] RectTransform imageRect;
    public float walkSpeed = 2f;        // Tốc độ đi bộ tối thiểu
    public float maxRunSpeed = 10f;     // Tốc độ chạy tối đa
    public float characterSpeed = 0f;  // Tốc độ hiện tại của nhân vật

    public float previous_z_left = 0;
    public float previous_z_right = 0;
    public int step_count = 0;
    public float start_time = 0;
    public float heightJump = 0;
    public float midPoint = 0;
    public float distanceToMidDefault = 25;
    public bool saveHeight = false;

    public bool countdownFirst = true;
    public float countDown = 5;
    public float WALK_THRESHOLD = 1.5f; // Steps per second threshold for walking
    public float RUN_THRESHOLD = 2.0f;   // Steps per second threshold for running
    public float RUN_THRESHOLD_EventDown = 2.0f;   // Steps per second threshold for running

    public bool startRun = false;
    public bool saveHeightTorso = false;
    public float defaultHeightTorso = 0;
    public Animator animator;
    public float distanceLand = 1.4f;
    public bool death = false;
    public bool joinDeath = false;
    public bool turnPlayer = false;
    public int land = 0;
    public float midLandPlayer;
    public float leftLandPlayer;
    public float rightLandPlayer;

    public float minX = 0, maxX = 0;
    public UnityEngine.Vector3 landPos;
    public UnityEngine.Vector3 beginPos;


    public GameObject colliderFull;
    public GameObject colliderUnder;
    public GameObject colliderAbove;
    public ColliderController colliderControllerFull;
    public ColliderController colliderControllerUnder;
    public ColliderController colliderControllerAbove;

    public RiverMapController roadController;
    public AudioController audioController;
    public float pointDistance = 0;
    public float pointPrePlayer = 0;

    [Header("\nCheck Canvas")]
    float defaultz = 0;
    public ResizeImageCanvas resizeCanvas;
    public Mode mode;
    public Difficulty difficulty;
    public float minMove, maxMove;
    public float minMoveCanvas, maxMoveCanvas, midCanvas;
    public float ratio;
    public RectTransform mainCanvas;
    public RectTransform areaPlayer;
    public Camera cameraPlayer;
    UnityEngine.Vector3 position;
    public GameObject NoticeWarning;
    bool isSpeedDown = false;
    float addpoint = 0;

    [SerializeField] TMP_Text addPointText;
    [SerializeField] Transform addPointObj;

    [Header("Test")]
    [SerializeField] bool isTest = false;
    [SerializeField] UnityEngine.UI.Image joint;
    [SerializeField] Vector2 posInCanvas;

    float minMainZ = 2.5f, maxMainZ = 3.5f;
    bool jumping = false;
    public bool finish = false;
    public ShowPoint showPointAdd;

    [Header("\nAudioClip")]
    [SerializeField] AudioClip clipEat;
    [SerializeField] AudioClip clipBubble;

    public void PrepareStart()
    {
        defaultz = transform.localPosition.z;
        resizeCanvas.Resize();
        CaculateCanvas();
        
    }


    void CaculateCanvas()
    {
        if (mode == Mode.Scenario)
        {
            areaPlayer.sizeDelta = new Vector2(areaPlayer.sizeDelta.x / 2, areaPlayer.sizeDelta.y);
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

        ratio = Mathf.Round((Mathf.Abs(maxMove - minMove) / Mathf.Abs(maxMoveCanvas - minMoveCanvas)) * 1000000) / 1000000;
        //ratio = Mathf.Abs(maxMove - minMove) / Mathf.Abs(maxMoveCanvas - minMoveCanvas);
    }

    [Obsolete]
    void Start()
    {
        //NuitrackManager.SkeletonTracker.OnSkeletonUpdateEvent += OnSkeletonUpdate;
        audioController = FindObjectOfType<AudioController>();
        character.DOLocalRotate(new UnityEngine.Vector3(0, 180, 0), 0.1f);
        previous_z_left = 0;
        previous_z_right = 0;
        step_count = 0;
        start_time = Time.time;
        beginPos = transform.position;
        midLandPlayer = transform.position.x;
        leftLandPlayer = transform.position.x - distanceLand;
        rightLandPlayer = transform.position.x + distanceLand;

        colliderControllerAbove.onTrigger = OnTriggerCustom;
        colliderControllerFull.onTrigger = OnTriggerCustom;
        colliderControllerUnder.onTrigger = OnTriggerCustom;

        colliderControllerAbove.onTriggerAddpoint = OnTriggerAddPoint;
        colliderControllerFull.onTriggerAddpoint = OnTriggerAddPoint;
        colliderControllerUnder.onTriggerAddpoint = OnTriggerAddPoint;

        colliderControllerAbove.onTriggerSpeed = OnTriggerEnventSpeedCustom;
        colliderControllerFull.onTriggerSpeed = OnTriggerEnventSpeedCustom;
        colliderControllerUnder.onTriggerSpeed = OnTriggerEnventSpeedCustom;

        AddJumpEvent();
        ActiveCollider(true, false, false);

    }

    public void ActiveCollider(bool full, bool above, bool under)
    {
        colliderFull.SetActive(full);
        colliderUnder.SetActive(under);
        colliderAbove.SetActive(above);
    }

    [Obsolete]
    void Update()
    {
        if (death)
        {
            return;
        }

        if(transform.position.z > 542.1f && transform.position.z < 642.1f)
        {
            isSpeedDown = true;
        }
        else
        {
            isSpeedDown = false;
        }
        NoticeWarning.SetActive(isSpeedDown);

        List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();

        var sortedUsers = userData.OrderByDescending(user => user.GetJoint(nuitrack.JointType.Waist).Proj.X).ToList();
        sortedUsers = FilterSkeleton(sortedUsers);
        OnSkeletonUpdate(sortedUsers);
        transform.Translate(UnityEngine.Vector3.forward * characterSpeed * Time.deltaTime);
        if (turnPlayer)
        {
            character.DOLocalRotate(new UnityEngine.Vector3(0, 0, 0), 1f);
            turnPlayer = false;
            pointDistance = pointPrePlayer;
        }
        landPos = transform.position;

        if (startRun)
        {
            pointDistance = Mathf.Ceil(transform.position.z-beginPos.z + pointPrePlayer + addpoint);
        }
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

    [Obsolete]
    public void OnTriggerCustom()
    {
        audioController?.PlaySplat();
        StartCoroutine(Restart());
    }
    public void OnTriggerAddPoint(float p)
    {
        audioController?.PlayAddPoint(p == 5 ? clipEat : clipBubble);
        showPointAdd.m_Text.text = $"+{p}";
        showPointAdd.gameObject.SetActive(true);
        addpoint += p;
    }
    public void OnTriggerEnventSpeedCustom(bool isSPDown)
    {
        isSpeedDown = isSPDown;
        NoticeWarning.SetActive(isSpeedDown);
    }

    public void StartRun()
    {
        saveHeightTorso = true;
        saveHeight = true;
        startRun = true;
        start_time = Time.time;
    }


    [Obsolete]
    private void OnSkeletonUpdate(List<Skeleton> skeletonData)
    {
        if (skeletonData.Count > indexPlayer)
        {
            //Debug.Log("JOINT");
            nuitrack.Joint j = skeletonData[indexPlayer].GetJoint(JointType.Torso);
            float xCanvas = AnchoredPosition(j.Proj, baseRect.rect, imageRect).x;
            if (xCanvas > maxMoveCanvas || xCanvas < minMoveCanvas) return;
            position = transform.localPosition;
            position.x = -(xCanvas - midCanvas) * ratio + defaultz;
            //Debug.Log("X Canvas " + position);
            transform.localPosition = position;
        }
        if (skeletonData.Count > indexPlayer)
        {
            //knee is wirst

            //float zLeftKnee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.LeftWrist).ToVector3().z / 100);
            //float zRightKnee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.RightWrist).ToVector3().z / 100);
            float zLeftKnee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.LeftWrist).ToVector3().y / 100);
            float zRightKnee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.RightWrist).ToVector3().y / 100);
            float yLeftKnee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.LeftWrist).ToVector3().y / 10);
            float yRightKnee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.RightWrist).ToVector3().y / 10);
            nuitrack.Joint j = skeletonData[indexPlayer].GetJoint(JointType.Torso);
            float heightTorso = (float)Math.Floor(j.ToVector3().y / 10);
            float XTorso = (float)Math.Floor(j.ToVector3().x / 10);
            float neck = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.Neck).ToVector3().y / 10);
            if (!saveHeight)
            {
                heightJump = Math.Abs(neck - heightTorso) / 2;
                midPoint = XTorso;
            }
            //posInCanvas = AnchoredPosition(j.Proj, baseRect.rect, imageRect);
            //joint.rectTransform.anchoredPosition = posInCanvas;
            float midBody = (heightTorso + neck )/2;
            if (((previous_z_left >= previous_z_right && (zLeftKnee) < zRightKnee) || (previous_z_right >= previous_z_left && zLeftKnee > (zRightKnee))) /*&& (yLeftKnee > midBody && yRightKnee > midBody)*/)
            {
                step_count += 1;
            }
            previous_z_left = zLeftKnee;
            previous_z_right = zRightKnee;

            
            if (!saveHeightTorso)
            {
                defaultHeightTorso = heightTorso;
            }
            else
            {
                float distance = heightTorso - defaultHeightTorso;
                Debug.Log("Distance : " + distance);
                if (distance >= heightJump)
                {
                    if (characterSpeed == 0)
                    {
                        characterSpeed = walkSpeed;
                    }
                    ActiveCollider(false, true, false);
                    JumpCustom();
                    animator.Play(NameAnim.jump);
                    Debug.LogWarning("JUMP");
                    start_time = Time.time;
                }
                else if (distance <= -15f)
                {
                    if (characterSpeed == 0)
                    {
                        characterSpeed = walkSpeed;
                    }
                    ActiveCollider(false, false, true);
                    //SlideCustom();
                    character.DOLocalMoveY(-1f, .5f)
                            .SetEase(Ease.OutQuad);
                    animator.Play("Swim");
                    Debug.LogWarning("SLIDE");
                    start_time = Time.time;
                }
                else
                {
                    character.DOLocalMoveY(-0.3f, .5f)
                                                                .SetEase(Ease.OutQuad);
                }
            }
            if (!startRun)
            {
                characterSpeed = 0;
                step_count = 0;
                start_time = Time.time;
                return;
            }
            float elapsed_time = Time.time - start_time;
            if (elapsed_time >= 1f)
            {
                float stepPerSecond = step_count / elapsed_time;
                Debug.LogError($"Step per second : {stepPerSecond}");
                if (stepPerSecond > 0 && stepPerSecond < RUN_THRESHOLD)
                {
                    characterSpeed = walkSpeed;
                    ActiveCollider(true, false, false);
                    animator.Play(NameAnim.walk);
                    Debug.LogWarning("Walk");
                }
                else if (stepPerSecond >= RUN_THRESHOLD)
                {
                    characterSpeed = maxRunSpeed;
                    ActiveCollider(true, false, false);
                    animator.Play(NameAnim.run);
                    Debug.LogWarning("Run");
                }
                else
                {
                    characterSpeed = 0;
                    animator.Play("Idle_A");
                    ActiveCollider(true, false, false);
                    Debug.LogWarning("Stand");
                }
                if (isSpeedDown)
                {
                    if(stepPerSecond >= RUN_THRESHOLD_EventDown)
                    {
                        characterSpeed = maxRunSpeed;
                    }
                    else
                    {
                        characterSpeed -= 2;
                    }
                }
                step_count = 0;
                start_time = Time.time;
            }
            
        }
        else
        {
            characterSpeed = 0;
            step_count = 0;
            start_time = Time.time;

            animator.Play("Idle_A");
            ActiveCollider(true, false, false);
            Debug.LogWarning("Stand");
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
        death = true;
        startRun = false;
        animator.Play("Death");
        yield return new WaitForSeconds(1.8f);
        animator.Play("Idle_A");
        death = false;
        startRun = true;
        start_time = Time.time;
        transform.position = beginPos;
        characterSpeed = 0;
        roadController.Restart();
    }

    [Obsolete]
    public void ChangePlayer()
    {
        character.DOLocalRotate(new UnityEngine.Vector3(0, 180, 0), 0.1f);
        pointPrePlayer = pointDistance;
        addpoint = 0;
        startRun = false;
        saveHeightTorso = false;
        death = false;
        animator.Play("Idle_B");
        transform.position = beginPos;
        roadController.Restart();
    }


    void AddJumpEvent()
    {

        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Jump")
            {
                AnimationEvent animationEvent = new AnimationEvent();
                animationEvent.time = clip.length;
                animationEvent.objectReferenceParameter = this;
                animationEvent.functionName = "EndJump";
                clip.AddEvent(animationEvent);
            }

            if (clip.name == "Swim")
            {
                AnimationEvent animationEvent = new AnimationEvent();
                animationEvent.time = clip.length;
                animationEvent.objectReferenceParameter = this;
                animationEvent.functionName = "EndSlide";
                clip.AddEvent(animationEvent);
            }

        }
    }
    void JumpCustom()
    {

        UnityEngine.Vector3 originalLocalPosition = character.localPosition;


        character.DOLocalMoveY(1f, 1f)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() =>
                 {
                     character.DOLocalMoveY(-0.3f, .6f)
                              .SetEase(Ease.InQuad);
                 });
    }
    void SlideCustom()
    {

        UnityEngine.Vector3 originalLocalPosition = character.localPosition;


        character.DOLocalMoveY(-1f, 1f)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() =>
                 {
                     character.DOLocalMoveY(-.3f, .6f)
                              .SetEase(Ease.InQuad);
                 });
    }

    public void EndJump()
    {
        ActiveCollider(true, false, false);
    }

    public void EndSlide()
    {
        ActiveCollider(true, false, false);
    }

}
