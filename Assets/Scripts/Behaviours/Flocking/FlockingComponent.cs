using System;
using UnityEngine;

public abstract class FlockingComponent
{
    public float InfluenceMagnitude { get; set; }
    public float Strength { get; set; }
    public abstract Vector2 GetDirection(Rigidbody2D self, Rigidbody2D[] neighbors);

    public FlockingComponent()
    { 
        
    }

    public FlockingComponent(float influenceMagnitude, float strength)
    { 
        InfluenceMagnitude = influenceMagnitude;
        Strength = strength;
    }
}
