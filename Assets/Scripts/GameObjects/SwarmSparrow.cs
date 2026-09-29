using UnityEngine;

public class SwarmSparrow : TriangularShip
{
    private FlockingBehaviour flockingBehaviour;
    private FlockingComponent[] flockingComponents;
    private Sprite shipSprite;

    public SwarmSparrow(string name) : base(name)
    {
        GameObject.layer = (int)CollisionLayer.Enemy;
        GameObject.GetComponent<SpriteRenderer>().color = Utility.DEEP_ORANGE;
        CreateFlockingBehaviour();
        GameObject.AddComponent<EnterCameraViewportBehaviour>();
        SetSprite();
    }
    
    private void SetSprite()
    {
        shipSprite = Resources.Load<Sprite>("Sprites/orange-ship");

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

    private void CreateFlockingBehaviour()
    {
        flockingComponents = new FlockingComponent[4]
        {
            new Cohesion(2, 3),
            new Separation(2, 1),
            new Alignment(2, 1),
            new FollowTransform(ServiceLocator.Instance.GetService<Player>().Transform, float.PositiveInfinity, 3)
        };

        flockingBehaviour = GameObject.AddComponent<FlockingBehaviour>();
        flockingBehaviour.AddFlockingComponents(flockingComponents);
    }

    public override void Activate(Vector2 position)
    {
        base.Activate(position);
        flockingBehaviour.MovementSpeed = UnitStats.HiveRaptorNormalSpeed;
    }
}
