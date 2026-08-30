using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerHawk : EntityList
{
    private Triangle firstTriangle;
    private Sphere window;
    private CollisionBehaviour collisionBehaviour;

    private FollowPlayerBehaviour playerFollowBehaviour;
    private ShootBehaviour shootBehaviour;
    public SeekerHawk(string name) : base(name)
    {
        GameObject.layer = (int)CollisionLayer.Enemy;

        playerFollowBehaviour = GameObject.AddComponent<FollowPlayerBehaviour>();
        playerFollowBehaviour.MovementSpeed = UnitStats.SeekerHawkSpeed;
        playerFollowBehaviour.RotationSpeed = 2;

        shootBehaviour = GameObject.AddComponent<ShootBehaviour>();
        shootBehaviour.ShootingInterval = 7.5f;        
        shootBehaviour.PoolableType = ObjectPool.PoolableType.HomingMissile;
        shootBehaviour.SubscribeToProjectileStateChangedEvent(OnProjectileActivated, OnProjectileDeactivated);

        collisionBehaviour = GameObject.AddComponent<CollisionBehaviour>();
        collisionBehaviour.SubscribeToTriggerEnteredEvent(HandleCollision);

        firstTriangle = new Triangle("wings");
        firstTriangle.CreatePrimitiveShape(new Vector2[3] { new(0, 0), new(3f, 0), new(1.5f, 1.5f) });
        firstTriangle.Transform.position = new Vector2(0, 0);
        SetPrimitiveShapeColorAndAttach(firstTriangle);

        Rigidbody2D rigidbody = GameObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;

        PolygonCollider2D polygonCollider2D = GameObject.AddComponent<PolygonCollider2D>();
        polygonCollider2D.points = new Vector2[3] { new(0, 0), new(3f, 0), new(1.5f, 1.5f) };
        polygonCollider2D.isTrigger = true;
        polygonCollider2D.offset = new Vector2(-1.5f, -1);
    }
    private void OnProjectileActivated(EntityId instanceId)
    {
        collisionBehaviour.StartIgnoringTriggerEventsFor(instanceId);
    }

    private void OnProjectileDeactivated(EntityId instanceId)
    {
        collisionBehaviour.StopIgnoringTriggerEventsFor(instanceId);
    }
    private void SetPrimitiveShapeColorAndAttach(PrimitiveShape primitiveShape)
    {
        primitiveShape.Transform.GetComponent<SpriteRenderer>().color = Color.blue;
        primitiveShape.Transform.SetParent(Transform, false);
    }

    public EntityId InstanceId => GameObject.GetEntityId();

    public Entity Entity => this;

    public void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        DestroySelf();
    }

    public override void Activate(Vector2 position)
    {
        base.Activate(position);        
        shootBehaviour.Activate(InstanceId);
    }
}
