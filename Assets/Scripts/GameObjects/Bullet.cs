using UnityEngine;

public class Bullet : Sphere, IProjectile
{    
    private Rigidbody2D rigidbody;
    public EntityId InstanceId => GameObject.GetEntityId();
    public EntityId[] OwnerIds => ownerIds;
    private EntityId[] ownerIds;
    public Entity Entity => this;

    private CollisionBehaviour collisionBehaviour;
    private ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated;
    
    private Sprite bulletSprite;
    private TrailRenderer trailRenderer;

    public Bullet(string name) : base(name)
    {
        CircleCollider2D circleCollider = base.AddCollider() as CircleCollider2D;
        circleCollider.radius = 0.3f;
        circleCollider.offset = new Vector2(-0.2f, -0.2f);

        GameObject.layer = (int)CollisionLayer.Projectile;

        CreatePrimitiveShape();
        rigidbody = GameObject.AddComponent<Rigidbody2D>();
        
        collisionBehaviour = GameObject.AddComponent<CollisionBehaviour>();
        GameObject.AddComponent<EnterCameraViewportBehaviour>();
        GameObject.AddComponent<BulletBehaviour>();
        ConfigureTrail();
        SetSprite();
        rigidbody.gravityScale = 0;

        collisionBehaviour.SubscribeToTriggerEnteredEvent(HandleCollision);
    }

    private void ConfigureTrail()
    {
        trailRenderer = GameObject.GetComponent<TrailRenderer>();
        if (trailRenderer == null)
        {
            trailRenderer = GameObject.AddComponent<TrailRenderer>();
        }

        trailRenderer.time = 0.18f;
        trailRenderer.startWidth = 0.5f;
        trailRenderer.endWidth = 0f;
        trailRenderer.minVertexDistance = 0.05f;
        trailRenderer.autodestruct = false;
        trailRenderer.emitting = true;
        trailRenderer.startColor = new Color(1f, 0.7f, 0.2f, 1f);
        trailRenderer.endColor = new Color(1f, 0.3f, 0f, 0f);
        trailRenderer.sortingOrder = 9;

        Material trailMaterial = new Material(Shader.Find("Sprites/Default"));
        trailMaterial.color = trailRenderer.startColor;
        trailRenderer.material = trailMaterial;
        trailRenderer.Clear();
    }

    private void SetSprite()
    {
        bulletSprite = Resources.Load<Sprite>("Sprites/fireball");

        SpriteRenderer spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        spriteRenderer.sprite = bulletSprite;
        spriteRenderer.sortingOrder = 10;
        spriteRenderer.color = Color.white;
    }

    public void DoShoot(Vector2 direction, ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated, params EntityId[] ownerId)
    {        
        this.ownerIds = ownerId;
        trailRenderer.Clear();
        rigidbody.linearVelocity = direction;
        collisionBehaviour.StartIgnoringTriggerEventsFor(ownerId);
        this.onProjectileDeactivated = onProjectileDeactivated;
    }

    public void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        Deactivate();
        CameraBehaviour.Instance.TriggerShake(0.25f, 0.08f);
        onProjectileDeactivated(GameObject.GetEntityId());        
    }

    public override void Deactivate()
    {
        base.Deactivate();
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }
        collisionBehaviour.ClearIgnoreTriggerEvents();
    }
}