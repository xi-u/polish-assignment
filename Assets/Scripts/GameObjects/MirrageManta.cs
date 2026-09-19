using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MirrageManta : DoubleWingedTriangularShip
{
    private MirrageMantaBehaviour mirrageMantaBehaviour;
    private EntityId[] childIds = new EntityId[0];

    public MirrageManta(string name) : base(name)
    {
        SetCollisionLayer(CollisionLayer.Enemy);
        AddCollider();

        SetColor(new Color32(93, 63, 211, 255));

        GameObject.AddComponent<RoamingBehaviour>().MovementSpeed = UnitStats.MirrageMantaSpeed;
        mirrageMantaBehaviour = GameObject.AddComponent<MirrageMantaBehaviour>();        
    }

    public override void Activate(Vector2 position)
    {
        base.Activate(position);
        mirrageMantaBehaviour.Activate(this);
    }

    public void SetChildIds(params EntityId[] childIds)
    {
        this.childIds = childIds;
        collisionBehaviour.StartIgnoringTriggerEventsFor(childIds);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        collisionBehaviour.StopIgnoringTriggerEventsFor(childIds);
    }
}
