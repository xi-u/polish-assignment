using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Mine : EntityList, IProjectile
{
    private EntityId[] ownerIds;
    private CollisionBehaviour collisionBehaviour;
    private MineBehaviour mineBehaviour;

    private Sphere body;
    private List<Triangle> spikes;
    private CircleCollider2D collider;
    private Rigidbody2D rigidbody;
    private Sprite mineSprite;

    public Mine(string name) : base(name)
    {
        GameObject.layer = (int)CollisionLayer.Projectile;

        body = new Sphere("Body");
        body.Transform.SetParent(Transform);
        body.CreatePrimitiveShape();
        collider = body.AddCollider() as CircleCollider2D;
        collider.offset = new Vector2(-0.2f, -0.2f);
        collider.radius = 0.3f;

        rigidbody = GameObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;

        body.Transform.GetComponent<SpriteRenderer>().color = new Color32(255, 105, 180, 255);
        body.Transform.localScale = Vector3.one * 1.4f;
        body.Transform.localPosition = Vector3.one * 0.28f;

        mineBehaviour = Transform.AddComponent<MineBehaviour>();
        mineBehaviour.BodyInstanceId = InstanceId;
        collisionBehaviour = Transform.AddComponent<CollisionBehaviour>();
        collisionBehaviour.SubscribeToTriggerEnteredEvent(HandleCollision);

        spikes = new List<Triangle>();
        CreateSpikes(8);

        Transform.localScale = Vector3.one * 0.5f;
        
        SetSprite();
    }
    
    private void SetSprite()
    {
        mineSprite = Resources.Load<Sprite>("Sprites/mine");

        SpriteRenderer spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.transform.localScale = new  Vector3(0.3f, 0.3f, 0.3f);
        spriteRenderer.sprite = mineSprite;
        spriteRenderer.sortingOrder = 10;
        spriteRenderer.color = Color.white;

    }

    private void CreateSpikes(int numberOfSpikes)
    {
        float radius = 0.5f;
        float angleIncrement = 360 / numberOfSpikes;
        float currentAngle = 0;
        for (int i = 0; i < numberOfSpikes; i++)
        {
            Triangle spike = new Triangle("Spike");
            spike.CreatePrimitiveShape();
            spike.Transform.SetParent(Transform);
            spike.GameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 105, 180, 255);

            float x = radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad); ;
            float y = radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

            spike.Transform.position = new Vector2(x, y);
            spike.Transform.Rotate(currentAngle - 90);
            spike.Transform.localScale = Vector2.one * 0.2730129f;

            currentAngle += angleIncrement;
        }
    }

    public EntityId InstanceId => body.GameObject.GetEntityId();

    public EntityId[] OwnerIds => ownerIds;

    public void DoShoot(Vector2 direction, ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated, params EntityId[] ownerIds)
    {
        this.ownerIds = ownerIds;
        mineBehaviour.DoShoot(this, direction, onProjectileDeactivated);
        collisionBehaviour.StartIgnoringTriggerEventsFor(ownerIds);
    }

    public override void Activate(Vector2 position)
    {
        base.Activate(position);
    }

    public void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        DestroySelf();
        collisionBehaviour.StopIgnoringTriggerEventsFor(ownerIds);
        CameraBehaviour.Instance.TriggerShake(0.75f, 0.08f);
    }
}
