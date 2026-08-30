using UnityEngine;

public class WebWeaver : DoubleWingedTriangularShip
{    
    private RoamingBehaviour roamingBehaviour;
    private ShootBehaviour shootingBehaviour;

    public WebWeaver(string name) : base(name)
    {
        SetCollisionLayer(CollisionLayer.Enemy);
        AddCollider();

        SetColor(new Color32(255, 105, 180, 255));

        roamingBehaviour = GameObject.AddComponent<RoamingBehaviour>();
        roamingBehaviour.MovementSpeed = 2.5f;

        shootingBehaviour = GameObject.AddComponent<ShootBehaviour>();
        shootingBehaviour.ShootingInterval = 3.5f;
        shootingBehaviour.PoolableType = ObjectPool.PoolableType.Mine;
        shootingBehaviour.SubscribeToProjectileStateChangedEvent(OnMineActivated, OnMineDeactivated);
    }
    
    public EntityId InstanceId => GameObject.GetEntityId();

    public override void Activate(Vector2 position)
    {
        base.Activate(position);
        shootingBehaviour.Activate(leftWing.GameObject.GetEntityId(), rightWing.GameObject.GetEntityId());
    }

    private void OnMineActivated(EntityId instanceId)
    {
        collisionBehaviour.StartIgnoringTriggerEventsFor(instanceId);
    }

    private void OnMineDeactivated(EntityId instanceId)
    {
        collisionBehaviour.StopIgnoringTriggerEventsFor(instanceId);
    }
}
