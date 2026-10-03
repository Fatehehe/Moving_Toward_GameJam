using UnityEngine;
using VContainer;

public class Inspection : MonoBehaviour
{
    private ObjectRotateService rotateService;
    private ObjectZoomService zoomService;
    private GameConfigData gameConfigData;
    private Camera camera;

    private Vector3 targetPosition;
    private Vector3 zoomVelocity = Vector3.zero;

    private float initialDistance;
    private Vector3 InitialInspectPosition;
    private Quaternion InitialInspectRotation;

    private bool isContain = false;

    [Inject]
    public void Construct(ObjectRotateService rotateService, ObjectZoomService zoomService, GameConfigData gameConfigData, Camera camera)
    {
        this.rotateService = rotateService;
        this.zoomService = zoomService;
        this.gameConfigData = gameConfigData;
        this.camera = camera;
    }

    private void Start()
    {
        targetPosition = transform.position;
        InitialInspectPosition = transform.position;
        InitialInspectRotation = transform.rotation;

        if (camera != null)
        {
            initialDistance = Vector3.Distance(camera.transform.position, transform.position);
        }
    }

    private void OnEnable()
    {
        rotateService.OnRotatePerformed += HandleRotatePerformed;
        zoomService.OnZoomPerformed += HandleZoomPerformed;
    }

    private void OnDisable()
    {
        rotateService.OnRotatePerformed -= HandleRotatePerformed;
        zoomService.OnZoomPerformed -= HandleZoomPerformed;
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref zoomVelocity,
            gameConfigData.smoothTime,
            gameConfigData.maxZoomSpeed
        );
    }

    private void HandleRotatePerformed(Vector2 delta)
    {
        float rotateY = -delta.x;
        float rotateX = delta.y;

        Vector3 cameraRight = camera.transform.right;
        Vector3 cameraUp = camera.transform.up;

        transform.Rotate(cameraUp, rotateY, Space.World);

        transform.Rotate(cameraRight, rotateX, Space.World);
    }

    private void HandleZoomPerformed(float zoomDelta)
    {
        if (!isContain) return;

        Vector3 direction = (targetPosition - camera.transform.position).normalized;
        float currentDistance = Vector3.Distance(camera.transform.position, targetPosition);

        float targetDistance = currentDistance + (zoomDelta * gameConfigData.zoomStepMultiplier);

        targetDistance = Mathf.Clamp(targetDistance, gameConfigData.minZoomDistance, initialDistance);

        targetPosition = camera.transform.position + direction * targetDistance;
    }

    public void ResetTransform()
    {
        targetPosition = camera.transform.position + (camera.transform.forward * initialDistance);
        transform.SetPositionAndRotation(InitialInspectPosition, InitialInspectRotation);
    }

    public void SetInspectionUsage(bool status) => isContain = status;

}