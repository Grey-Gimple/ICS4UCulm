using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSystemScript : MonoBehaviour
{
    [Header("Door Settings")]
    public SpriteRenderer doorSpriteRenderer;
    public Sprite openDoorSprite;
    public Sprite closedDoorSprite;
    private bool doorIsOpen;

    [Space(5)]

    [Header("Door Inspect Settings")]
    public Vector2 doorInspectDistance;
    public LayerMask doorInspectLayerMask;
    public float doorInspectSpeed;
    public SpriteRenderer doorInspectSpriteRenderer;
    public Sprite openDoorInspectSprite;
    public Sprite closedDoorInspectSprite;
    private bool isInspect;

    [Space(10)]

    [Header("Plate Settings")]
    public SpriteRenderer plateSpriteRenderer;
    public Sprite enabledPlateSprite;
    public Sprite disabledPlateSprite;
    public LayerMask whatisPlateLayer;
    public Transform playerTransform;
    public Vector2 plateCheckSize;
    private bool isPlate;

    [Space(10)]

    [Header("Lights")]
    public UnityEngine.Rendering.Universal.Light2D plateLightDisabled;
    public UnityEngine.Rendering.Universal.Light2D plateLightEnabled;
    public UnityEngine.Rendering.Universal.Light2D doorLight;
    public Color openDoorGlowColor;

    [Space(10)]
    [Header("Audio")]
    public AudioSource plateSound;

    private void Update() {
        isPlate = Physics2D.OverlapBox(playerTransform.position, plateCheckSize, 0, whatisPlateLayer);
        isInspect = Physics2D.OverlapBox(playerTransform.position, doorInspectDistance, 0, doorInspectLayerMask);

        if (isPlate)
        {
            plateSpriteRenderer.sprite = enabledPlateSprite;
            plateLightDisabled.enabled = false;
            plateLightEnabled.enabled = true;
            doorSpriteRenderer.sprite = openDoorSprite;
            doorInspectSpriteRenderer.sprite = openDoorInspectSprite;
            doorLight.color = openDoorGlowColor;
            if (!doorIsOpen)
            {
                plateSound.Play();
            }
            doorIsOpen = true;
        }

        if (isInspect)
        {
            doorInspectSpriteRenderer.color = Color.Lerp(doorInspectSpriteRenderer.color, new Color(1, 1, 1, 1), doorInspectSpeed * Time.deltaTime);
        } else
        {
            doorInspectSpriteRenderer.color = Color.Lerp(doorInspectSpriteRenderer.color, new Color(1, 1, 1, 0), doorInspectSpeed * Time.deltaTime);
        }

        if (isInspect && doorIsOpen && Input.GetKeyDown(KeyCode.W) || isInspect && doorIsOpen && Input.GetAxis("Vertical") > 0.5f)
        {
            if (SceneManager.GetActiveScene().buildIndex == 8)
            {
                SceneManager.LoadScene(1);
            } else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
