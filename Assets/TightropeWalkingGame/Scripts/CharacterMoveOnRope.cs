using DG.Tweening;
using nuitrack;
using PathCreation.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;


public class CharacterMoveOnRope : MonoBehaviour
{
    public int indexPlayer;

    public Transform character;
    public float walkSpeed = 2f;        // Tốc độ đi bộ tối thiểu
    public float characterSpeed = 0f;  // Tốc độ hiện tại của nhân vật
    public bool haveFall = false;
    public float previous_z_left = 0;
    public float previous_z_right = 0;
    public int step_count = 0;
    public float heightJump = 0;

    public float x_Hip = 0;

    public bool saveHeight = false;

    public bool countdownFirst = true;
    public float countDown = 5;
    public float WALK_THRESHOLD = 1.5f; // Steps per second threshold for walking
    public float balance_threshold = 15f;
    public float distance_ankle = 0;

    public bool startRun = false;
    public bool saveHeightTorso = false;
    public float defaultHeightTorso = 0;
    public Animator animator;
 
    public bool death = false;
    public bool joinDeath = false;
    public bool turnPlayer = false;

    public UnityEngine.Vector3 beginPos;


    public GameObject colliderFull;
    public GameObject colliderUnder;
    public GameObject colliderAbove;
    public ColliderController colliderControllerFull;
    public ColliderController colliderControllerUnder;
    public ColliderController colliderControllerAbove;

    public Transform _camera;
    public Transform _targetCamera;
    public Transform _finishCamera;

    Tween tweenCamMove;
    Tween tweenCamRotate;

    public UnityEngine.Vector3 posCamera;
    public Quaternion quaternionCamera;

    public AudioController audioController;
    public float pointDistance = 0;
    public float pointPrePlayer = 0;

    public PathFollower pathFollower;

    [Header("Test")]
    [SerializeField] bool isTest = false;
    [SerializeField] Image joint;
    [SerializeField] Vector2 posInCanvas;

    float minMainZ = 2.5f, maxMainZ = 3.5f;
    bool jumping = false; 
    public bool finish = false;

    [Obsolete]
    void Start()
    {
        //NuitrackManager.SkeletonTracker.OnSkeletonUpdateEvent += OnSkeletonUpdate;
        jumping = false;
        audioController = FindObjectOfType<AudioController>();
        character.DOLocalRotate(new UnityEngine.Vector3(0, 180, 0), 0.1f);
        previous_z_left = 0;
        previous_z_right = 0;
        step_count = 0;
        beginPos = transform.position;
        posCamera = _camera.position;
        quaternionCamera = _camera.rotation;
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

    [Obsolete]
    void Update()
    {
        if (finish && startRun)
        {
            pointDistance += 0.01f;
            pointDistance = Mathf.Ceil(pointDistance);
        }
        if (death || finish)
        {
            return;
        }

        if ( !finish && transform.position == pathFollower.pathCreator.path.GetPoint(pathFollower.pathCreator.path.NumPoints-1))
        {
            
            finish = true;
            animator.Play("Whirl jump");
        }

        if (isTest)
        {
            pathFollower.speed = 5;
            return;
        }

        List<Skeleton> userData = NuitrackManager.SkeletonTracker?.GetSkeletonData().Skeletons.ToList();

        var sortedUsers = userData.OrderByDescending(user => user.GetJoint(nuitrack.JointType.Waist).Proj.X).ToList();
        sortedUsers = FilterSkeleton(sortedUsers);
        OnSkeletonUpdate(sortedUsers);
        //transform.Translate(UnityEngine.Vector3.forward * characterSpeed * Time.deltaTime);
        if(startRun)pathFollower.speed = characterSpeed/2;
        if (turnPlayer)
        {
            character.DOLocalRotate(new UnityEngine.Vector3(0, 0, 0), 1f);
            tweenCamMove = _camera.DOMove(_targetCamera.position, 1);
            tweenCamRotate = _camera.DORotateQuaternion(_targetCamera.rotation, 1);
            turnPlayer = false;
            pointDistance = pointPrePlayer;
        }

        if (startRun)
        {
            pointDistance = Mathf.Ceil( UnityEngine.Vector3.Distance(transform.position, beginPos) + pointPrePlayer);
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
        characterSpeed = 0;
        Debug.Log("hjkfljhaskjfhhakjshfakf");
        audioController.PlaySplat();
        StartCoroutine(Restart());
    }

    public void StartRun()
    {
        saveHeightTorso = true;
        saveHeight = true;
        startRun = true;
        beginPos = transform.position;
    }


    [Obsolete]
    private void OnSkeletonUpdate(List<Skeleton> skeletonData)
    {
        if (skeletonData.Count > indexPlayer)
        {
            float zLeftKnee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.LeftKnee).ToVector3().z / 100);
            float zRightKnee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.RightKnee).ToVector3().z / 100);
            nuitrack.Joint j = skeletonData[indexPlayer].GetJoint(JointType.Torso);
            float heightTorso = (float)Math.Floor(j.ToVector3().y / 10);

            float xLeftHip = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.LeftHip).ToVector3().x / 10);
            float xRightHip = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.RightHip).ToVector3().x / 10);

            float xMidHip = (xLeftHip + xRightHip) / 2;

            float xLeftShoulder = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.LeftShoulder).ToVector3().x / 10);
            float xRightShoulder = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.RightShoulder).ToVector3().x / 10);

            float xMidShoulder = (xLeftShoulder + xRightShoulder) / 2;

            float YLeftknee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.LeftKnee).ToVector3().y);
            float YRightknee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.RightKnee).ToVector3().y);


            float XLeftknee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.LeftKnee).ToVector3().x);
            float XRightknee = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.RightKnee).ToVector3().x);

            if (!saveHeight)
            {
                float neck = (float)Math.Floor(skeletonData[indexPlayer].GetJoint(JointType.Neck).ToVector3().y / 10);
                heightJump = Math.Abs(neck - heightTorso) / 2;
            }
            float distance = 0;
            if ((previous_z_left >= previous_z_right && (zLeftKnee) < zRightKnee) || (previous_z_right >= previous_z_left && zLeftKnee > (zRightKnee)))
            {
                step_count += 1;
            }
            

            if (!saveHeightTorso)
            {
                defaultHeightTorso = heightTorso;
                x_Hip = xMidHip;
            }
            else
            {
                float distanceHip = xMidHip - x_Hip;
                float distanceHip_Shoulder = xMidHip - xMidShoulder;

                character.rotation = Quaternion.Euler(character.rotation.eulerAngles.x, character.rotation.eulerAngles.y, -distanceHip_Shoulder*2);
                
                if(/*distanceHip > 10 ||*/ Math.Abs(distanceHip_Shoulder) > balance_threshold && haveFall)
                {
                    Debug.Log("Losing Balance! Try to adjust.");
                    characterSpeed = 0;
                    StartCoroutine(Restart());
                    return;
                }
                distance = heightTorso - defaultHeightTorso;
                if (distance >= heightJump)
                {
                    if (characterSpeed == 0)
                    {
                        characterSpeed = walkSpeed;
                    }
                    characterSpeed = 2.5f;
                    ActiveCollider(false, true, false);
                    animator.Play(NameAnim.jump);
                    jumping = true;
                    Debug.LogWarning("JUMP");
                    return;
                }
                else if (distance <= -15f)
                {
                    if (characterSpeed == 0)
                    {
                        characterSpeed = walkSpeed;
                    }
                    characterSpeed = 2;
                    ActiveCollider(false, false, true);
                    animator.Play(NameAnim.slide);
                    Debug.LogWarning("SLIDE");
                    return;
                }
            }
            if (jumping) {
                characterSpeed = 2.5f;
                return;
            }
            if (!startRun)
            {
                characterSpeed = 0;
                step_count = 0;
                return;
            }
            if (Math.Abs(YLeftknee - YRightknee) >40)
            {
                characterSpeed = walkSpeed;
                ActiveCollider(true, false, false);
                animator.Play(NameAnim.walkRope);
                Debug.LogWarning("WalkRope");
            }
            else
            {
                if(Math.Abs(XLeftknee - XRightknee) > 200 && haveFall)
                {
                    Debug.Log("Losing Balance! Try to adjust.");
                    characterSpeed = 0;
                    StartCoroutine(Restart());
                    return;
                }
                else
                {
                    characterSpeed = 0;
                    animator.Play(NameAnim.idle);
                    ActiveCollider(true, false, false);
                    Debug.LogWarning("Stand");
                }
            }

        }
        else
        {
            characterSpeed = 0;
            step_count = 0;

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
        characterSpeed = 0;
        pathFollower.speed = characterSpeed/2;
        yield return new WaitForSeconds(1.8f);
        animator.Play(NameAnim.idle);
        death = false;
        startRun = true;
        pathFollower.distanceTravelled = 0;
        //_camera.DOMove(posCamera, 1);
        //_camera.DORotateQuaternion(quaternionCamera, 1);
        //transform.position = beginPos;
    }

    [Obsolete]
    public void ChangePlayer()
    {
        tweenCamMove.Kill();
        tweenCamRotate.Kill();

        character.DOLocalRotate(new UnityEngine.Vector3(0, 180, 0), 0.1f);
        pointPrePlayer = pointDistance;
        startRun = false;
        saveHeightTorso = false;
        death = false;
        finish = false;
        animator.Play(NameAnim.startAnim);
        transform.position = beginPos;
        characterSpeed = 0;
        pathFollower.speed = characterSpeed/2;
        pathFollower.distanceTravelled = 0;
        tweenCamMove = _camera.DOMove(posCamera, 1);
        tweenCamRotate = _camera.DORotateQuaternion(quaternionCamera, 1);
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
            
            if (clip.name == "Whirl jump")
            {
                AnimationEvent animationEvent = new AnimationEvent();
                animationEvent.time = clip.length;
                animationEvent.objectReferenceParameter = this;
                animationEvent.functionName = "EndWhirlJump";
                clip.AddEvent(animationEvent);
            }

        }
    }

    public void EndJump()
    {
        ActiveCollider(true, false, false);
        jumping = false;

    }

    public void EndSlide()
    {
        ActiveCollider(true, false, false);
    }

    public void EndWhirlJump()
    {
        animator.Play("Idle & waving");
        tweenCamMove = _camera.DOMove(_finishCamera.position, .5f);
        tweenCamRotate = _camera.DORotateQuaternion(_finishCamera.rotation, .5f);
        //character.DOLocalRotate(new UnityEngine.Vector3(0, 180, 0), 1f);
    }

    // Ceil ---------------------------------------------------------------
    
    [HideInInspector] int decimals = 2;
    [HideInInspector] float factor;
    [HideInInspector] float result;
    public float CeilNumber(float n)
    {
        decimals = 2;         
        factor = Mathf.Pow(10, decimals); 
        result = Mathf.Ceil(n * factor) / factor;

        return result;
    }

}

