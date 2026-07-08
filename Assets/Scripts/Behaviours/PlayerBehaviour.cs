using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ObjectPool;

public class PlayerBehaviour : MonoBehaviour
{
	new private Rigidbody2D rigidbody;
	private Camera mainCamera;

	[SerializeField]
	private float movementSpeed = 13f;

    public float MovementSpeed { get { return movementSpeed; } set { movementSpeed = value; } }

    private float targetAngle;  // The angle we want to reach
	[SerializeField]
    private float rotationSpeed = 90f, rotationTime = 0.15f;  // The speed at which we rotate (degrees per second)
    private float currentAngle;  // The current angle of the playerTransform
    private float rotationProgress = 0;  // Progress of the current rotation

	Transform pivot;

    private void Start()
	{
		mainCamera = Camera.main;
		rigidbody = GetComponent<Rigidbody2D>();
		pivot = transform.Find("PivotPoint");
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -19.8f);
    }

	private void FixedUpdate()
	{
        Vector2 moveDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.x = 1;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection.x = -1;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection.y = 1;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection.y = -1;
        }

        float speed = moveDirection.magnitude;

        moveDirection.Normalize();
        Vector2 targetVelocity = moveDirection * movementSpeed;
        rigidbody.linearVelocity = Vector2.Lerp(rigidbody.linearVelocity, targetVelocity, (Time.deltaTime / 1.5f) * MovementSpeed);

        if (speed > 0)
        {
            RotatePlayer(moveDirection);
        }

        ConfinePlayerToWorldBoundaries();
    }

	public void ConfinePlayerToWorldBoundaries()
	{
        // Predict the next position based on the current velocity
        Vector2 nextPosition = rigidbody.position + rigidbody.linearVelocity * Time.fixedDeltaTime;

        // Create a copy of the velocity vector
        Vector2 newVelocity = rigidbody.linearVelocity;

        // Check if the next position is outside the world bounds and correct the velocity
        if (nextPosition.x < GenerateWorldBehaviour.PlayArea.xMin)
        {
            newVelocity.x = 0;
        }
        else if (nextPosition.x > GenerateWorldBehaviour.PlayArea.xMax)
        {
            newVelocity.x = 0;
        }

        if (nextPosition.y < GenerateWorldBehaviour.PlayArea.yMin)
        {
            newVelocity.y = 0;
        }
        else if (nextPosition.y > GenerateWorldBehaviour.PlayArea.yMax)
        {
            newVelocity.y = 0;
        }

        // Update the velocity
        rigidbody.linearVelocity = newVelocity;
    }

	private void RotatePlayer(Vector2 moveDirection)
	{
		// Calculate the new target angle
		float newTargetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;

		// If the target angle has changed, start a new rotation
        if (Mathf.Abs(newTargetAngle - targetAngle) > 0.01f)
        {
            targetAngle = newTargetAngle;
            currentAngle = transform.rotation.eulerAngles.z;

            // Calculate the shortest direction for the rotation
            float deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
            targetAngle = currentAngle + deltaAngle;

            rotationProgress = 0;
        }

        // Perform the rotation over several frames
        if (rotationProgress < 1)
        {
            rotationProgress += Time.fixedDeltaTime / rotationTime;
            float angle = Mathf.LerpAngle(currentAngle, targetAngle, rotationProgress);
            transform.RotateAround(pivot.position, Vector3.forward, angle - transform.eulerAngles.z);
        }
    }
}
