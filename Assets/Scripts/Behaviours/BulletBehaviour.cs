using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MovementBehaviour
{
    private const float closeRange = 8f;
    private const float farRange = 10f;
    private const float wooshCooldown = 3f;

    private Rigidbody2D rb2d;
    private AudioClip wooshClip;
    private float nextWooshTime;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
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

        if (distanceToPlayer >= farRange)
        {
            nextWooshTime = 0f;
            return;
        }

        if (distanceToPlayer > closeRange)
        {
            return;
        }

        if (wooshClip == null || Time.time < nextWooshTime)
        {
            return;
        }

        AudioManager.EnsureInstanceExists();
        AudioManager.Instance.Play(wooshClip, transform.position, 0.5f);
        nextWooshTime = Time.time + wooshCooldown;
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
