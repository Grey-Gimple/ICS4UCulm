using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableHazard : MonoBehaviour
{
    public Vector3 top;
    public Vector3 bottom;

    public Transform hazardTransform;

    public float timeOffset;

    public Vector3 smoothDampVelocity = Vector3.zero;

    public bool horizontal;

    public bool toTop = true;

    // Update is called once per frame
    void Update()
    {
        if (horizontal)
        {
            if (hazardTransform.position.x >= top.x - 0.1f)
            {
                toTop = false;
            } else if (hazardTransform.position.x <= bottom.x + 0.1f)
            {
                toTop = true;
            }
        }
        else
        {
            if (hazardTransform.position.y >= top.y - 0.1f)
            {
                toTop = false;
            } else if (hazardTransform.position.y <= bottom.y + 0.1f)
            {
                toTop = true;
            }
        }

        if (toTop)
        {
            hazardTransform.position = Vector3.SmoothDamp(hazardTransform.position, top, ref smoothDampVelocity, timeOffset);
        } else
        {
            hazardTransform.position = Vector3.SmoothDamp(hazardTransform.position, bottom, ref smoothDampVelocity, timeOffset);
        }
    }
}
