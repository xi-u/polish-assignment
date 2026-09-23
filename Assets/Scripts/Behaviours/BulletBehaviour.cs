using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MovementBehaviour
{
    private const float closeRange = 8f;
    private const float farRange = 10f;
    private const float wooshCooldown = 3f;

    private Rigidbody2D rb2d;
    private AudioSource audioSource;
    private AudioClip wooshClip;
    private float nextWooshTime;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.pitch = Random.Range(2f, 2.5f);
        audioSource.volume = 0.6f;

        wooshClip = Resources.Load<AudioClip>("Audio/woosh");
    }

    private void Update()
    {
        if (ServiceLocator.Instance == null)
        {
            return;
        }

        Player player;
        try
        {
            player = ServiceLocator.Instance.GetService<Player>();
        }
        catch (System.Exception)
        {
            return;
        }

        if (player == null || player.Transform == null)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.Transform.position);

        if (distanceToPlayer <= closeRange)
        {
            if (wooshClip != null && Time.time >= nextWooshTime)
            {
                audioSource.PlayOneShot(wooshClip);
                nextWooshTime = Time.time + wooshCooldown;
            }
        }
        else if (distanceToPlayer >= farRange)
        {
            audioSource.Stop();
            nextWooshTime = 0f;
        }
    }

    private void OnDisable()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public override float MovementSpeed
    {
        get
        {
            return rb2d.linearVelocity.magnitude;
        }
        set
        {
            rb2d.linearVelocity.Normalize();
            rb2d.linearVelocity *= value;
        }
    }
}
