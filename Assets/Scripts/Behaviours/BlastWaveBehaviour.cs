using ObjectPool;
using System.Collections.Generic;
using UnityEngine;

public class BlastWaveBehaviour : MonoBehaviour
{
    private Vector2 localPosition;

    [SerializeField]
    private float maxForce = 2000;

    public void SetLocalPosition(Vector2 position)
    {
        localPosition = position;
    }

    public void CreateBlast(float scale, bool shockwave)
    {
        if (shockwave)
        {
            CreateShockwave(scale);
        }
    }

    private void CreateShockwave(float maxScale)
    {
        Vector2 blastCenter = transform.TransformPoint(localPosition);
        List<Entity> entities = PoolManager.Instance.GetActiveEntitiesWithinRange(blastCenter, 10, PoolManager.Instance.InteractiveEntities);

        foreach (Entity entity in entities)
        {
            Rigidbody2D rb = entity.GameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (rb.gameObject != gameObject)
                {
                    // Calculate direction from the center of the blast to the entity
                    Vector2 direction = rb.position - blastCenter;

                    if (direction.sqrMagnitude < (maxScale * maxScale))
                    {
                        // If entity is within blast radius (scale), calculate and apply the force
                        rb.AddForce(direction.normalized * maxForce);
                    }
                }
            }
        }
    }
}
