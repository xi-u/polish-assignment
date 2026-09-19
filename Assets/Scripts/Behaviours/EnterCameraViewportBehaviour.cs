using System.Collections;
using UnityEngine;

public class EnterCameraViewportBehaviour : MonoBehaviour
{
    private Camera camera;
    private Player player;
    private bool effectTriggered = false;
    private SpriteRenderer spriteRenderer;
    private MovementBehaviour movementBehaviour;
    private Collider2D[] colliders;

    private float timeToStop = 0.2f;
    private float timeToBlink = 0.3f;
    private float timeToResume = 0.2f;

    private void Awake()
    {
        camera = Camera.main;
        player = ServiceLocator.Instance.GetService<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementBehaviour = GetComponent<MovementBehaviour>();
        colliders = GetComponentsInChildren<Collider2D>();
    }

    private void OnEnable()
    {
        effectTriggered = false;
    }
}
