using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveInTime : MonoBehaviour
{
    public float speed = 2f; // Tốc độ di chuyển của object
    public float duration = 2f; // Thời gian di chuyển tính bằng phút
    public Transform spawner;
    public Transform target;
    public float timeElapsed = 0f;
    public bool stMove = false;
    public PathFollower pathFollower;
    private void Start()
    {
        transform.position = spawner.position;
        transform.LookAt(target);
    }

    void Update()
    {
        if (!stMove) return;
        pathFollower.enabled = true;
        if (timeElapsed < duration * 60) // Chuyển đổi phút sang giây
        {
            //transform.Translate(Vector3.forward * speed * Time.deltaTime); // Di chuyển object về phía trước
            timeElapsed += Time.deltaTime;
        }
    }
}
