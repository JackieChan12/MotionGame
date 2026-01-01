using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public int indexBall;
    public float speed = 10f;
    private Rigidbody rb;
    public Vector3 ballStartPos;

    public delegate void onGoalEvent(int indexGoal, int indexB);
    public onGoalEvent onGoal;
    public delegate void onNumberEvent(int number, int indexB);
    public onNumberEvent onNumber;
    bool isStart = false;
    AudioController audioController;
    public bool isTable0 =false;
    public Difficulty difficulty;
    Vector3 touchPoint = Vector3.zero;

    void Start()
    {
        audioController = FindObjectOfType<AudioController>();  
        ballStartPos = transform.position;
        rb = GetComponent<Rigidbody>();
        //StartBall();
    }

    public void StartBall()
    {
        isStart = true;

        rb.velocity = new Vector3(indexBall == 0? speed : -speed, 0, Random.Range(-1f, 1f));
    }

    void FixedUpdate()
    {
        if (!isStart) return;
        if (rb.velocity.magnitude < speed)
        {
            rb.velocity = (rb.velocity.normalized ) * speed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.name.Contains("Goal") && difficulty != Difficulty.Easy)
        {
            audioController.PlaySplatDone();
        }
        if (collision.gameObject.name.Contains("Wall") || collision.gameObject.name.Contains("Cube"))
        {
            audioController.PlaySplat();
        }
        if (collision.gameObject.name.Contains("Cube"))
        {
            touchPoint = transform.position;
        }

        
        if (collision.gameObject.name.Contains("Goal"))
        {
            if (collision.gameObject.name.Contains("1")){
                onGoal?.Invoke(1, indexBall);
            }
            else
            {
                onGoal?.Invoke(2, indexBall);
            }
            StartCoroutine(OnGoalAndWait());
            return;
        }
        if (collision.gameObject.tag == "Number")
        {
            int i = 0;
            switch (collision.gameObject.name){
                case "1":
                    i = 1;
                    break;
                case "2":
                    i = 2;
                    break;
                case "3":
                    i = 3;
                    break;
                case "4":
                    i = 4;
                    break;
                case "5":
                    i = 5;
                    break;
                case "6":
                    i = 6;
                    break;
                case "7":
                    i = 7;
                    break;
                case "8":
                    i = 8;
                    break;
                case "9":
                    i = 9;
                    break;
            }
            onNumber?.Invoke(i, indexBall);
            //audioController.PlaySplat();
            
        }

    }

    IEnumerator OnGoalAndWait()
    {
        ResetBall();
        yield return new WaitForSeconds(2);
        StartBall();
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.LogWarning("STAY");
        AdjustToCenter(touchPoint, 10);
        //ContactPoint contact = collision.contacts[0];
        //Vector3 normal = contact.normal;
        //Vector3 reflectedVelocity = Vector3.Reflect(rb.velocity, normal);

        ////if (Vector3.Angle(rb.velocity, reflectedVelocity) < 5f)
        ////{
        //    reflectedVelocity -= new Vector3(0, 0, 0.5f);
        ////}
        //rb.velocity = reflectedVelocity.normalized * speed;
    }

    public void ResetBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = ballStartPos;
        isStart = false;
    }

    void AdjustToCenter(Vector3 centerPosition, float adjustmentAngle)
    {
        centerPosition.x = -centerPosition.x;

        Vector3 directionToCenter = (centerPosition - transform.position).normalized;

        float radians = adjustmentAngle * Mathf.Deg2Rad;

        Vector3 adjustedDirection = Vector3.RotateTowards(rb.velocity.normalized, directionToCenter, radians, 0.0f);

        rb.velocity = adjustedDirection * rb.velocity.magnitude;
    }
}
