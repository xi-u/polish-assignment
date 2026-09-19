using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkWolf : TriangularShip
{
    BlinkWolfBehaviour blinkWolfBehaviour;
    private FlockingBehaviour flockingBehaviour;
    private FlockingComponent[] flockingComponents;
    public BlinkWolf(string name) : base(name)
    {
        GameObject.layer = (int)CollisionLayer.Enemy;
        blinkWolfBehaviour = GameObject.AddComponent<BlinkWolfBehaviour>();

        GameObject.GetComponent<SpriteRenderer>().color = new Color32(92, 255, 255, 255);
        CreateFlockingBehaviour();

        flockingBehaviour.MovementSpeed = UnitStats.BlinkWolfNormalSpeed;        
    }

    private void CreateFlockingBehaviour()
    {
        flockingComponents = new FlockingComponent[4]
        {
            new Cohesion(5, 3),
            new Separation(3, 7),
            new Alignment(4, 4),
            new FollowTransform(ServiceLocator.Instance.GetService<Player>().Transform, float.PositiveInfinity, 3)
        };

        flockingBehaviour = GameObject.AddComponent<FlockingBehaviour>();        
        flockingBehaviour.AddFlockingComponents(flockingComponents);
    }

    public void EnterChargeMode()
    {
        flockingComponents[3].Strength = 7;
        flockingBehaviour.MovementSpeed = UnitStats.BlinkWolfChaseSpeed;
    }

    public void ExitChargeMode()
    {
        flockingComponents[3].Strength = 3;
        flockingBehaviour.MovementSpeed = UnitStats.BlinkWolfNormalSpeed;
    }

    public override void Activate(Vector2 position)
    {
        base.Activate(position);
        blinkWolfBehaviour.Activate(this);
        ExitChargeMode();
    }

    public override void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        DestroySelf();      
    }
}
