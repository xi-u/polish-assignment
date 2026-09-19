using UnityEngine;

public class BlastBadger : TriangularShip
{
    private BlastBadgerBehaviour blastBadgerBehaviour;
    private FlockingBehaviour flockingBehaviour;
    private FlockingComponent[] flockingComponents;

    public BlastBadger(string name) : base(name)
    {
        GameObject.layer = (int)CollisionLayer.Enemy;

        blastBadgerBehaviour = GameObject.AddComponent<BlastBadgerBehaviour>();
        GameObject.GetComponent<SpriteRenderer>().color = Utility.RED_VIOLET;
        CreateFlockingBehaviour();
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
        blastBadgerBehaviour.Activate();

        flockingBehaviour.MovementSpeed = UnitStats.HiveRaptorNormalSpeed;
    }
}
