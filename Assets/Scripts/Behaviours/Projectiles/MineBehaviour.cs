using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBehaviour : MonoBehaviour
{
    private Mine owner;
    private ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated;

    private EntityId bodyInstanceId;
    public EntityId BodyInstanceId { set { bodyInstanceId = value; } }

    private float rotation = 0;

    [SerializeField]
    private float rotationSpeed = 1.5f, amplitude = 0.5f, speed = 1.5f;

    private Vector2 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation);
        rotation += rotationSpeed;

        float newY = initialPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector2(initialPosition.x, newY);
    }

    public void DoShoot(Mine owner, Vector2 initialDirection, ShootBehaviour.OnProjectileStateChanged onProjectileDeactivated)
    {
        initialPosition = transform.position;
        this.owner = owner;
        this.onProjectileDeactivated = onProjectileDeactivated;
        StartCoroutine(CanShootOwner());
    }

    private IEnumerator CanShootOwner()
    {
        yield return new WaitForSeconds(1.5f);
        onProjectileDeactivated(bodyInstanceId);
        GetComponent<CollisionBehaviour>().StopIgnoringTriggerEventsFor(owner.OwnerIds);
    }
}
