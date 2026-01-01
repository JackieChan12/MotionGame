using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPaddleController : MonoBehaviour
{
    public Transform ball;
    public float speed = 5f;
    public float allowdistance;
    public int direct = 1;
    public bool isWall = false;
    void Update()
    {
        if (!isWall)
        {
            if (transform.position.z - ball.position.z > allowdistance)
                transform.Translate(Vector3.forward * speed * Time.deltaTime * direct);
            else if (ball.position.z - transform.position.z > allowdistance)
                transform.Translate(Vector3.back * speed * Time.deltaTime * direct);
        }
        else
        {
            if (transform.position.z - ball.position.z > allowdistance)
                transform.Translate(Vector3.left * speed * Time.deltaTime * direct);
            else if (ball.position.z - transform.position.z > allowdistance)
                transform.Translate(Vector3.left * speed * Time.deltaTime * direct);
        }
        
    }
}
