using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    public Vector3 top;
    public Vector3 bottom;

    public Transform platformTransform;

    public Transform playerTransform;

    public float timeOffset;

    public Vector3 smoothDampVelocity = Vector3.zero;

    private bool toTop = true;

    public Vector2 platformCheckSize;
    public LayerMask platformCheckLayer;
    public bool isPlatform;

    // Update is called once per frame
    void Update()
    {
        isPlatform = Physics2D.OverlapBox(playerTransform.position, platformCheckSize, 0, platformCheckLayer);
        if (platformTransform.position.y >= top.y - 0.1f)
        {
            toTop = false;
        } else if (platformTransform.position.y <= bottom.y + 0.1f)
        {
            toTop = true;
        }

        if (toTop)
        {
            platformTransform.position = Vector3.SmoothDamp(platformTransform.position, top, ref smoothDampVelocity, timeOffset);
        } else
        {
            platformTransform.position = Vector3.SmoothDamp(platformTransform.position, bottom, ref smoothDampVelocity, timeOffset);
        }
    }
}
