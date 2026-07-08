using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : FlockingComponent
{   
    public Cohesion(float influenceMagnitude, float strength) : base(influenceMagnitude, strength)
    { 
        
    }

    public override Vector2 GetDirection(Rigidbody2D self, Rigidbody2D[] neighbors)
    {
        Vector2 cohesion = Vector2.zero;
        int count = 0;

        foreach (Rigidbody2D neighbor in neighbors)
        {
            if (neighbor != null && neighbor != self)
            {
                Vector2 dif = (Vector2)neighbor.position - self.position;

                float dist = Vector3.SqrMagnitude(dif);
                if (dist <= InfluenceMagnitude * InfluenceMagnitude)
                {
                    //normalize the difference and add it to the result.
                    cohesion += (Vector2)neighbor.transform.position;
                    count++;
                }
            }
        }

        if (count > 0)
        {            
            cohesion /= count;
            Vector3 dir = cohesion - self.position;
            return dir.normalized;
        }
        return cohesion;
    }
}
