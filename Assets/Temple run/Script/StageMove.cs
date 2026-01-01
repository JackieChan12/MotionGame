using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class StageMove : MonoBehaviour
{
    public float speed = 5.0f;
    public bool isMoving = false;
    public float zEnd;
    public delegate void ToEndEvent();
    public ToEndEvent toEnd;
    private Rigidbody rb;

    public List<GameObject> listObstacle= new List<GameObject>();

    float distance = 0;
    float z;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        //GenerateObstacle();
    }
    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            if(speed > 0)
            {
                transform.Translate(Vector3.back * speed * Time.deltaTime);

                Vector3 roundedPosition = new Vector3(
                    Mathf.Round(transform.position.x * 1000f) / 1000f,
                    Mathf.Round(transform.position.y * 1000f) / 1000f,
                    Mathf.Round(transform.position.z * 1000f) / 1000f
                );
                transform.position = roundedPosition;
            }
            
            if (zEnd >= transform.localPosition.z)
            {
                toEnd?.Invoke();
                isMoving = false;
                Destroy(gameObject);
            }
        }
    }

    public void SetupStage(float s, float z)
    {
        speed = s;
        zEnd = z;
        isMoving = true;
    }

    public void GenerateObstacle(int startRan = 0, int endRan = 12, int numObstacle = 2)
    {
        int countObstacle = numObstacle;//Random.Range(2, 3);
        distance = 18 / (countObstacle + 1);
        for(int i = 0; i < countObstacle; i++)
        {
            z = (i + 1) * distance - 9;
            Transform trans = Instantiate(listObstacle[Random.Range(startRan, endRan)], transform).transform;
            trans.gameObject.SetActive(true);
            Vector3 pos = Vector3.zero;
            pos.z = z;
            trans.localPosition = pos;
        }
    }
}
