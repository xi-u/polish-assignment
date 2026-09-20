using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public static CameraBehaviour Instance { get; private set; }

    private Rect cameraRect;
    public Rect CameraRect => cameraRect;
    private Transform playerTransform;
    private Rigidbody2D playerRB;

    private Camera mainCamera;
    private Player player;

    private float smoothing = 5f;
    private Vector3 offset = Vector3.back * 20;

    private Vector3 shakeOffset;
    private float shakeMagnitude;
    private float shakeDuration;
    private float shakeDecay = 2f;

    [SerializeField]
    private float zoomOutFactor = 12f, cameraMoveAheadFactor = 0.5f, cameraSlowDownFactor = 0.5f;
    private Vector2 lastTargetPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void TriggerShake(float strength, float duration = 0.2f, float decay = 2f)
    {
        shakeMagnitude = Mathf.Max(shakeMagnitude, strength);
        shakeDuration = Mathf.Max(shakeDuration, duration);
        shakeDecay = decay;
    }

    private void UpdateShake()
    {
        if (shakeDuration <= 0f)
        {
            shakeOffset = Vector3.zero;
            shakeMagnitude = 0f;
            return;
        }

        shakeOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * shakeMagnitude;
        shakeDuration -= Time.deltaTime;
        shakeMagnitude = Mathf.Max(0f, shakeMagnitude - shakeDecay * Time.deltaTime);

        if (shakeDuration <= 0f)
        {
            shakeDuration = 0f;
            shakeMagnitude = 0f;
            shakeOffset = Vector3.zero;
        }
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
        Vector3 followPosition = Vector3.Lerp(transform.position, targetCamPos, cameraMoveSpeed) + offset;

        UpdateShake();
        transform.position = followPosition + shakeOffset;
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