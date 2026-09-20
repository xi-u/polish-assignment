using ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrageMantaClone : DoubleWingedTriangularShip
{
    private EntityId[] instanceIds;
    public MirrageMantaClone(string name) : base(name)
    {
        SetCollisionLayer(CollisionLayer.Enemy);
        AddCollider();

        Color32 color = new Color32(93, 63, 211, 255);
        Color32 colorTo = new Color(128, 128, 128, 255);
        SetColor(color);

        GameObject.AddComponent<RoamingBehaviour>().MovementSpeed = UnitStats.MirrageMantaSpeed;
        GameObject.AddComponent<ColorOscillationBehaviour>().Activate(color, colorTo, SpriteRenderers);
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
