using System;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using UnityEngine;

public class CollisionBehaviour : MonoBehaviour
{
    public delegate void OnTriggerEntered(GameObject self, Collider2D colliderInformation);
    private OnTriggerEntered onTriggerEntered;
    
    private List<int> ignoreTriggerEventsFor;

    private void Awake()
    {
        ignoreTriggerEventsFor = new List<int>();    
    }

    public void SubscribeToTriggerEnteredEvent(OnTriggerEntered onTriggerEntered)
    {
        this.onTriggerEntered = onTriggerEntered;
    }

    public void StartIgnoringTriggerEventsFor(params int[] instanceId)
    {
        ignoreTriggerEventsFor.AddRange(instanceId);
    }
    public void StopIgnoringTriggerEventsFor(params int[] instanceIds)
    {
        foreach (int instanceId in instanceIds)
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
        if (ignoreTriggerEventsFor.Contains(collider.gameObject.GetInstanceID()))
        {
            return;
        }
        onTriggerEntered(gameObject, collider);
    }
}