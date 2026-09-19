using UnityEngine;

public class RoamingBehaviour : MovementBehaviour
{
    private FlockingBehaviour flockingBehaviour;
    private FlockingComponent[] flockingComponents;
    private Transform waypoint;
    private Transform playerTransform;

    [SerializeField]
    private float distanceToCreateNewWaypoint = 3f;

    [SerializeField]
    private float followPlayerDistance = 5f;

    public override float MovementSpeed { get => flockingBehaviour.MovementSpeed; set => flockingBehaviour.MovementSpeed = value; }

    // Start is called before the first frame update
    private void Awake()
    {
        CreateWaypoint();
        UpdateWaypoint();
        playerTransform = ServiceLocator.Instance.GetService<Player>().Transform;

        flockingComponents = new FlockingComponent[5]
        {
            new Cohesion(5, 3),
            new Separation(3, 4),
            new Alignment(4, 4),
            new FollowTransform(waypoint, float.PositiveInfinity, 4),
            new FollowTransform(playerTransform, 5, 8)
        };

        flockingBehaviour = gameObject.AddComponent<FlockingBehaviour>();
        flockingBehaviour.AddFlockingComponents(flockingComponents);
    }

    private void OnEnable()
    {        
        UpdateWaypoint();
    }

    private void CreateWaypoint()
    {
        GameObject gameObject = new GameObject("Waypoint");
        waypoint = gameObject.transform;
        waypoint.SetParent(transform.parent, false);
    }

    private void UpdateWaypoint()
    {
        waypoint.position = Utility.GetRandomPositionInsideRect(GenerateWorldBehaviour.PlayArea);
    }

    // Update is called once per frame
    private void Update()
    {
        float sqrMagnitudeToWaypoint = Vector2.SqrMagnitude(waypoint.position - transform.position);
        if (sqrMagnitudeToWaypoint < distanceToCreateNewWaypoint * distanceToCreateNewWaypoint)
        {
            UpdateWaypoint();
        }        
        //flockingComponents[4].InfluenceMagnitude = (sqrMagnitudeToPlayer < followPlayerDistance * followPlayerDistance) ? 3 : 0;         
    }
}
