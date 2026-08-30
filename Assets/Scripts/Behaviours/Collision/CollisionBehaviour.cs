using System;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using UnityEngine;

public class CollisionBehaviour : MonoBehaviour
{
    public delegate void OnTriggerEntered(GameObject self, Collider2D colliderInformation);
    private OnTriggerEntered onTriggerEntered;
    
    private List<EntityId> ignoreTriggerEventsFor;

    private void Awake()
    {
        ignoreTriggerEventsFor = new List<EntityId>();
    }

    public void SubscribeToTriggerEnteredEvent(OnTriggerEntered onTriggerEntered)
    {
        this.onTriggerEntered = onTriggerEntered;
    }

    public void StartIgnoringTriggerEventsFor(params EntityId[] instanceId)
    {
        ignoreTriggerEventsFor.AddRange(instanceId);
    }
    public void StopIgnoringTriggerEventsFor(params EntityId[] instanceIds)
    {
        foreach (EntityId instanceId in instanceIds)
        { 
            ignoreTriggerEventsFor.Remove(instanceId);
        }
    }

    public void ClearIgnoreTriggerEvents()
    { 
        ignoreTriggerEventsFor.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (ignoreTriggerEventsFor.Contains(collider.gameObject.GetEntityId()))
        {
            return;
        }
        onTriggerEntered(gameObject, collider);
    }
}