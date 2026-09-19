using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangularShip : Triangle
{
    protected CollisionBehaviour collisionBehaviour;
    protected Rigidbody2D rigidbody;

    public TriangularShip(string name) : base(name) 
    {
        CreatePrimitiveShape();
        base.AddCollider().offset = new Vector2(-0.5f, -0.5f);

        rigidbody = GameObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0;

        collisionBehaviour = GameObject.AddComponent<CollisionBehaviour>();
        collisionBehaviour.SubscribeToTriggerEnteredEvent(HandleCollision);
    }

    public virtual void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        DestroySelf();
    }
}
