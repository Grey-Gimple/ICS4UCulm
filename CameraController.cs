using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    public Transform cameraTransform;
    public Vector3[] cameraPositions;
    public Vector3[] cameraPositionsOffset;
    public Vector3[] cameraBoundSize;

    public float timeOffset;

    private Vector3 smoothDampVelocity = Vector3.zero;

    public Color[] gizmosColor;

    private void Start() {
        cameraTransform = GetComponent<Transform>();
    }

    private void Update() {
        for (int i = 0; i < cameraPositions.Length; i++)
        {
            if (playerTransform.position.x < cameraPositions[i].x + (cameraBoundSize[i].x / 2) && 
            playerTransform.position.x > cameraPositions[i].x - (cameraBoundSize[i].x / 2) && 
            playerTransform.position.y < cameraPositions[i].y + (cameraBoundSize[i].y / 2) &&
            playerTransform.position.y > cameraPositions[i].y - (cameraBoundSize[i].y / 2))
            {
                cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, new Vector3(cameraPositions[i].x + cameraPositionsOffset[i].x, cameraPositions[i].y + cameraPositionsOffset[i].y, cameraPositions[i].z + cameraPositionsOffset[i].z), ref smoothDampVelocity, timeOffset);
            }
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < cameraPositions.Length; i++) {
            Gizmos.color = gizmosColor[i];
            Gizmos.DrawCube(cameraPositions[i], cameraBoundSize[i]);
        }
    }
}
