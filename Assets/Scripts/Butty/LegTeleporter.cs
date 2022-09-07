using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegTeleporter : MonoBehaviour
{
    [SerializeField] Transform movingTransform;

    [SerializeField] float wantStepAtDistance;

    void Update()
    {
        float distFromHome = Vector3.Distance(transform.position, movingTransform.position);

        if (distFromHome > wantStepAtDistance)
        {
            movingTransform.position = transform.position;

            movingTransform.rotation = transform.rotation;
        }
    }

}

