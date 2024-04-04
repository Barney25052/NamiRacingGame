using System.ComponentModel;
using UnityEngine;

public class SplinePoint : MonoBehaviour
{
    [ReadOnly(true)]
    public Vector3 point => transform.position;
    public float rotation;
    public float width = 1;

}