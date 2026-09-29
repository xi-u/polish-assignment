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
    [SerializeField] private GameObject explosionParticlesPrefab;

    private void Awake()
    {
        ignoreTriggerEventsFor = new List<EntityId>();
        impactClip = Resources.Load<AudioClip>("Audio/boom");

        if (explosionParticlesPrefab == null)
        {
            explosionParticlesPrefab = CreateExplosionParticlesPrefab();
        }
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

        if (CameraBehaviour.Instance != null)
        {
            CameraBehaviour.Instance.TriggerShake(0.8f, 0.3f, 0.9f);
            CameraBehaviour.Instance.TriggerFlash(0.9f, 0.18f);
        }

        SpawnExplosionParticles();
        PlayCollisionSound();
        onTriggerEntered?.Invoke(gameObject, collider);
    }

    private void SpawnExplosionParticles()
    {
        if (explosionParticlesPrefab == null)
        {
            return;
        }

        GameObject explosion = Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
        explosion.SetActive(true);

        ParticleSystem[] particleSystems = explosion.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Play();
        }

        Destroy(explosion, 1.5f);
    }

    private GameObject CreateExplosionParticlesPrefab()
    {
        GameObject explosion = new GameObject("ExplosionParticles");
        explosion.SetActive(false);
        explosion.hideFlags = HideFlags.HideInHierarchy;

        ParticleSystem burstParticles = explosion.AddComponent<ParticleSystem>();
        var burstMain = burstParticles.main;
        burstMain.loop = false;
        burstMain.startLifetime = new ParticleSystem.MinMaxCurve(0.12f, 0.5f);
        burstMain.startSpeed = new ParticleSystem.MinMaxCurve(3f, 12f);
        burstMain.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.22f);
        burstMain.gravityModifier = 0.25f;
        burstMain.maxParticles = 140;
        burstMain.simulationSpace = ParticleSystemSimulationSpace.World;

        var burstGradient = new ParticleSystem.MinMaxGradient();
        burstGradient.mode = ParticleSystemGradientMode.TwoColors;
        burstGradient.colorMin = new Color(1f, 0.85f, 0.2f, 1f);
        burstGradient.colorMax = new Color(1f, 0.18f, 0.05f, 1f);
        burstMain.startColor = burstGradient;

        var burstEmission = burstParticles.emission;
        burstEmission.rateOverTime = 0;
        burstEmission.SetBursts(new[] { new ParticleSystem.Burst(0f, 120) });

        var burstShape = burstParticles.shape;
        burstShape.shapeType = ParticleSystemShapeType.Sphere;
        burstShape.radius = 0.2f;

        var burstRenderer = burstParticles.GetComponent<ParticleSystemRenderer>();
        burstRenderer.material = new Material(Shader.Find("Sprites/Default"));
        burstRenderer.material.color = new Color(1f, 0.7f, 0.15f, 1f);

        GameObject smoke = new GameObject("ExplosionSmoke");
        smoke.transform.SetParent(explosion.transform, false);

        ParticleSystem smokeParticles = smoke.AddComponent<ParticleSystem>();
        var smokeMain = smokeParticles.main;
        smokeMain.loop = false;
        smokeMain.startLifetime = 0.6f;
        smokeMain.startSpeed = 1.5f;
        smokeMain.startSize = 0.16f;
        smokeMain.startColor = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        smokeMain.maxParticles = 40;
        smokeMain.gravityModifier = 0.05f;

        var smokeEmission = smokeParticles.emission;
        smokeEmission.rateOverTime = 0;
        smokeEmission.SetBursts(new[] { new ParticleSystem.Burst(0f, 35) });

        var smokeShape = smokeParticles.shape;
        smokeShape.shapeType = ParticleSystemShapeType.Sphere;
        smokeShape.radius = 0.15f;

        var smokeRenderer = smokeParticles.GetComponent<ParticleSystemRenderer>();
        smokeRenderer.material = new Material(Shader.Find("Sprites/Default"));
        smokeRenderer.material.color = new Color(0.7f, 0.7f, 0.7f, 0.7f);

        return explosion;
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