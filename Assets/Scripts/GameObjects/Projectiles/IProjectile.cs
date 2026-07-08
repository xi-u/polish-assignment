using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    public int InstanceId { get; }
    public int[] OwnerIds { get; }
    public void DoShoot(Vector2 direction, ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated, params int[] ownerIds);
}
