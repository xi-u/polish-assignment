using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkWolf : TriangularShip
{
    BlinkWolfBehaviour blinkWolfBehaviour;
    private FlockingBehaviour flockingBehaviour;
    private FlockingComponent[] flockingComponents;
    private Sprite shipSprite;
    private Transform visual;
    private readonly Vector3 visualScale = new Vector3(0.4f, 0.4f, 0.4f);

    public BlinkWolf(string name) : base(name)
    {
        CreateVisual();
        SetSprite();
        GameObject.layer = (int)CollisionLayer.Enemy;

        SpriteRenderer rootSpriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (rootSpriteRenderer != null)
        {
            rootSpriteRenderer.enabled = false;
        }

        blinkWolfBehaviour = GameObject.AddComponent<BlinkWolfBehaviour>();
        CreateFlockingBehaviour();
        flockingBehaviour.MovementSpeed = UnitStats.BlinkWolfNormalSpeed;
    }
    
    private void CreateVisual()
    {
        GameObject visualObject = new GameObject("Visual");
        visualObject.transform.SetParent(GameObject.transform, false);
        visual = visualObject.transform;
    }
    
    private void SetSprite()
    {
        SpriteRenderer spriteRenderer = visual.gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/cyan-ship");
        spriteRenderer.sortingOrder = 10;
        spriteRenderer.color = new Color32(92, 255, 255, 255);
        visual.localScale = visualScale;
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
        CameraBehaviour.Instance.TriggerShake(0.5f, 0.08f);
    }
}
