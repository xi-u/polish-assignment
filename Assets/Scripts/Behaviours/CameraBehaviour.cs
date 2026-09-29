using UnityEngine;
using UnityEngine.UI;

public class CameraBehaviour : MonoBehaviour
{
    public static CameraBehaviour Instance { get; private set; }

    private Rect cameraRect;
    private Canvas flashCanvas;
    private Image flashImage;
    private float flashAlpha;
    private float flashFadePerSecond;
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
            return;
        }

        CreateScreenFlash();
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    private void CreateScreenFlash()
    {
        GameObject flashObject = new GameObject("ScreenFlash");
        flashObject.transform.SetParent(transform, false);

        flashCanvas = flashObject.AddComponent<Canvas>();
        flashCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        flashCanvas.sortingOrder = 1000;

        flashObject.AddComponent<CanvasScaler>();

        GameObject flashImageObject = new GameObject("FlashImage");
        flashImageObject.transform.SetParent(flashObject.transform, false);

        RectTransform flashRect = flashImageObject.AddComponent<RectTransform>();
        flashRect.anchorMin = Vector2.zero;
        flashRect.anchorMax = Vector2.one;
        flashRect.offsetMin = Vector2.zero;
        flashRect.offsetMax = Vector2.zero;

        flashImage = flashImageObject.AddComponent<Image>();
        flashImage.color = new Color(1f, 1f, 1f, 0f);
        flashImage.raycastTarget = false;
    }

    public void TriggerShake(float strength, float duration = 0.2f, float decay = 2f)
    {
        shakeMagnitude = Mathf.Max(shakeMagnitude, strength);
        shakeDuration = Mathf.Max(shakeDuration, duration);
        shakeDecay = decay;
    }

    public void TriggerFlash(float intensity = 0.8f, float duration = 0.15f)
    {
        if (flashImage == null)
        {
            return;
        }

        flashAlpha = Mathf.Max(flashAlpha, intensity);
        flashFadePerSecond = Mathf.Max(0.1f, intensity / Mathf.Max(duration, 0.05f));
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

    private void Update()
    {
        if (flashImage == null)
        {
            return;
        }

        if (flashAlpha > 0f)
        {
            flashAlpha = Mathf.Max(0f, flashAlpha - flashFadePerSecond * Time.deltaTime);
            flashImage.color = new Color(1f, 1f, 1f, flashAlpha);
        }
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