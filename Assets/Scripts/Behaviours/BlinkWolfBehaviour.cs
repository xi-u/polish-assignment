using ObjectPool;
using System.Collections;
using UnityEngine;

public class BlinkWolfBehaviour : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private Transform playerTransform;
    private MovementBehaviour movementBehaviour;
    private float speedChangeDuration = 0.66f;
    private BlinkWolf blinkWolf;
    private Transform visual;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerTransform = ServiceLocator.Instance.GetService<Player>().Transform;
        movementBehaviour = GetComponent<MovementBehaviour>();
        
        visual = transform.Find("Visual");
    }

    public void Activate(BlinkWolf blinkWolf)
    {
        this.blinkWolf = blinkWolf;
        StartCoroutine(BlinkForward());
        transform.localScale = Vector3.one;
        if (visual != null)
        {
            visual.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
    }

    private void FixedUpdate()
    {

    }

    private IEnumerator BlinkForward()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 4.5f));
            float speed = movementBehaviour.MovementSpeed;

            for (float t = 0; t <= speedChangeDuration; t += Time.deltaTime)
            {
                movementBehaviour.MovementSpeed = Mathf.Lerp(speed, 0, t / speedChangeDuration);
                yield return null;
            }

            Vector2 playerPosition = playerTransform.position;
            Vector2 direction = playerPosition - (Vector2)transform.position;
            Vector2 targetLocation = rigidbody.position + direction.normalized * (direction.magnitude - 1);

            for (float t = 0; t < speedChangeDuration; t += Time.deltaTime)
            {
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / speedChangeDuration);
                yield return null;
            }

            transform.position = targetLocation;

            for (float t = 0; t < speedChangeDuration; t += Time.deltaTime)
            {
                movementBehaviour.MovementSpeed = Mathf.Lerp(0, speed, t / speedChangeDuration);
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / speedChangeDuration);
                yield return null;
            }

            blinkWolf.EnterChargeMode();
            yield return new WaitForSeconds(1);
            blinkWolf.ExitChargeMode();
        }
    }
}