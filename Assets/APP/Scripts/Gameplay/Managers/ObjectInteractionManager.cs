using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ObjectInteractionManager : IInitializable, IDisposable
{
    private readonly ObjectDetectionService detectionService;
    private readonly ObjectPressService pressService;
    private readonly ObjectDragService dragService;
    private readonly Inspection inspection;
    private Vector3 inspectionUp = Vector3.up;
    private readonly Camera camera;

    private IInteractable currentInteract;

    public event Action OnPressStart;
    public event Action OnPressEnd;

    public event Action<IInteractable, Vector3> OnDragStarted;
    public event Action<IInteractable, Vector3> OnDragPerformed;
    public event Action<IInteractable, Vector3> OnDragEnded;

    public event Action<IInteractable> OnHoldCompleted;
    public event Action<IInteractable, float, Vector2> OnHoldPerformed;
    public event Action<IInteractable> OnHoldCanceled;

    private bool isGameEnded = false;

    [Inject]
    public ObjectInteractionManager(ObjectDetectionService detectionService, ObjectPressService press, ObjectDragService swipe, Inspection inspection, Camera camera)
    {
        this.detectionService = detectionService;
        this.pressService = press;
        this.dragService = swipe;
        this.inspection = inspection;
        this.camera = camera;
    }

    public void Initialize()
    {
        GameEvents.OnAssemblingFinished += HandleAssemblingFinished;

        detectionService.OnInteractDetected += HandleInteractDetected;

        pressService.OnPressStarted += HandlePressStarted;
        pressService.OnPressEnded += HandlePressEnded;
        pressService.OnHoldCompleted += HandleHoldCompleted;
        pressService.OnHoldPerformed += HandleHoldPerformed;
        pressService.OnHoldCanceled += HandleHoldCanceled;

        dragService.OnDragStarted += HandleDragStart;
        dragService.OnDragPerformed += HandleDragPerformed;
        dragService.OnDragEnded += HandleDragEnded;

        inspectionUp = inspection.transform.up;
    }

    public void Dispose()
    {
        GameEvents.OnAssemblingFinished -= HandleAssemblingFinished;

        detectionService.OnInteractDetected -= HandleInteractDetected;

        pressService.OnPressStarted -= HandlePressStarted;
        pressService.OnPressEnded -= HandlePressEnded;
        pressService.OnHoldCompleted -= HandleHoldCompleted;
        pressService.OnHoldPerformed -= HandleHoldPerformed;
        pressService.OnHoldCanceled -= HandleHoldCanceled;

        dragService.OnDragStarted -= HandleDragStart;
        dragService.OnDragPerformed -= HandleDragPerformed;
        dragService.OnDragEnded -= HandleDragEnded;
    }

    private void HandleAssemblingFinished()
    {
        isGameEnded = true;
        ForceDropCurrentObject();
    }

    public void ForceDropCurrentObject()
    {
        if (currentInteract != null)
        {
            Vector3 dropPosition = Vector3.zero;
            if (currentInteract is MonoBehaviour monoBehaviour)
            {
                dropPosition = monoBehaviour.transform.position;
            }

            OnDragEnded?.Invoke(currentInteract, dropPosition);
            currentInteract = null;
        }

        detectionService.SetInteractObjectUsed(false);
    }

    private void HandleInteractDetected(IInteractable interact)
    {
        if (isGameEnded) return;
        currentInteract = interact;
        Debug.Log($"[ObjectInteractionManager] HandleInteractDetected: {interact}");
    }

    private bool IsInteractValid()
    {
        if (isGameEnded) return false;
        if (currentInteract == null) return false;
        if (currentInteract is MonoBehaviour mono && mono == null) return false;
        return true;
    }

    private void HandlePressStarted()
    {
        if (isGameEnded) return;
        OnPressStart?.Invoke();
        detectionService.SetInteractObjectUsed(true);
    }

    private void HandlePressEnded()
    {
        if (isGameEnded) return;
        OnPressEnd?.Invoke();
        detectionService.SetInteractObjectUsed(false);
    }

    private void HandleHoldPerformed(float value, Vector2 screenPos)
    {
        if (!IsInteractValid()) return;
        OnHoldPerformed?.Invoke(currentInteract, value, screenPos);
    }

    private void HandleHoldCompleted()
    {
        if (!IsInteractValid()) return;
        OnHoldCompleted?.Invoke(currentInteract);
    }

    private void HandleHoldCanceled()
    {
        if (!IsInteractValid()) return;
        OnHoldCanceled?.Invoke(currentInteract);
    }

    private void HandleDragStart(Vector2 vector)
    {
        if (!IsInteractValid()) return;
        Debug.Log($"[ObjectInteractionManager] HandleDragStart: {currentInteract}");
        detectionService.SetInteractObjectUsed(true);
        Vector3 worldPos = GetInspectionPlaneWorldPosition(vector);
        OnDragStarted?.Invoke(currentInteract, worldPos);
    }

    private void HandleDragPerformed(Vector2 vector)
    {
        Debug.Log($"[ObjectInteractionManager] HandleDragPerformed: {currentInteract}");

        if (!IsInteractValid()) return;
        Vector3 worldPos = GetInspectionPlaneWorldPosition(vector);
        OnDragPerformed?.Invoke(currentInteract, worldPos);
    }

    private void HandleDragEnded(Vector2 vector)
    {
        Debug.Log($"[ObjectInteractionManager] HandleDragEnded: {currentInteract}");
        if (IsInteractValid())
        {
            Vector3 worldPos = GetInspectionPlaneWorldPosition(vector);
            OnDragEnded?.Invoke(currentInteract, worldPos);
        }

        detectionService.SetInteractObjectUsed(false);
    }

    private Vector3 GetInspectionPlaneWorldPosition(Vector2 screenPos)
    {
        if (camera == null || inspection == null) return Vector3.zero;

        Ray ray = camera.ScreenPointToRay(screenPos);
        Plane dragPlane = new(inspectionUp, inspection.transform.position);

        if (dragPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}