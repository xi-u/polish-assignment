using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRhino : Triangle
{
    private Circle shield;
    private Triangle body;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private byte hitsTaken = 0;

    public ShieldRhino(string name) : base(name)
    {
        
        rigidbody = GameObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0;

        body = new Triangle("Body");
        body.CreatePrimitiveShape();
        body.GameObject.GetComponent<SpriteRenderer>().color = new Color32(90, 255, 255, 255);
        body.AddCollider().offset = new Vector2(-0.5f, -0.5f);
        body.GameObject.AddComponent<CollisionBehaviour>().SubscribeToTriggerEnteredEvent(HandleBodyCollision);
        GameObject.AddComponent<FollowBehaviour>();
        body.Transform.SetParent(Transform, false);

        shield = new Circle("Shield");
        shield.CreatePrimitiveShape();
        shield.AddCollider();
        CircleCollider2D circleCollider = shield.GameObject.GetComponent<CircleCollider2D>();
        circleCollider.offset = new Vector2(0, -1.5f);

        spriteRenderer = shield.GameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color32(90, 255, 255, 255);
        shield.Transform.SetParent(Transform, false);

        CollisionBehaviour collisionBehaviour = shield.GameObject.AddComponent<CollisionBehaviour>();        
    }

    public int InstanceId => GameObject.GetInstanceID();

    public Entity Entity => this;

    public void HandleBodyCollision(GameObject self, Collider2D colliderInformation)
    {
        DestroySelf();
    }
}