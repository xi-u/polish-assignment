using System.Collections;
using UnityEngine;

public class FollowBehaviour : MonoBehaviour
{
    private float followDelay = 0.5f, speed = 3.5f, rotationSpeed = 2.25f;
    private Transform playerTransform;

    public Transform PlayerTransform => playerTransform;

    public float Speed { get { return speed; } set { speed = value; } }

    private Vector2 direction;
    public Vector2 Direction => direction;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = ServiceLocator.Instance.GetService<Player>().Transform;
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector2 targetPosition = Vector2.Lerp(playerTransform.position, transform.position, Time.deltaTime / followDelay);
        direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.RotateSlerp(angle, rotationSpeed);
    }
}
