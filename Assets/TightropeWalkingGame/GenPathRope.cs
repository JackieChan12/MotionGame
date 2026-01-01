using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenPathRope : MonoBehaviour
{
    public bool closedLoop = true;
    public Transform[] waypoints;

    public Transform ST;
    public Transform END;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            // Create a new bezier path from the waypoints.
            BezierPath bezierPath = new BezierPath(waypoints, closedLoop, PathSpace.xyz);
            bezierPath.GlobalNormalsAngle = 90;
            PathCreator pathCr = GetComponent<PathCreator>();
            pathCr.bezierPath = bezierPath;
        }
    }
}
