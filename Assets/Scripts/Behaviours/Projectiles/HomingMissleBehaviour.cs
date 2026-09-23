using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissleBehaviour : MovementBehaviour
{
    private Transform playerTransform;
    private bool isInPursuit;
    private Rigidbody2D rb2d;
    private HomingMissile owner;
    private ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated;
    private EntityId bodyInstanceId;
    public EntityId BodyInstanceId { set { bodyInstanceId = value; } }
    
    private AudioClip wooshClip;
    private float nextWooshTime;
    
    private const float closeRange = 8f;
    private const float farRange = 10f;
    private const float wooshCooldown = 3f;

    // Start is called before the first frame update
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        wooshClip = Resources.Load<AudioClip>("Audio/woosh");
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

    public void DoShoot(HomingMissile owner, Vector2 initialDirection, ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated)
    {
        this.owner = owner;
        this.onProjectileDeactivated = onProjectileDeactivated;
        isInPursuit = false;
        playerTransform = ServiceLocator.Instance.GetService<Player>().Transform;        
        StartCoroutine(FollowPlayer());
        StartCoroutine(CanShootOwner());
    }

    private IEnumerator CanShootOwner()
    {
        yield return new WaitForSeconds(2);
        onProjectileDeactivated(bodyInstanceId);
        GetComponent<CollisionBehaviour>().StopIgnoringTriggerEventsFor(owner.OwnerIds);
    }

    public IEnumerator FollowPlayer()
    {
        Vector2 targetPosition = playerTransform.position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        LookAtAndSetVelocity(direction, 2f);

        yield return new WaitForSeconds(1.5f);

        isInPursuit = true;
        rb2d.linearVelocity = direction * 20f;

        yield return new WaitForSeconds(10);

        owner.DestroySelf();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 playerDir = playerTransform.transform.position - transform.position;

        if (!isInPursuit)
        {
            return;
        }

        if (playerDir.sqrMagnitude <= 160)
        {
            playerDir.Normalize();
            Vector2 velocity = (rb2d.linearVelocity + playerDir * 0.6f).normalized;

            LookAtAndSetVelocity(velocity, 20f);
        }
        
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

    private void LookAtAndSetVelocity(Vector2 direction, float speed)
    {
        // Calculate the angle to rotate
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        // Rotate the missile towards its direction
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Apply force in the forward direction
        rb2d.linearVelocity = direction * UnitStats.HomingMissileSpeed;
    }
}
