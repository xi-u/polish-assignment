using UnityEngine;

public class HiveRaptor : TriangularShip
{
    private HiveRaptorBehaviour hiveRaptorBehaviour;    
    private FlockingBehaviour flockingBehaviour;
    private FlockingComponent[] flockingComponents;
    
    public HiveRaptor(string name) : base(name)
    {
        GameObject.layer = (int)CollisionLayer.Enemy;

        hiveRaptorBehaviour = GameObject.AddComponent<HiveRaptorBehaviour>();
        //GameObject.AddComponent<EnterCameraViewportBehaviour>();
        GameObject.GetComponent<SpriteRenderer>().color = Color.yellow;

        CreateFlockingBehaviour();
    }

    private void CreateFlockingBehaviour()
    {
        flockingComponents = new FlockingComponent[4]
        {
            new Cohesion(5, 3),
            new Separation(2, 2),
            new Alignment(4, 4),
            new FollowTransform(ServiceLocator.Instance.GetService<Player>().Transform, float.PositiveInfinity, 3)
        };

        flockingBehaviour = GameObject.AddComponent<FlockingBehaviour>();
        flockingBehaviour.AddFlockingComponents(flockingComponents);
    }

    public void EnterKamikazeMode()
    {
        flockingComponents[3].Strength = 7;        
    }

    public void SetSpeed(float speed)
    {
        flockingBehaviour.MovementSpeed = speed;
    }
        
    public override void Activate(Vector2 position)
    {
        base.Activate(position);
        hiveRaptorBehaviour.Activate(this);

        flockingBehaviour.MovementSpeed = UnitStats.HiveRaptorNormalSpeed;
        flockingComponents[3].Strength = 3;
    }

    public override void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        DestroySelf();        
    }
}
