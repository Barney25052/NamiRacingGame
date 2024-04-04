using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class BezierCurveGenerator
{
    public static Vector3[] GenerateBezierCurve(Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint, int resolution)
    {
        Vector3[] finalPoints = new Vector3[resolution];

        float step = 1 / (float)(resolution - 1);
        for(int i = 0; i < resolution; i++)
        {
            finalPoints[i] = CalculatePoint(startPoint, controlPoint, endPoint, step * i);
        }

        return finalPoints;
    }

    static Vector3 CalculatePoint(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float tt = t * t;
        Vector3 value = ((1 -2*t + tt) * a) + (2*t - 2*tt)*b + tt*c;
        Debug.Log(a + " + " + b + " + " + c + " at " + t + ": " + value);
        return value;
    }

    public struct RoadPoint
    {
        public Vector3 point;
        public  Quaternion rotation;
    }

    public static RoadPoint[] GenerateRoadPoints(Transform a, Transform b, int resolution)
    {
        RoadPoint[] finalPoints = new RoadPoint[resolution];

        float step = 1 / (float)(resolution - 1);
        for (int i = 0; i < resolution; i++)
        {
            RoadPoint point = new RoadPoint();
            point.point = Vector3.Lerp(a.position, b.position, step * i);
            point.rotation = Quaternion.Lerp(a.rotation, b.rotation, step * i);
            finalPoints[i] = point;
        }

        return finalPoints;
    }
}
