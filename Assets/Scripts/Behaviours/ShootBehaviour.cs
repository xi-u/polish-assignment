using ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBehaviour : MonoBehaviour
{
    public delegate void OnProjectileStateChanged(EntityId instanceId);
    private OnProjectileStateChanged onProjectileActivated, onProjectileDeactivated;

    [SerializeField]
    private float speedChangeDuration = 0.66f, bulletSpeed = 12f,
        shootingInterval = 5f, waitForShooting = 0.5f;

    private float currentSpeed;
    private EntityId[] ownerIds;

    MovementBehaviour movementBehaviour;

    private PoolableType poolableType;
    public PoolableType PoolableType
    {
        get => poolableType;
        set
        {
            poolableType = value;
        }
    }

    public float SpeedChangeDuration { get => speedChangeDuration; set => speedChangeDuration = value; }
    public float BulletSpeed { get => bulletSpeed; set => bulletSpeed = value; }
    public float ShootingInterval { get => shootingInterval; set => shootingInterval = value; }
    public float WaitForShooting { get => waitForShooting; set => waitForShooting = value; }

    public void Activate(params EntityId[] ownerId)
    {
        if (movementBehaviour == null)
        {
            movementBehaviour = GetComponent<MovementBehaviour>();
        }
        
        StartCoroutine(ShootAtPlayer());
        
        this.ownerIds = ownerId;
    }

    public void SubscribeToProjectileStateChangedEvent(OnProjectileStateChanged activated, OnProjectileStateChanged deactivated)
    {
        this.onProjectileActivated = activated;
        this.onProjectileDeactivated = deactivated;
    }


    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            for (float t = 0; t < SpeedChangeDuration; t += Time.deltaTime)
            {
                currentSpeed = Mathf.Lerp(movementBehaviour.MovementSpeed, 0, t / SpeedChangeDuration);
                yield return null;
            }

            Vector2 dir = transform.right.normalized;
            float rads = 90f * Mathf.Deg2Rad;
            Vector2 shootDirection = new Vector2(dir.x * Mathf.Cos(rads) - dir.y * Mathf.Sin(rads), dir.x * Mathf.Sin(rads) + dir.y * Mathf.Cos(rads)).normalized * BulletSpeed;

            PoolManager.Instance.GetEntity(PoolableType, transform.position, (Entity e) =>
            {
                IProjectile projectile = e as IProjectile;
                onProjectileActivated(projectile.InstanceId);
                projectile.DoShoot(shootDirection, onProjectileDeactivated, ownerIds);
            });

            yield return new WaitForSeconds(WaitForShooting);

            for (float t = 0; t < SpeedChangeDuration; t += Time.deltaTime)
            {
                currentSpeed = Mathf.Lerp(0, movementBehaviour.MovementSpeed, t / SpeedChangeDuration);
                yield return null;
            }

            yield return new WaitForSeconds(ShootingInterval);
        }
    }
}
