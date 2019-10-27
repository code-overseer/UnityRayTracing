using System;
using System.Collections;
using UnityEngine;
using System.Collections;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = GetComponent<Camera>().transform;
    }

    private void Update()
    {
        MoveLongitudinal(Input.GetAxis("Vertical") * SpeedScale);
        MoveLateral(Input.GetAxis("Horizontal") * SpeedScale);
        Rotate(Input.GetAxis("Rotate"));

        Zoom(Input.GetAxis("Mouse ScrollWheel") * SpeedScale);
        //FPS mouse hold click
        if (Input.GetMouseButton(0))
        {
            Pitch(Input.GetAxis("Mouse Y"));
            Yaw(Input.GetAxis("Mouse X"));
            ClampPitch();
        }
        // Bounds
        ClampVertical();
        
    }

    private float SpeedScale => 2 * transform.position.y + 1;

    #region Movement Implementation

    private void MoveLongitudinal(float input)
    {
        var positiveDirection = Vector3.Cross(_cameraTransform.right, Vector3.up).normalized;

        transform.position += input * Time.unscaledDeltaTime * positiveDirection;
    }

    private void MoveLateral(float input)
    {

        transform.position += input * Time.unscaledDeltaTime * _cameraTransform.right;
    }
    
    private void Zoom(in float input)
    {
        var front = _cameraTransform.forward;
        // Cannot zoom when facing up
        if (front.y < 0)
        {
            transform.position += input * Time.unscaledDeltaTime * front;
        }
    }

    private void Pitch(in float input)
    {
        transform.Rotate(input, 0, 0);
    }

    private void Yaw(in float input)
    {
        transform.Rotate(0, input, 0, Space.World);
    }

    private void Rotate(in float input)
    {
        var pos = transform.position;
        var forward = _cameraTransform.forward;
        pos -= forward * pos.y / forward.y;
        transform.RotateAround(pos, Vector3.up, input);
    }

    private void ClampVertical()
    {
        var position = transform.position;
        position.y = Mathf.Clamp(position.y, 0, 10);
        transform.position = position;
    }

    private void ClampPitch()
    {
        const float lowerAngle = -35;
        const float upperAngle = 90;
        var front = Vector3.Cross(_cameraTransform.right, Vector3.up).normalized;
        if (_cameraTransform.forward.y > 0)
        {
            var up = Vector3.Angle(front, _cameraTransform.forward);
            if (up > upperAngle)
            {
                transform.rotation *= Quaternion.AngleAxis(up - upperAngle, Vector3.right);
            }
            if (up < -lowerAngle)
            {
                transform.rotation *= Quaternion.AngleAxis(up + lowerAngle, Vector3.right);
            }
        }
        else
        {
            var down = Vector3.Angle(front, _cameraTransform.forward);
            if (down > lowerAngle)
            {
                transform.rotation *= Quaternion.AngleAxis(down - lowerAngle, -Vector3.right);
            }
        }
    }

    #endregion

}
