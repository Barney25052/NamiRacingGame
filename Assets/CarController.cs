using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _carEngine;

    [SerializeField]
    private Transform _carModel;

    [SerializeField]
    private float _maxSpeed;

    [SerializeField]
    private AnimationCurve _acceleration;
    [SerializeField]
    private float _accelMult;

    [SerializeField]
    private float _engineDropoff;
    [SerializeField]
    private float _lateralDrag;
    [SerializeField]
    private AnimationCurve _drag;

    [SerializeField]
    private AnimationCurve _lateralDamping;

    [SerializeField]
    private float _currentSpeed;

    [SerializeField]
    private LayerMask _carLayer;

    [SerializeField]
    private Transform[] _wheels;

    int turnPerFrame = 0;
    int maxTurns = 30;

    private void FixedUpdate()
    {
        float accelerationInput = Input.GetKey(KeyCode.W) ? 1 : 0;
        float brakingInput = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Space) ? 1 : 0;

        float lateralVelocity = Vector3.Dot(_carModel.right, _carEngine.velocity);
        float forwardVelocity = Vector3.Dot(_carModel.forward, _carEngine.velocity);
        float lateralPercent = lateralVelocity / (_carEngine.velocity.magnitude+0.05f);
        lateralPercent = lateralPercent <= 0.5f || lateralPercent == float.NaN ? 0 : lateralPercent;

        float maxSpeedPercent = 1-(_currentSpeed / _maxSpeed);

        float potentialAccel = maxSpeedPercent * _acceleration.Evaluate(1-maxSpeedPercent) * _accelMult;
        float accel = potentialAccel * accelerationInput * Time.fixedDeltaTime;

        _currentSpeed += accel;

        //Add new acceleration
        _carEngine.AddForce(_currentSpeed * _carModel.forward, ForceMode.Acceleration);
        //Remove lateral velocity by percentage
        Vector3 reverseVel = -lateralVelocity * _carModel.right;
        if(forwardVelocity < 0)
        {
            reverseVel += -forwardVelocity * _carModel.forward;
        }
        _carEngine.AddForce(reverseVel * _lateralDamping.Evaluate(1-maxSpeedPercent), ForceMode.Impulse);

        if (accelerationInput == 0)
        {
            _currentSpeed -= _engineDropoff * Time.fixedDeltaTime;
            //Add drag to car 
            Vector3 velocityToReduce = _carEngine.velocity;
            velocityToReduce.y = 0;
            _carEngine.AddForce(-velocityToReduce * _drag.Evaluate(_currentSpeed/_maxSpeed), ForceMode.Acceleration);
        }

        if(brakingInput > 0)
        {
            _carEngine.AddForce(-_carEngine.velocity, ForceMode.Acceleration);
            _currentSpeed *= 0.5f;
        }
        _carModel.Rotate(_carModel.transform.up, Input.GetAxis("Horizontal") * 90 * Time.fixedDeltaTime);
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            turnPerFrame += 1;
            turnPerFrame = Mathf.Min(maxTurns, turnPerFrame);
        } else
        {
            turnPerFrame -= 1;
            turnPerFrame = Mathf.Max(turnPerFrame, 0);
        }
        _currentSpeed -=  (1 - maxSpeedPercent) * (turnPerFrame/maxTurns) * _lateralDrag * Time.fixedDeltaTime;
        _currentSpeed = Mathf.Max(0, _currentSpeed);

        _carModel.transform.position = _carEngine.position;
    }

    private void Update()
    {

        Vector3 normal = Vector3.zero;
        Vector3 halfDepth = Vector3.forward * _carModel.localScale.z / 2;
        Vector3 halfWidth = Vector3.forward * _carModel.localScale.x / 2;
        RaycastHit hit;
        for (int i = 0; i < 2; i++)
        {
            int mult = -1 + i * 2;
            Physics.Raycast(_carModel.position + halfDepth * mult, _carModel.up * -1, out hit, 1.25f, ~_carLayer);
            normal += hit.normal;
        }
        for (int i = 0; i < 2; i++)
        {
            int mult = -1 + i * 2;
            Physics.Raycast(_carModel.position + halfWidth * mult, _carModel.up * -1, out hit, 1.25f, ~_carLayer);
            normal += hit.normal;
        }

        normal /= 4;
        if(normal != Vector3.zero)
        {
            _carModel.rotation = Quaternion.Lerp(_carModel.rotation, Quaternion.FromToRotation(_carModel.up, normal) * _carModel.rotation, 0.4f);
        }

        foreach(Transform wheel in _wheels)
        {
            int mult = -1;
            if(wheel.transform.localPosition.x < 0)
            {
                mult = 1;
            }
            wheel.transform.Rotate(Vector3.up, _currentSpeed * Time.deltaTime * 150 * mult);
        }
    }
}