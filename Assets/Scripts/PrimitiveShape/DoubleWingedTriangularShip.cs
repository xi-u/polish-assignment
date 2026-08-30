using UnityEngine;

public class DoubleWingedTriangularShip : EntityList
{
    protected Triangle leftWing, rightWing;
    protected SpriteRenderer leftWingSpriteRenderer, rightWingSpriteRenderer;
    protected Rigidbody2D rigidbody;
    protected CollisionBehaviour collisionBehaviour;

    public EntityId[] InstanceIds
    {
        get
        {
            return new EntityId[2]
            {
                leftWing.InstanceId,
                rightWing.InstanceId
            };
        } 
    }

    public SpriteRenderer[] SpriteRenderers
    {
        get
        {
            return new SpriteRenderer[2]
            {
                leftWingSpriteRenderer,
                rightWingSpriteRenderer
            };
        }
    }

    public DoubleWingedTriangularShip(string name) : base(name)
    {
        CreateBody();

        rigidbody = GameObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
    }

    public void AddCollider()
    {
        collisionBehaviour = GameObject.AddComponent<CollisionBehaviour>();
        collisionBehaviour.SubscribeToTriggerEnteredEvent(HandleCollision);

        leftWing.AddCollider();
        leftWing.GameObject.GetComponent<Collider2D>().offset = new Vector2(0, -0.7f);

        rightWing.AddCollider();
        rightWing.GameObject.GetComponent<Collider2D>().offset = new Vector2(-1, -0.7f);
    }

    protected void CreateBody()
    {
        leftWing = new Triangle("Left Wing");
        leftWing.CreatePrimitiveShape(new Vector2[3] { new(-0.5f, -0.3f), new(0, 0), new(0, 1) });
        leftWingSpriteRenderer = leftWing.Transform.GetComponent<SpriteRenderer>();
        leftWing.Transform.SetParent(Transform, false);

        rightWing = new Triangle("Right Wing");
        rightWing.CreatePrimitiveShape(new Vector2[3] { new(1, -0.3f), new(0.5f, 0), new(0.5f, 1) });
        rightWingSpriteRenderer = rightWing.Transform.GetComponent<SpriteRenderer>();
        rightWing.Transform.SetParent(Transform, false);
        rightWing.Transform.localPosition = new Vector3(0.5f, 0, 0);

        // The two wings together are centered at local x = 0.25 (left wing spans
        // roughly -0.25..0.25, right wing 0.25..0.75), not at the root's own
        // origin. FlockingBehaviour rotates around this pivot instead of the
        // root transform so the ship turns around its visual center.
        GameObject rotationPivot = new GameObject("RotationPivot");
        rotationPivot.transform.SetParent(Transform, false);
        rotationPivot.transform.localPosition = new Vector3(0.25f, 0, 0);
    }

    public void SetCollisionLayer(CollisionLayer collisionLayer)
    {
        leftWing.GameObject.layer = (int)collisionLayer;
        rightWing.GameObject.layer = (int)collisionLayer;
    }

    public void SetColor(Color32 color)
    {
        leftWingSpriteRenderer.color = color;
        rightWingSpriteRenderer.color = color;
    }

    protected virtual void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        DestroySelf();
    }
}
