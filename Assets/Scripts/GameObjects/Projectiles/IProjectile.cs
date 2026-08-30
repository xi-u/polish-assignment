using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    public EntityId InstanceId { get; }
    public EntityId[] OwnerIds { get; }
    public void DoShoot(Vector2 direction, ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated, params EntityId[] ownerIds);
}
