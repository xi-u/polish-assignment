using ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : EntityList, IProjectile
{
    private EntityId[] ownerIds;
    private Rectangle body;
    private Triangle[] wings;
    private Triangle hat;
    private Rigidbody2D rigidbody;
    public EntityId[] OwnerIds { get { return ownerIds; } set { ownerIds = value; } }

    private HomingMissleBehaviour homingMissleBehaviour;
    private CollisionBehaviour collisionBehaviour;
    
    private Sprite missleSprite;

    public EntityId InstanceId => body.GameObject.GetEntityId();

    public Entity Entity => this;

    public HomingMissile(string name) : base(name)
    {
        body = new Rectangle("body");
        body.CreatePrimitiveShape(new Vector2[4] { new (0, 0), new (0.25f, 1), new (0.25f, 0), new (0, 1) });
        body.AddCollider();
        body.GameObject.layer = (int)CollisionLayer.Projectile;
        collisionBehaviour = GameObject.AddComponent<CollisionBehaviour>();
        BoxCollider2D collider = body.GameObject.GetComponent<BoxCollider2D>();

        rigidbody = GameObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;

        collider.offset = new Vector2(-0.37f, 0);
        collider.size = new Vector2(0.25f, 1);

        CreateRocketWings();
        SetPrimitiveShapeColorAndAttach(body);

        hat = new Triangle("hat");
        hat.CreatePrimitiveShape(new Vector2[3] { new (0, 0), new (0.25f, 0), new (0.125f, 0.25f) });
        hat.Transform.position = new Vector2(0, 1);
        SetPrimitiveShapeColorAndAttach(hat);

        GameObject.AddComponent<EnterCameraViewportBehaviour>();
        homingMissleBehaviour = GameObject.AddComponent<HomingMissleBehaviour>();
        homingMissleBehaviour.BodyInstanceId = InstanceId;

        collisionBehaviour.SubscribeToTriggerEnteredEvent(HandleCollision);
        
        SetSprite();
    }
    
    private void SetSprite()
    {
        missleSprite = Resources.Load<Sprite>("Sprites/missle");

        SpriteRenderer spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.transform.localScale = new  Vector3(0.5f, 0.5f, 0.5f);
        spriteRenderer.sprite = missleSprite;
        spriteRenderer.sortingOrder = 10;
        spriteRenderer.color = Color.white;
    }

    private void SetPrimitiveShapeColorAndAttach(PrimitiveShape primitiveShape)
    {
        primitiveShape.Transform.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        primitiveShape.Transform.SetParent(Transform, false);
    }

    private void CreateRocketWings()
    { 
        wings = new Triangle[4];

        wings[0] = new Triangle("Left back wing");
        wings[0].CreatePrimitiveShape(new Vector2[3] { new (0, 0), new (0.25f, 0), new (0.25f, 0.25f) });
        wings[0].Transform.position = new Vector2(-0.25f, 0.08f);

        wings[1] = new Triangle("Right back wing");
        wings[1].CreatePrimitiveShape(new Vector2[3] { new(0, 0), new(-0.25f, 0), new(-0.25f, 0.25f) });
        wings[1].Transform.position = new Vector2(0.25f, 0.08f);

        wings[2] = new Triangle("Left front wing");
        wings[2].CreatePrimitiveShape(new Vector2[3] { new(0, 0), new(0.18f, 0), new(0.18f, 0.18f) });
        wings[2].Transform.position = new Vector2(-0.18f, 0.65f);

        wings[3] = new Triangle("Right front wing");
        wings[3].CreatePrimitiveShape(new Vector2[3] { new(0, 0), new(-0.18f, 0), new(-0.18f, 0.18f) });
        wings[3].Transform.position = new Vector2(0.25f, 0.65f);

        for (int i = 0; i < wings.Length; i++)
        {
            SetPrimitiveShapeColorAndAttach(wings[i]);
        }
    }

    public void DoShoot(Vector2 direction, ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated, params EntityId[] ownerId)
    {
        this.ownerIds = ownerId;
        homingMissleBehaviour.DoShoot(this, direction, onProjectileDeactivated);
        collisionBehaviour.StartIgnoringTriggerEventsFor(ownerId);
    }

    public void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        DestroySelf();
        collisionBehaviour.StopIgnoringTriggerEventsFor(ownerIds);
        CameraBehaviour.Instance.TriggerShake(0.75f, 0.08f);
    }
}