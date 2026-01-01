using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StatusAnim
{
    public const string run = "Run";
    public const string jump = "Jump";
    public const string slide = "Slide";
    public const string death = "Death";
}

public class A_Player : MonoBehaviour
{
    public RoadController roadController;
    public Transform character;
    public Animator animator;
    public GameObject colliderFull;
    public GameObject colliderUnder;
    public GameObject colliderAbove;
    public ColliderController colliderControllerFull;
    public ColliderController colliderControllerUnder;
    public ColliderController colliderControllerAbove;

    public float distanceLand = 1.4f;
    public float speedRoad = 0;
    public bool startRun = false;

    public bool liftLeft = false;
    public bool liftRight = false;

    [Header("Point")]
    public float pointDistance = 0;
    float pointPrePlayer = 0;
    public Vector3 targetPosition;
    public float speed = 5f;
    bool canChangeLand = true;
    bool canJump = true;
    bool canSlide = true;
    Vector3 newPos;
    float constX = 0;
    int land = 0;
    bool turnPlayer = false;
    Quaternion quaternion;
    private Tween myTween;
    private Coroutine myCoroutine;

    [System.Obsolete]
    void Start()
    {
        colliderControllerAbove.onTrigger = OnTriggerCustom;
        colliderControllerFull.onTrigger = OnTriggerCustom;
        colliderControllerUnder.onTrigger = OnTriggerCustom;
        constX = transform.localPosition.x;
        AddJumpEvent();
        targetPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (startRun)
        {
            if (!turnPlayer)
            {
                character.DOLocalRotate(new Vector3(0, 0, 0), 1f);
                //roadController.AcceptMove();
                turnPlayer = true;
                pointDistance = pointPrePlayer;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Slide();
            }
            if (canChangeLand)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    ToRight();
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    ToLeft();
                }
            }
            //roadController.SetStageSpeed(speedRoad);
        }
    }

    void AddJumpEvent()
    {
        
        foreach(var clip in animator.runtimeAnimatorController.animationClips)
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

            if (clip.name == "Dizzy")
            {
                AnimationEvent animationEvent = new AnimationEvent();
                animationEvent.time = clip.length;
                animationEvent.objectReferenceParameter = this;
                animationEvent.functionName = "EndDeath";
                clip.AddEvent(animationEvent);
            }

        }
    }

    public void ToLeft()
    {
        newPos = transform.localPosition;
        if (land <= -1) return;
        newPos.x -= distanceLand;
        land -= 1;
        canChangeLand = false;
        transform.DOLocalMoveX(newPos.x, 0.1f).OnComplete(() =>
        {
            canChangeLand = true;
        });
    }

    public void ToRight()
    {
        newPos = transform.localPosition;
        if (land >= 1) return;
        newPos.x += distanceLand;
        land++;
        canChangeLand = false;
        transform.DOLocalMoveX(newPos.x, 0.01f).OnComplete(() =>
        {
            canChangeLand = true;
        });
    }

    public void ToLandLeft()
    {
        if (myTween != null && myTween.IsActive())
        {
            myTween.Kill();
        }
        newPos = new Vector3(constX-distanceLand,0,0);
        myTween = transform.DOLocalMoveX(newPos.x, 0.01f);
    }
    public void ToLandMid()
    {
        if (myTween != null && myTween.IsActive())
        {
            myTween.Kill();
        }
        newPos = new Vector3(constX, 0, 0);
        myTween = transform.DOLocalMoveX(newPos.x, 0.01f);
    }
    public void ToLandRight()
    {
        if (myTween != null && myTween.IsActive())
        {
            myTween.Kill();
        }
        newPos = new Vector3(constX+distanceLand, 0, 0);
        myTween = transform.DOLocalMoveX(newPos.x, 0.01f);
    }


    public void EndJump()
    {
        animator.SetBool(StatusAnim.jump, false);
        colliderAbove?.SetActive(false);
        colliderFull?.SetActive(true);
        canJump = true;
    }

    public void EndSlide()
    {
        animator.SetBool(StatusAnim.slide, false);
        colliderUnder?.SetActive(false);
        colliderFull?.SetActive(true);
        canSlide = true;
    }

    public void EndDeath()
    {
        animator.SetBool(StatusAnim.run, startRun);
        animator.SetBool(StatusAnim.slide, false);
        animator.SetBool(StatusAnim.jump, false);
        animator.SetBool(StatusAnim.death, false);

        colliderFull?.SetActive(true);
        colliderUnder?.SetActive(false);
        colliderAbove?.SetActive(false);
    }

    public void Run()
    {
        animator.SetBool(StatusAnim.run, true);
        animator.SetBool(StatusAnim.slide, false);
        animator.SetBool(StatusAnim.death, false);
        animator.SetBool(StatusAnim.jump, false);

        colliderFull?.SetActive(true);
        colliderUnder?.SetActive(false);
        colliderAbove?.SetActive(false);
    }

    public void Jump()
    {
        if (canJump)
        {
            canJump = false;
            canSlide = true;
            animator.Play("Jump",0,0f);
            colliderFull?.SetActive(false);
            colliderAbove?.SetActive(true);
            animator.SetBool(StatusAnim.jump, true);
            animator.SetBool(StatusAnim.slide, false);
        }
    }
    public void Slide()
    {
        if (canSlide)
        {
            canSlide = false;
            canJump = true;
            animator.Play("Skidding", 0, 0f);
            colliderFull?.SetActive(false);
            colliderUnder?.SetActive(true);
            animator.SetBool(StatusAnim.slide, true);
            animator.SetBool(StatusAnim.jump, false);
        }
    }

    [System.Obsolete]
    public void OnTriggerCustom()
    {
       // myCoroutine = StartCoroutine(Restart());
    }

    [System.Obsolete]
    public IEnumerator Restart()
    {
        startRun = false;
        animator.SetBool(StatusAnim.death, true);
        roadController.Pause();
        yield return new WaitForSeconds(1.8f);
        pointDistance = 0;
        roadController.Restart();
        startRun = true;
        animator.SetBool(StatusAnim.run, true);
        roadController.AcceptMove();
    }

    [System.Obsolete]
    public void NextPlayerInTeam()
    {
        StopMyCoroutine();
        pointPrePlayer = pointDistance;
        character.DOLocalRotate(new Vector3(0, 180, 0), 1f);
        turnPlayer = false;
        startRun = false;
        animator.SetBool(StatusAnim.run, false);
        roadController.Pause();
        roadController.Restart();
    }
    void StopMyCoroutine()
    {
        if (myCoroutine != null)
        {
            StopCoroutine(myCoroutine);
            myCoroutine = null; 
        }
    }
}
