using ObjectPool;
using System.Collections;
using UnityEngine;

public class FollowPlayerBehaviour : MovementBehaviour
{
    [SerializeField]
    private float followDelay = 0.5f, rotationSpeed = 2.25f;
    private Transform playerTransform;    

    public float FollowDelay { get => followDelay; set => followDelay = value; }
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }

    // Start is called before the first frame update
    private void Start()
    {
        playerTransform = ServiceLocator.Instance.GetService<Player>().Transform;
    }
    
    private void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector2 targetPosition = Vector2.Lerp(playerTransform.position, transform.position, Time.deltaTime / FollowDelay);
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MovementSpeed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.RotateSlerp(angle, rotationSpeed);
    }
}