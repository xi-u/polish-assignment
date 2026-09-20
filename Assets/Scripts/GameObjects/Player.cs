using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Player : DoubleWingedTriangularShip
{
    // Define a delegate for the event
    public delegate void PlayerDiedDelegate(GameObject sender);

    // Create an event based on the delegate
    public static event PlayerDiedDelegate OnPlayerDied;

    public Entity Entity => this;
	public EntityId InstanceId => GameObject.GetEntityId();

    private PlayerBehaviour playerBehaviour;
    
    public Player(string name, Vector2 startPosition) : base(name)
	{
		AddCollider();
		GameObject.tag = "Player";
        GameObject.layer = (int)CollisionLayer.Player;
        playerBehaviour = GameObject.AddComponent<PlayerBehaviour>();

        GameObject pivotPoint = new GameObject("PivotPoint");
        pivotPoint.transform.SetParent(Transform, false);
        pivotPoint.transform.localPosition = Vector2.down * 0.5f;

        SetColor(Color.green);
		Activate(startPosition);
    }

    public Vector2 Velocity => rigidbody.linearVelocity;

    protected override void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        // OnPlayerDied?.Invoke(GameObject);
        // DestroySelf();
        
        // TODO: for testing returbn
        return;
    }
}