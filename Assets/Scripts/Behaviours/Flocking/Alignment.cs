using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : FlockingComponent
{
    public Alignment(float influenceMagnitude, float strength) : base(influenceMagnitude, strength)
    {
        InfluenceMagnitude = 16;
        Strength = 3;
    }

    public override Vector2 GetDirection(Rigidbody2D self, Rigidbody2D[] neighbors)
    {
        Vector2 alignment = Vector2.zero;
        int count = 0;

        foreach (Rigidbody2D neighbor in neighbors)
        {
            if (neighbor != null && neighbor != self)
            {
                Vector2 dif = self.position - (Vector2)neighbor.position;
                if (dif.sqrMagnitude < InfluenceMagnitude * InfluenceMagnitude)
                {
                    alignment += neighbor.linearVelocity.normalized;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            alignment /= count;
            alignment.Normalize();
        }
        return alignment;
    }
}
