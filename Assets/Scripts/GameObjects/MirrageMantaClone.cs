using ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrageMantaClone : DoubleWingedTriangularShip
{
    private EntityId[] instanceIds;
    private Sprite shipSprite;
    public MirrageMantaClone(string name) : base(name)
    {
        SetCollisionLayer(CollisionLayer.Enemy);
        AddCollider();

        Color32 color = new Color32(93, 63, 211, 255);
        Color32 colorTo = new Color(128, 128, 128, 255);
        SetColor(color);

        GameObject.AddComponent<RoamingBehaviour>().MovementSpeed = UnitStats.MirrageMantaSpeed;
        GameObject.AddComponent<ColorOscillationBehaviour>().Activate(color, colorTo, SpriteRenderers);
        SetSprite();
    }
    
    private void SetSprite()
    {
        shipSprite = Resources.Load<Sprite>("Sprites/purple-clone-ship");

        SpriteRenderer spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.transform.localScale = new  Vector3(0.5f, 0.5f, 0.5f);
        spriteRenderer.sprite = shipSprite;
        spriteRenderer.sortingOrder = 10;
        spriteRenderer.color = Color.white;

        if (leftWingSpriteRenderer != null)
            leftWingSpriteRenderer.enabled = false;

        if (rightWingSpriteRenderer != null)
            rightWingSpriteRenderer.enabled = false;
    }
    
    public void SetParentIds(params EntityId[] instanceId)
    {
        this.instanceIds = instanceId;
        collisionBehaviour.StartIgnoringTriggerEventsFor(instanceId);
    }

    protected override void HandleCollision(GameObject self, Collider2D colliderInformation)
    {
        base.HandleCollision(self, colliderInformation);
        collisionBehaviour.StopIgnoringTriggerEventsFor(this.instanceIds);
        CameraBehaviour.Instance.TriggerShake(0.5f, 0.08f);
    }

    public override void DestroySelf(bool raiseEntityDestroyedEvent = true)
    {
        Deactivate();
    }
}
