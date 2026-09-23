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
    
    private AudioSource audioSource;
    

    private void Awake()
    {
        ignoreTriggerEventsFor = new List<EntityId>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.enabled = true;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 0.6f;
        audioSource.clip = Resources.Load<AudioClip>("Audio/boom");
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
        CameraBehaviour.Instance.TriggerShake(0.1f, 0.08f, 0.5f);
        PlayCollisionSound();
    }
    
    public void PlayCollisionSound()
    {
        if (audioSource == null || audioSource.clip == null)
        {
            return;
        }

        if (!gameObject.activeSelf || !audioSource.enabled)
        {
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position, audioSource.volume);
            return;
        }

        audioSource.enabled = true;
        audioSource.PlayOneShot(audioSource.clip);
    }
}