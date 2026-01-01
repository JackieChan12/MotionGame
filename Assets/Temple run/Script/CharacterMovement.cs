using DG.Tweening;
using nuitrack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static ColliderController;

public class NameAnim
{
    public const string startAnim = "Idle & waving";
    public const string idle = "Idle";
    public const string run = "Run";
    public const string walk = "Walk";
    public const string jump = "Jump";
    public const string slide = "Skidding";
    public const string death = "Dizzy";
    public const string walkRope = "WalkRope";
}

public class CharacterMovement : MonoBehaviour
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
    public bool saveHeight= false;

    public bool countdownFirst = true;
    public float countDown = 5;
    public float WALK_THRESHOLD = 1.5f; // Steps per second threshold for walking
    public float RUN_THRESHOLD = 2.0f;   // Steps per second threshold for running

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

    public RoadController roadController;
    public AudioController audioController;
    public float pointDistance = 0;
    public float pointPrePlayer = 0;

    [Header("Test")]
    [SerializeField] Image joint;
    [SerializeField] Vector2 posInCanvas;

    float minMainZ = 2.5f, maxMainZ = 3.5f;
    float minMainX = -1.0f, maxMainX = 1.0f;

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
        AddJumpEvent();
        ActiveCollider(true, false, false);
    }

    public void ActiveCollider(bool full, bool above, bool under)
    {
        colliderFull.SetActive(full);
        colliderUnder.SetActive(under);
        colliderAbove.SetActive(above);
    }

    void Update()
    {
        if (death)
        {
            return;
        }
        
        List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();

        var sortedUsers = userData.OrderByDescending(user => user.GetJoint(nuitrack.JointType.Waist).Proj.X).ToList();
        //sortedUsers = FilterSkeleton(sortedUsers);
        OnSkeletonUpdate(sortedUsers);
        transform.Translate(UnityEngine.Vector3.forward * characterSpeed * Time.deltaTime);
        if (turnPlayer)
        {
            character.DOLocalRotate(new UnityEngine.Vector3(0, 0, 0), 1f);
            turnPlayer = false;
            pointDistance = pointPrePlayer;
        }
        landPos = transform.position;
        switch (land)
        {
            case 0:
                landPos.x = midLandPlayer;
                break;
            case 1:
                landPos.x = rightLandPlayer;
                break;
            case -1:
                landPos.x = leftLandPlayer;
                break;
        }
        transform.position = UnityEngine.Vector3.Lerp(transform.position, landPos, Time.deltaTime*10);

        if (startRun)
        {
            pointDistance = Mathf.Ceil(UnityEngine.Vector3.Distance(transform.position, beginPos) + pointPrePlayer);
        }

        //if (countdownFirst)
        //{
        //    countDown -= Time.deltaTime;
        //    if (countDown <= 0)
        //    {
        //        //audioController?.PlayAudioStartGame();
        //        countdownFirst = false;
        //        countDown = 5;
        //        StartRun();
        //    }
        //    return;
        //}
    }

    public List<Skeleton> FilterSkeleton(List<Skeleton> user)
    {
        List<Skeleton> newSkeleton = new List<Skeleton>();

        foreach(Skeleton s in user)
        {
            float z = s.GetJoint(JointType.Torso).Real.Z/1000;
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
        audioController.PlaySplat();
        StartCoroutine(Restart());
    }

    public void StartRun()
    {
        saveHeightTorso = true;
        saveHeight = true;
        startRun = true;
        start_time = Time.time;
    }

    private void OnSkeletonUpdate(List<Skeleton> skeletonData)
    {
        if (skeletonData.Count > indexPlayer)
        {
            float zLeftKnee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.LeftKnee).ToVector3().z / 100);
            float zRightKnee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.RightKnee).ToVector3().z / 100);
            nuitrack.Joint j = skeletonData[indexPlayer].GetJoint(JointType.Torso);
            float heightTorso = (float)Math.Floor(j.ToVector3().y / 10);
            float XTorso = (float)Math.Floor(j.ToVector3().x / 10);
            if (!saveHeight)
            {
                float neck = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.Neck).ToVector3().y / 10);
                heightJump = Math.Abs(neck - heightTorso)/2;
                midPoint = XTorso;
            }
            posInCanvas = AnchoredPosition(j.Proj, baseRect.rect, imageRect);
            //joint.rectTransform.anchoredPosition = posInCanvas;
            float distance = 0;
            if ((previous_z_left >= previous_z_right && (zLeftKnee) < zRightKnee) || (previous_z_right >= previous_z_left && zLeftKnee > (zRightKnee)))
            {
                step_count += 1;
            }
            previous_z_left = zLeftKnee;
            previous_z_right = zRightKnee;

            float posX = posInCanvas.x;
            float distanceMidPoint = midPoint - XTorso;
            //if (posX < maxX && posX > minX)
            //{
            //    land = 0;
            //}
            //else if (posX < minX)
            //{
            //    land = 1;
            //}
            //else if (posX > maxX)
            //{
            //    land = -1;
            //}
            if(Math.Abs(distanceMidPoint) < distanceToMidDefault)
            {
                land = 0;
            }
            else if(distanceMidPoint > distanceToMidDefault)
            {
                land = 1;
            }
            else if(distanceMidPoint < -distanceToMidDefault)
            {
                land = -1;
            }

            if (!saveHeightTorso)
            {
                defaultHeightTorso = heightTorso;
            }
            else
            {
                distance = heightTorso - defaultHeightTorso;
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
                    animator.Play(NameAnim.slide);
                    Debug.LogWarning("SLIDE");
                    start_time = Time.time;
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
                //if (distance >= 7f)
                //{
                //    if(characterSpeed == 0)
                //    {
                //        characterSpeed = walkSpeed;
                //    }
                //    animator.Play(NameAnim.jump);
                //    Debug.LogWarning("JUMP");
                //}
                //else if (distance <= -7f)
                //{
                //    if (characterSpeed == 0)
                //    {
                //        characterSpeed = walkSpeed;
                //    }
                //    animator.Play(NameAnim.slide);
                //    Debug.LogWarning("SLIDE");
                //}
                //else
                if (stepPerSecond > 0 && stepPerSecond < RUN_THRESHOLD)
                {
                    characterSpeed = walkSpeed;
                    ActiveCollider(true, false, false);
                    animator.Play(NameAnim.walk);
                    Debug.LogWarning("Walk");
                }
                else if(stepPerSecond >= RUN_THRESHOLD)
                {
                    characterSpeed = maxRunSpeed;
                    ActiveCollider(true, false, false);
                    animator.Play(NameAnim.run);
                    Debug.LogWarning("Run");
                }
                else
                {
                    characterSpeed = 0;
                    animator.Play(NameAnim.idle);
                    ActiveCollider(true, false, false);
                    Debug.LogWarning("Stand");
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

            animator.Play(NameAnim.idle);
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
        animator.Play(NameAnim.death);
        yield return new WaitForSeconds(1.8f);

        animator.Play(NameAnim.idle);
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
        startRun = false;
        saveHeightTorso = false;
        death= false;
        animator.Play(NameAnim.startAnim);
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

            if (clip.name == "Skidding")
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

        
        character.DOLocalMoveY(.5f, .6f)
                 .SetEase(Ease.OutQuad) 
                 .OnComplete(() =>
                 {
                     character.DOLocalMoveY(0, .6f)
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

