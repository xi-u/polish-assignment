using UnityEngine;

public class Separation : FlockingComponent
{
    public Separation(float influenceMagnitude, float strength) : base(influenceMagnitude, strength)
    {
    }

    public override Vector2 GetDirection(Rigidbody2D self, Rigidbody2D[] neighbors)
    {
        Vector2 separation = Vector2.zero;
        int count = 0;

        foreach (Rigidbody2D neighbor in neighbors)
        {
            if (neighbor != null && neighbor != self)
            {
                Vector2 dif = self.position - (Vector2)neighbor.transform.position;
                float dist = dif.sqrMagnitude;

                if (dist <= InfluenceMagnitude * InfluenceMagnitude)
                {
                    dif.Normalize();
                    separation += dif;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            separation /= count;
            separation.Normalize();
        }

        return separation * Strength;
    }
}
