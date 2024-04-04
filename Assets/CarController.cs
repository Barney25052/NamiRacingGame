using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    Rigidbody _rb;

    [SerializeField]
    Transform _body;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.AddForce(Input.GetAxisRaw("Vertical") * _body.forward * 100);
        _body.transform.Rotate(_body.transform.up, Input.GetAxis("Horizontal") * 45 * Time.fixedDeltaTime);
        _body.transform.position = transform.position;
    }
}
