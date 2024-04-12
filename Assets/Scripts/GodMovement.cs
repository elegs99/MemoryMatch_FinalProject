using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodMovement : MonoBehaviour
{
    public GameObject rightHand, leftHand;
    public InputActionReference rightHandGrab, leftHandGrab;
    public float scalingFactor = .1f;
    public GameObject parentEnvironment;
    GameObject cloneEnviroment;
    bool rightHandClosed, leftHandClosed;
    Vector3 currentRightHandPosition, currentLeftHandPosition, lastRightHandPosition, lastLeftHandPosition;
    float currentHandsDistance, lastHandsDistance;
    Vector3 handMovement;
    // Start is called before the first frame update
    void Start()
    {
        lastRightHandPosition = rightHand.transform.position;
        lastLeftHandPosition = leftHand.transform.position;
        lastHandsDistance = Vector3.Distance(lastRightHandPosition, lastLeftHandPosition);
    }

    // Update is called once per frame
    void Update()
    {
        currentRightHandPosition = rightHand.transform.position;
        currentLeftHandPosition = leftHand.transform.position;
        currentHandsDistance = Vector3.Distance(currentRightHandPosition, currentLeftHandPosition);
        CheckIfHandsAreClosed();
        if (rightHandClosed && leftHandClosed)
        {
            RotateAndScaleEnvironment();
        }
        if (rightHandClosed || leftHandClosed)
        {
            TranslateEnvironment();
        } else {
            handMovement = Vector3.zero;
        }
        lastRightHandPosition = currentRightHandPosition;
        lastLeftHandPosition = currentLeftHandPosition;
        lastHandsDistance = currentHandsDistance;
    }

    void CheckIfHandsAreClosed()
    {
        rightHandClosed = rightHandGrab.action.ReadValue<float>() > 0.5f;
        leftHandClosed = leftHandGrab.action.ReadValue<float>() > 0.5f;
    }

    void TranslateEnvironment()
    {
        if (rightHandClosed)
        {
            handMovement = currentRightHandPosition - lastRightHandPosition;
        }
        else if (leftHandClosed)
        {
            handMovement = currentLeftHandPosition - lastLeftHandPosition;
        }
        parentEnvironment.transform.position += handMovement;
        cloneEnviroment.transform.position += handMovement;
    }

    void RotateAndScaleEnvironment()
    {
        float currentHandsDistance = Vector3.Distance(currentRightHandPosition, currentLeftHandPosition);

        float handDistanceChange = currentHandsDistance - lastHandsDistance;
        
        // Calculate new scale
        Vector3 newScale = parentEnvironment.transform.localScale + (Vector3.one * scalingFactor * handDistanceChange);
        newScale = Vector3.Max(newScale, new Vector3(0.1f, 0.1f, 0.1f)); // Setting minimum scale limit
        newScale = Vector3.Min(newScale, new Vector3(5f, 5f, 5f)); // Setting maximum scale limit

        // Apply the new scale
        parentEnvironment.transform.localScale = newScale;
        cloneEnviroment.transform.localScale = newScale;

        Vector3 previousDirection = lastRightHandPosition - lastLeftHandPosition;
        Vector3 currentDirection = currentRightHandPosition - currentLeftHandPosition;
        Vector3 rotationAxis = Vector3.Cross(previousDirection, currentDirection).normalized;

        // Determine the magnitude of the rotation
        float angle = Vector3.Angle(previousDirection, currentDirection);

        // Apply the rotation around the axis
        parentEnvironment.transform.Rotate(rotationAxis, angle, Space.World);
        cloneEnviroment.transform.Rotate(rotationAxis, angle, Space.World);
    }
    public void setCloneWorld(GameObject clone) {
        cloneEnviroment = clone;
    }
}