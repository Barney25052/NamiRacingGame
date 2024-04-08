using UnityEngine;

// Draws a catmull-rom spline in the scene view,
// along the child objects of the transform of this component
public class CatmullRomSpline : MonoBehaviour
{
    [Range(0, 1)]
    public float alpha = 0.5f;

    [SerializeField]
    bool render;
    int PointCount => transform.childCount;
    int SegmentCount => PointCount - 3;
    Vector3 GetPoint(int i) => transform.GetChild(i).position;

    CatmullRomCurve GetCurve(int i)
    {
        return new CatmullRomCurve(GetPoint(i), GetPoint(i + 1), GetPoint(i + 2), GetPoint(i + 3), alpha);
    }

    void OnDrawGizmos()
    {
        if (!render)
        {
            return;
        }
        for (int i = 0; i < SegmentCount; i++)
            DrawCurveSegment(GetCurve(i));
    }

    void DrawCurveSegment(CatmullRomCurve curve)
    {
        const int detail = 32;
        Vector3 prev = curve.p1;

        for (int i = 1; i < detail; i++)
        {
            float t = i / (detail - 1f);
            Vector3 pt = curve.GetPoint(t);
            Gizmos.DrawLine(prev, pt);
            prev = pt;
        }
    }
}