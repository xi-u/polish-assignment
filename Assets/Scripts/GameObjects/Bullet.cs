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
        GameObject.GetComponent<SpriteRenderer>().color = Color.red;
        rigidbody.gravityScale = 0;

        collisionBehaviour.SubscribeToTriggerEnteredEvent(HandleCollision);
    }

    public void DoShoot(Vector2 direction, ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated, params EntityId[] ownerId)
    {        
        this.ownerIds = ownerId;
        rigidbody.linearVelocity = direction;
        collisionBehaviour.StartIgnoringTriggerEventsFor(ownerId);
        this.onProjectileDeactivated = onProjectileDeactivated;
    }

    public void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        Deactivate();
        CameraBehaviour.Instance.TriggerShake(0.5f, 0.08f);
        onProjectileDeactivated(GameObject.GetEntityId());        
    }

    public override void Deactivate()
    {
        base.Deactivate();
        collisionBehaviour.ClearIgnoreTriggerEvents();
    }
}