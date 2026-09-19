
using UnityEngine;

public class FollowTransform : FlockingComponent
{
    private Transform followTransform;
    public Transform Transform => followTransform;

    public FollowTransform(Transform transform, float influenceMagnitude, float strength) : base(influenceMagnitude, strength)
    {
        followTransform = transform;
    }

    public override Vector2 GetDirection(Rigidbody2D self, Rigidbody2D[] neighbors)
    {
        Vector2 direction = Vector2.zero;

        Vector2 directionToTransform = ((Vector2)followTransform.position - self.position);
        if (directionToTransform.sqrMagnitude < InfluenceMagnitude * InfluenceMagnitude)
        {
            direction = directionToTransform.normalized * Strength;
        }

        return direction;
    }
}
