using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBehaviour : MonoBehaviour
{
    private Mine owner;
    private ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated;

    private EntityId bodyInstanceId;
    public EntityId BodyInstanceId { set { bodyInstanceId = value; } }

    private float rotation = 0;

    [SerializeField]
    private float rotationSpeed = 1.5f, amplitude = 0.5f, speed = 1.5f;
    [SerializeField]
    private float pulseAmplitude = 0.22f, pulseSpeed = 7f;
    [SerializeField]
    private float ringPulseSpeed = 1.4f, ringMaxScale = 2.6f, ringStartScale = 1.05f;

    private Vector2 initialPosition;
    private Vector3 baseScale;
    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private SpriteRenderer pulseRingRenderer;
    private Color ringBaseColor = new Color(1f, 0f, 0f, 0.7f);

    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            baseColor = spriteRenderer.color;
        }

        GameObject pulseRing = new GameObject("PulseRing");
        pulseRing.transform.SetParent(transform, false);
        pulseRing.transform.localPosition = Vector3.zero;
        pulseRing.transform.localScale = Vector3.one * ringStartScale;

        pulseRingRenderer = pulseRing.AddComponent<SpriteRenderer>();
        pulseRingRenderer.sprite = spriteRenderer != null ? spriteRenderer.sprite : null;
        pulseRingRenderer.color = ringBaseColor;
        pulseRingRenderer.sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder - 1 : 9;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation);
        rotation += rotationSpeed;

        float newY = initialPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector2(initialPosition.x, newY);

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        transform.localScale = baseScale * pulse;

        if (spriteRenderer != null)
        {
            float alphaPulse = 0.75f + (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.125f;
            spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alphaPulse);
        }

        if (pulseRingRenderer != null)
        {
            float t = Mathf.Repeat(Time.time * ringPulseSpeed, 1f);
            float ringScale = Mathf.Lerp(ringStartScale, ringMaxScale, t);
            pulseRingRenderer.transform.localScale = Vector3.one * ringScale;

            float ringAlpha = Mathf.Lerp(ringBaseColor.a, 0f, t);
            pulseRingRenderer.color = new Color(ringBaseColor.r, ringBaseColor.g, ringBaseColor.b, ringAlpha);
        }
    }

    public void DoShoot(Mine owner, Vector2 initialDirection, ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated)
    {
        initialPosition = transform.position;
        this.owner = owner;
        this.onProjectileDeactivated = onProjectileDeactivated;
        AudioManager.Instance.Play(Resources.Load<AudioClip>("Audio/mine"), transform.position, 0.25f);
        StartCoroutine(CanShootOwner());
    }

    private IEnumerator CanShootOwner()
    {
        yield return new WaitForSeconds(1.5f);
        onProjectileDeactivated(bodyInstanceId);
        GetComponent<CollisionBehaviour>().StopIgnoringTriggerEventsFor(owner.OwnerIds);
    }
}
