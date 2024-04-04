using UnityEngine;
public class RoadPoint
{
    public Vector3 point;
    public float rotation;
    public float width;

    private Vector3 forward;
    private Vector3 right;
    private Vector3 up;

    public Vector3 Forward { get => forward; }
    public Vector3 Right { get => right; }
    public Vector3 Up { get => up; }

    public RoadPoint(Vector3 point, float rotation, float width)
    {
        this.point = point;
        this.rotation = rotation;
        this.width = width;
    }

    public void SetDirection(Vector3 nextPoint)
    {
        this.forward = (nextPoint - point).normalized;
        Vector3 flat = forward;
        flat.y = 0;

        float angle = Mathf.Acos(Vector3.Dot(flat, forward) / (flat.magnitude + forward.magnitude));
        Vector3 proxyRight = Vector3.Cross(flat, Vector3.up);

        this.up = Quaternion.AngleAxis(90, proxyRight) * this.forward;
        this.up = Quaternion.AngleAxis(rotation, this.forward) * this.up;
        this.right = Vector3.Cross(up, forward);

    }

}
