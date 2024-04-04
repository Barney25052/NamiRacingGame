using JetBrains.Annotations;
using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
public class Spline
{
    public SplinePoint[] controlPoints;
    private RoadPoint[] roadPoints;
    private int resolution;
    private int size;
    
    public int Size { get { return size; } }

    public Spline(SplinePoint[] controlPoints, int resolution)
    {
        this.controlPoints = controlPoints;
        Array.Resize(ref this.controlPoints, this.controlPoints.Length + 2);
        this.controlPoints[this.controlPoints.Length - 2] = controlPoints[0];
        this.controlPoints[this.controlPoints.Length - 1] = controlPoints[1];
        this.resolution = resolution;
        this.size = resolution * (this.controlPoints.Length-2);
        
        GeneratePoints();
    }

    void GeneratePoints()
    {
        roadPoints = new RoadPoint[size];
        for(int i = 0; i < size; i++)
        {
            roadPoints[i] = CalculatePoint(i);
        }
        RoadPoint extra = CalculatePoint(size);

        for (int i = 0; i < size-1; i++)
        {
            roadPoints[i].SetDirection(roadPoints[i + 1].point);
        }
        roadPoints[size-1].SetDirection(extra.point);
    }

    public RoadPoint GetRoadPoint(int i)
    {
        return roadPoints[i];
    }

    RoadPoint CalculatePoint (int point)
    {
        int segmentIndex = Mathf.FloorToInt(point / resolution);
        int partIndex = point - (segmentIndex * resolution);
        float t = (float)partIndex/resolution; 

        Vector3 position = CalculatePosition(segmentIndex, partIndex);
        float rotation = controlPoints[segmentIndex + 1].rotation;
        if (Mathf.Abs(controlPoints[segmentIndex].rotation - controlPoints[segmentIndex + 1].rotation) != 360)
        {
            rotation = Mathf.Lerp(controlPoints[segmentIndex].rotation, controlPoints[segmentIndex + 1].rotation, t);
        }
        float width = Mathf.Lerp(controlPoints[segmentIndex].width, controlPoints[segmentIndex + 1].width, t);

        RoadPoint roadPoint = new RoadPoint(position, rotation, width);
        return roadPoint;
    }

    Vector3 CalculatePosition(int segmentIndex, int partIndex)
    {
        Vector3 p0 = segmentIndex == 0 ? controlPoints[0].point : controlPoints[segmentIndex-1].point;
        Vector3 p1 = controlPoints[segmentIndex].point;
        Vector3 p2 = controlPoints[segmentIndex + 1].point;
        Vector3 p3 = segmentIndex == controlPoints.Length-2 ? controlPoints[segmentIndex+1].point : controlPoints[segmentIndex + 2].point;

        float t = (float)partIndex / resolution;
        float tt = t * t;
        float ttt = tt * t;

        return  p1 + 
                (0.5f * t) * (p2 - p0) +
                (0.5f * tt) * (2*p0 - 5*p1 + 4*p2 - p3) +
                (0.5f * ttt) * (-p0 + 3*p1 - 3*p2 + p3);
        
    }
}