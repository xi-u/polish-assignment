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
    
    private AudioClip impactClip;

    private void Awake()
    {
        ignoreTriggerEventsFor = new List<EntityId>();
        impactClip = Resources.Load<AudioClip>("Audio/boom");
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

        CameraBehaviour.Instance.TriggerShake(0.5f, 0.2f, 0.75f);
        PlayCollisionSound();
        onTriggerEntered?.Invoke(gameObject, collider);
    }
    
    public void PlayCollisionSound()
    {
        if (impactClip == null)
        {
            return;
        }

        if (AudioManager.Instance == null)
        {
            GameObject audioManagerObject = new GameObject("AudioManager");
            audioManagerObject.AddComponent<AudioManager>();
        }

        AudioManager.Instance.Play(impactClip, transform.position, 0.6f);
    }
}