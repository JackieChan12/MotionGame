using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObjectInRope : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform parent;
    public GameObject box;
    public float distance;
    public Transform ST;
    public Transform END;
    [System.Obsolete]
    void Start()
    {
        distance = Vector3.Distance(ST.position, END.position);
        for(int i=0; i<lineRenderer.numPositions; i++)
        {
            Instantiate(box, lineRenderer.GetPosition(i), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
