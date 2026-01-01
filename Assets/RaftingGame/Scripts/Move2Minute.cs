using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2Minute : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.forward;
    public float speed = 5f;

    private float startTime;
    private bool isMoving = true;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (isMoving)
        {
            // Di chuyển object
            transform.Translate(moveDirection * speed * Time.deltaTime);

            // Kiểm tra nếu đã qua 2 phút (120 giây)
            if (Time.time - startTime >= 200f)
            {
                isMoving = false;
            }
        }
    }
}
