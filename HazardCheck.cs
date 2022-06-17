using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardCheck : MonoBehaviour
{
    [Header("Hazard Check Points")]
    public Vector3[] hazardSafetyPoints;
    public Vector3[] hazardSafetySize;
    private int currentHazardIndex;

    [Space(5)]

    [Header("Hazard Variables")]
    public Vector2 hazardCheckSize;
    public LayerMask hazardCheckLayer;
    private bool isHazard;

    [Space(5)]

    [Header("Gizmos")]
    public Color[] gizmosColor;

    [Space(10)]

    [Header("Scripts Required")]
    public Transform playerTransform;
    public PlayerController playerController;
    public Rigidbody2D playerRigidbody;


    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerController = GetComponent<PlayerController>();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isHazard = Physics2D.OverlapBox(playerTransform.position, hazardCheckSize, 0, hazardCheckLayer);

        for (int i = 0; i < hazardSafetyPoints.Length; i++)
        {
            if (playerTransform.position.x < hazardSafetyPoints[i].x + (hazardSafetySize[i].x / 2) &&
                playerTransform.position.x > hazardSafetyPoints[i].x - (hazardSafetySize[i].x / 2) &&
                playerTransform.position.y < hazardSafetyPoints[i].y + (hazardSafetySize[i].y / 2) &&
                playerTransform.position.y > hazardSafetyPoints[i].y - (hazardSafetySize[i].y / 2))
            {
                currentHazardIndex = i;
            }
        }

        if (isHazard)
        {
            StartCoroutine(HazardHit());
        }
    }

    private IEnumerator HazardHit()
    {
        playerController.enabled = false;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.gravityScale = 0;
        yield return new WaitForSeconds(0.3f);
        playerRigidbody.gravityScale = 1;
        playerTransform.position = new Vector3(hazardSafetyPoints[currentHazardIndex].x, hazardSafetyPoints[currentHazardIndex].y, playerTransform.position.z);
        yield return new WaitForSeconds(1f);
        playerController.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < hazardSafetyPoints.Length; i++)
        {
            Gizmos.color = gizmosColor[i];
            Gizmos.DrawCube(hazardSafetyPoints[i], hazardSafetySize[i]);
        }
    }
}
