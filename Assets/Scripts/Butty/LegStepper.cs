﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class LegStepper : MonoBehaviour
{
    [SerializeField] Transform homeTransform;

    [SerializeField] float wantStepAtDistance;

    [SerializeField] float moveDuration;

    [SerializeField] float stepOvershootFraction;

    public bool CanStep;

    public bool Moving;

    public void TryMove()
    {
        // If we are already moving, don't start another move
        if (Moving) return;

        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);

        // If we are too far off in position or rotation
        if (distFromHome > wantStepAtDistance)
        {
            // Start the step coroutine
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        Moving = true;
        Vector3 startPoint = transform.position;
        Quaternion startRot = transform.rotation;

        Quaternion endRot = homeTransform.rotation;

        // Directional vector from the foot to the home position
        Vector3 towardHome = (homeTransform.position - transform.position);
        // Total distnace to overshoot by   
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;
        // Since we don't ground the point in this simplified implementation,
        // we restrict the overshoot vector to be level with the ground
        // by projecting it on the world XZ plane.
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        // Apply the overshoot
        Vector3 endPoint = homeTransform.position + overshootVector;

        // We want to pass through the center point
        Vector3 centerPoint = (startPoint + endPoint) / 2;
        // But also lift off, so we move it up by half the step distance (arbitrarily)
        centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2f;

        float timeElapsed = 0;
        do
        {
            if (CanStep)
            {
                timeElapsed += Time.deltaTime;
                float normalizedTime = timeElapsed / moveDuration;

                normalizedTime = Easing.InOutCubic(normalizedTime);

                // Quadratic bezier curve
                transform.position =
                    Vector3.Lerp(
                    Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                    Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                    normalizedTime
                    );

                transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);
            }
            else
            {
                timeElapsed = moveDuration;
            }
            yield return null;

        }
        while (timeElapsed < moveDuration);
        Moving = false;
    }
}
