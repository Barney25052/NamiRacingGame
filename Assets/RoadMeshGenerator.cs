using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadMeshGenerator : MonoBehaviour
{

    [SerializeField]
    MeshRenderer _meshRenderer;

    [SerializeField]
    MeshFilter _meshFilter;

    [SerializeField]
    Transform _points;

    [SerializeField]
    int resolution;

    [SerializeField]
    float depth = 0.2f;

    [SerializeField]
    bool straightDepth;

    [SerializeField]
    float bottomYPlane = -Mathf.Infinity;

    SplinePoint GetSplinePoint(int i) => _points.GetChild(i).GetComponent<SplinePoint>();

    void Start()
    {
        SplinePoint[] points = new SplinePoint[_points.childCount];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = GetSplinePoint(i);
        }

        Spline spline = new Spline(points, resolution);

        GenerateMesh(spline);
    }

    void GenerateMesh(Spline spline)
    {
        Mesh mesh = new Mesh
        {
            name = "Road"
        };

        int[] triangles = new int[(spline.Size) * 24];
        int vCount = 8;
        Vector3[] vertices = new Vector3[spline.Size * vCount];
        Vector3[] normals = new Vector3[spline.Size * vCount];

        for (int i = 0; i < spline.Size; i++)
        {
            RoadPoint p = spline.GetRoadPoint(i);
            Vector3 up = straightDepth ? Vector3.up : p.Up;
 
            vertices[8 * i + 0] = p.point - p.Right * (p.width / 2);
            vertices[8 * i + 1] = p.point + p.Right * (p.width / 2);
            vertices[8 * i + 2] = p.point - (p.Right * (p.width / 2)) - (up * depth);
            vertices[8 * i + 3] = p.point + (p.Right * (p.width / 2)) - (up * depth);

            vertices[8 * i + 2].y = Mathf.Clamp(vertices[8 * i + 2].y, bottomYPlane, vertices[8 * i + 2].y);
            vertices[8 * i + 3].y = Mathf.Clamp(vertices[8 * i + 3].y, bottomYPlane, vertices[8 * i + 3].y);


            vertices[8 * i + 4] = vertices[8 * i + 0];
            vertices[8 * i + 5] = vertices[8 * i + 1];
            vertices[8 * i + 6] = vertices[8 * i + 2];
            vertices[8 * i + 7] = vertices[8 * i + 3]; 

            normals[8 * i + 0] = p.Up;
            normals[8 * i + 1] = p.Up;
            normals[8 * i + 2] = -p.Up;
            normals[8 * i + 3] = -p.Up;

            normals[8 * i + 4] = -p.Right;
            normals[8 * i + 5] = p.Right;
            normals[8 * i + 6] = -p.Right;
            normals[8 * i + 7] = p.Right;
        }

        for(int i = 0; i < spline.Size-1;i++)
        {
            //TOP
            triangles[24 * i + 0] = 0 + i * vCount;
            triangles[24 * i + 1] = 8 + i * vCount;
            triangles[24 * i + 2] = 1 + i * vCount;

            triangles[24 * i + 3] = 1 + i * vCount;
            triangles[24 * i + 4] = 8 + i * vCount;
            triangles[24 * i + 5] = 9 + i * vCount;

            //BOTTOM
            triangles[24 * i + 6] = 2 + i * vCount;
            triangles[24 * i + 7] = 3 + i * vCount;
            triangles[24 * i + 8] = 10 + i * vCount;

            triangles[24 * i + 9] = 10 + i * vCount;
            triangles[24 * i + 10] = 3 + i * vCount;
            triangles[24 * i + 11] = 11 + i * vCount;

            //LEFT
            triangles[24 * i + 12] = 6 + i * vCount;
            triangles[24 * i + 13] = 14 + i * vCount;
            triangles[24 * i + 14] = 4 + i * vCount;

            triangles[24 * i + 15] = 14 + i * vCount;
            triangles[24 * i + 16] = 12 + i * vCount;
            triangles[24 * i + 17] = 4 + i * vCount;

            //RIGHT
            triangles[24 * i + 18] = 13 + i * vCount;
            triangles[24 * i + 19] = 15 + i * vCount;
            triangles[24 * i + 20] = 7 + i * vCount;

            triangles[24 * i + 21] = 13 + i * vCount;
            triangles[24 * i + 22] = 7 + i * vCount;
            triangles[24 * i + 23] = 5 + i * vCount;
        }

        int s = 24 * (spline.Size - 1);
        int v = vertices.Length - 16;

        //TOP
        triangles[s + 0] = 1;
        triangles[s + 1] = 8 + v;
        triangles[s + 2] = 0;

        triangles[s + 3] = 9 + v;
        triangles[s + 4] = 8 + v;
        triangles[s + 5] = 1;

        //BOTTOM
        triangles[s + 6] = 10 + v ;
        triangles[s + 7] = 3;
        triangles[s + 8] = 2;

        triangles[s + 9] = 11 + v;
        triangles[s + 10] = 3;
        triangles[s + 11] = 10 + v;

        //LEFT
        triangles[s + 12] = 4;
        triangles[s + 13] = 14 + v;
        triangles[s + 14] = 6;

        triangles[s + 15] = 4;
        triangles[s + 16] = 12 + v;
        triangles[s + 17] = 14 + v;

        //RIGHT
        triangles[s + 18] = 7;
        triangles[s + 19] = 15 + v;
        triangles[s + 20] = 13 + v;

        triangles[s + 21] = 5;
        triangles[s + 22] = 7;
        triangles[s + 23] = 13 + v;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        _meshFilter.mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        
    }

}
