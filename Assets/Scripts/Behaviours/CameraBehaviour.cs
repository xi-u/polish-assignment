using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraBehaviour : MonoBehaviour
{
    private Rect cameraRect;
    public Rect CameraRect => cameraRect;
    private Transform playerTransform;
    private Rigidbody2D playerRB;
    
    private Camera mainCamera;
    private Player player;

    private float smoothing = 5f;
    private Vector3 offset = Vector3.back * 20;

    [SerializeField]
    private float zoomOutFactor = 12f, cameraMoveAheadFactor = 0.5f, cameraSlowDownFactor = 0.5f;
    private Vector2 lastTargetPosition;

    void Start()
    {
        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        //return;
        if (playerTransform == null)
        {
            player = ServiceLocator.Instance.GetService<Player>();
            playerTransform = player.Transform;
        }

        // Zoom out slightly when the playerTransform is moving
        Vector2 moveDelta = (Vector2)playerTransform.position - lastTargetPosition;
        float moveDistance = moveDelta.magnitude;
        //mainCamera.orthographicSize = 5 + (moveDistance * zoomOutFactor);
        //mainCamera.transform.position = playerTransform.position + Vector3.back * 20;

        // Move the camera ahead in the direction the playerTransform is moving
        Vector2 targetCamPos = (Vector2)playerTransform.position + player.Velocity * cameraMoveAheadFactor;

        float cameraMoveSpeed = smoothing * Time.deltaTime * (1 - cameraSlowDownFactor);
        transform.position = Vector3.Lerp(transform.position, targetCamPos, cameraMoveSpeed) + offset;
        lastTargetPosition = playerTransform.position;
    }

    private void UpdateCameraRect()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        cameraRect = new Rect(bottomLeft.x, bottomLeft.y, cameraWidth, cameraHeight);        
    }

    void LateUpdate()
    {
        /*if (playerTransform == null)
        {
            player = ServiceLocator.Instance.GetService<Player>();
            playerTransform = player.Transform;
        }

        mainCamera.transform.position = playerTransform.position + Vector3.back * 20;*/
        UpdateCameraRect();
    }
}