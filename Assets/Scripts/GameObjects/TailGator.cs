using ObjectPool;
using UnityEngine;

public class TailGator : TriangularShip
{
    public EntityId InstanceId => GameObject.GetEntityId();
    private FollowPlayerBehaviour followPlayer;
    private ShootBehaviour shootBehaviour;
    private EntityId bulletId;
    private Sprite shipSprite;

    public TailGator(string name) : base(name)
	{
        GameObject.layer = (int)CollisionLayer.Enemy;

        followPlayer = GameObject.AddComponent<FollowPlayerBehaviour>();
        followPlayer.MovementSpeed = UnitStats.TailGatorSpeed;

        shootBehaviour = GameObject.AddComponent<ShootBehaviour>();

        shootBehaviour.PoolableType = PoolableType.Bullet;
        shootBehaviour.SubscribeToProjectileStateChangedEvent(OnProjectileActivated, OnProjectileDeactivated);
        GameObject.GetComponent<SpriteRenderer>().color = Color.red;
        SetSprite();
    }
    
    private void SetSprite()
    {
        shipSprite = Resources.Load<Sprite>("Sprites/red-ship");

        SpriteRenderer spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.transform.localScale = new  Vector3(0.5f, 0.5f, 0.5f);
        spriteRenderer.sprite = shipSprite;
        spriteRenderer.sortingOrder = 10;
        spriteRenderer.color = Color.white;
    }

    public override void Activate(Vector2 position)
    {
        base.Activate(position);        
        shootBehaviour.Activate(InstanceId);
    }

    private void OnProjectileActivated(EntityId instanceId)
    {   
        bulletId = instanceId;
        collisionBehaviour.StartIgnoringTriggerEventsFor(instanceId);
    }

    private void OnProjectileDeactivated(EntityId instanceId)
    {
        collisionBehaviour.ClearIgnoreTriggerEvents();
    }

    public override void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        base.HandleCollision(self, colliderInformation);
        collisionBehaviour.ClearIgnoreTriggerEvents();
    }

    public override void DestroySelf(bool raiseEntityDestroyedEvent = true)
    {
        base.DestroySelf(raiseEntityDestroyedEvent);
        collisionBehaviour.ClearIgnoreTriggerEvents();
    }
}