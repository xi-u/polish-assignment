using ObjectPool;
using UnityEngine;

public class TailGator : TriangularShip
{
    public EntityId InstanceId => GameObject.GetEntityId();
    private FollowPlayerBehaviour followPlayer;
    private ShootBehaviour shootBehaviour;
    private EntityId bulletId;

    public TailGator(string name) : base(name)
	{
        GameObject.layer = (int)CollisionLayer.Enemy;

        followPlayer = GameObject.AddComponent<FollowPlayerBehaviour>();
        followPlayer.MovementSpeed = UnitStats.TailGatorSpeed;

        shootBehaviour = GameObject.AddComponent<ShootBehaviour>();

        shootBehaviour.PoolableType = PoolableType.Bullet;
        shootBehaviour.SubscribeToProjectileStateChangedEvent(OnProjectileActivated, OnProjectileDeactivated);
        GameObject.GetComponent<SpriteRenderer>().color = Color.red;
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