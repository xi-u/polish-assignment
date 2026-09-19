using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionHandler
{
    EntityId InstanceId { get; }
    Entity Entity { get; }
    void HandleCollision(Collider2D colliderInformation, ICollisionHandler other, ICollisionHandler self);
}