using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClapDetector : MonoBehaviour
{
    public OVRHand rightHand;
    public OVRHand leftHand;

    public float thresholdDistance = 0.15f;

    private Vector3 newPosRight;
    private Vector3 prevPosRight;
    private Vector3 rightHandVelocity;

    private Vector3 newPosLeft;
    private Vector3 prevPosLeft;
    private Vector3 leftHandVelocity;

    public UnityEvent onClap;
    public GameObject clapFeedback;

    private bool clapInvoked = false;

    void FixedUpdate()
    {
        newPosRight = rightHand.transform.position;
        rightHandVelocity = (newPosRight - prevPosRight) / Time.fixedDeltaTime;
        prevPosRight = newPosRight;

        newPosLeft = leftHand.transform.position;
        leftHandVelocity = (newPosLeft - prevPosLeft) / Time.fixedDeltaTime;
        prevPosLeft = newPosLeft;
    }

    void Update()
    {
        //Debug.Log(Vector3.Distance(rightHand.transform.position, leftHand.transform.position));
        if (rightHand.IsTracked && leftHand.IsTracked)
        {
            if (!clapInvoked && Vector3.Distance(rightHand.transform.position, leftHand.transform.position) <= thresholdDistance
                && rightHandVelocity.x < -0.15f && leftHandVelocity.x > 0.15f)
            {
                onClap.Invoke();
               // Instantiate(clapFeedback, rightHand.transform.position, Quaternion.identity);
                clapInvoked = true;
            }

            if (Vector3.Distance(rightHand.transform.position, leftHand.transform.position) > thresholdDistance * 2)
            {
                clapInvoked = false;
            }
        }
    }
}
