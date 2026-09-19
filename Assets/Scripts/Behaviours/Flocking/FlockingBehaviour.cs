using System.Collections.Generic;
using UnityEngine;
using System;

public class FlockingBehaviour : MovementBehaviour
{
    private float followDelay = 1.5f;
    private static List<Rigidbody2D> flockers = new List<Rigidbody2D>(50);
    private List<FlockingComponent> flockingComponents;
    private Rigidbody2D rb2d;
    private Transform rotationPivot;

    private void Awake()
    {
        flockingComponents = new List<FlockingComponent>();
        rb2d = GetComponent<Rigidbody2D>();
        rotationPivot = transform.Find("RotationPivot");
    }

    private void OnEnable()
    {
        flockers.Add(GetComponent<Rigidbody2D>());
    }

    private void OnDisable()
    {
        flockers.Remove(GetComponent<Rigidbody2D>());
    }

    public void AddFlockingComponent(FlockingComponent flockingComponent)
    { 
        flockingComponents.Add(flockingComponent);
    }

    public void RemoveFlockingComponent(FlockingComponent flockingComponent)
    { 
        flockingComponents.Remove(flockingComponent);
    }

    public void AddFlockingComponents(FlockingComponent[] flockingComponents)
    {
        foreach (FlockingComponent flockingComponent in flockingComponents)
        {
            AddFlockingComponent(flockingComponent);
        }
    }

    private float NeighborRadius
    {
        get
        {
            float radius = 0;
            foreach (FlockingComponent flockingComponent in flockingComponents)
            {
                radius = Mathf.Max(radius, flockingComponent.InfluenceMagnitude);
            }

            return radius;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        Vector2 velocity = rb2d.linearVelocity;

        // Add the influences of neighboring boids to the velocity.
        foreach (FlockingComponent flockingComponent in flockingComponents)
        {
            velocity += flockingComponent.GetDirection(GetComponent<Rigidbody2D>(), flockers.ToArray());
        }

        // Normalize the velocity and set it as the targetVelocity.
        velocity.Normalize();
        velocity *= MovementSpeed;
        Vector2 targetVelocity = velocity;

        // Lerp towards the target velocity.
        if (MovementSpeed > 0.1f) // Use a small threshold value to check if the speed is greater than zero
        {
            rb2d.linearVelocity = Vector2.Lerp(rb2d.linearVelocity, targetVelocity, (Time.deltaTime / followDelay) * MovementSpeed);
        }
        else
        {
            rb2d.linearVelocity = targetVelocity;
        }

        Vector2 direction = rb2d.linearVelocity.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        if (rotationPivot != null)
        {
            transform.RotateAround(rotationPivot.position, Vector3.forward, angle - transform.eulerAngles.z);
        }
        else
        {
            transform.Rotate(angle);
        }
    }
}
