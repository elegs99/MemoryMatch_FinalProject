using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerCamera;
    public float thresholdAngle = 30.0f;
    public float distanceFromPlayer = 10.0f;

    void Update()
    {
        Vector3 forwardFlat = playerCamera.forward;
        forwardFlat.y = 0;

        if (Vector3.Angle(forwardFlat, playerCamera.forward) > thresholdAngle)
        {
            transform.position = playerCamera.position + playerCamera.forward * distanceFromPlayer;
            transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
        }
        else
        {
            transform.position = new Vector3(0, -1000, 0);
        }
    }
    
}
