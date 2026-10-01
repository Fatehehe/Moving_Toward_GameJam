using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ObjectInteractionManager : IInitializable, IDisposable
{
    private readonly ObjectDetectionService detectionService;
    private readonly ObjectPressService pressService;
    private readonly ObjectDragService dragService;

    private IInteractable currentInteract;

    public event Action OnPressStart;
    public event Action OnPressEnd;

    public event Action<IInteractable, Vector3> OnDragStarted;
    public event Action<IInteractable, Vector3> OnDragPerformed;
    public event Action<IInteractable, Vector3> OnDragEnded;

    [Inject]
    public ObjectInteractionManager(ObjectDetectionService detectionService, ObjectPressService press, ObjectDragService swipe)
    {
        this.detectionService = detectionService;
        this.pressService = press;
        this.dragService = swipe;
    }

    public void Initialize()
    {
        detectionService.OnInteractDetected += HandleInteractDetected;

        pressService.OnPressStarted += HandlePressStarted;
        pressService.OnPressEnded += HandlePressEnded;

        dragService.OnDragStarted += HandleDragStart;
        dragService.OnDragPerformed += HandleDragPerformed;
        dragService.OnDragEnded += HandleDragEnded;
    }

    public void Dispose()
    {
        detectionService.OnInteractDetected -= HandleInteractDetected;

        pressService.OnPressStarted -= HandlePressStarted;
        pressService.OnPressEnded -= HandlePressEnded;

        dragService.OnDragStarted -= HandleDragStart;
        dragService.OnDragPerformed -= HandleDragPerformed;
        dragService.OnDragEnded -= HandleDragEnded;
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
        currentInteract = interact;
    }

    private bool IsInteractValid()
    {
        if (currentInteract == null) return false;
        if (currentInteract is MonoBehaviour mono && mono == null) return false;
        return true;
    }

    private void HandlePressStarted()
    {
        OnPressStart?.Invoke();
        detectionService.SetInteractObjectUsed(true);
    }

    private void HandlePressEnded()
    {
        OnPressEnd?.Invoke();
        detectionService.SetInteractObjectUsed(false);
    }

    private void HandleDragStart(Vector2 vector)
    {
        if (!IsInteractValid()) return;

        detectionService.SetInteractObjectUsed(true);
        detectionService.CacheDragDepth(currentInteract);

        Vector3 worldPos = detectionService.ScreenToWorld(vector, currentInteract);
        OnDragStarted?.Invoke(currentInteract, worldPos);
    }

    private void HandleDragPerformed(Vector2 vector)
    {
        if (!IsInteractValid()) return;

        Vector3 worldPos = detectionService.ScreenToWorld(vector, currentInteract);
        OnDragPerformed?.Invoke(currentInteract, worldPos);
    }

    private void HandleDragEnded(Vector2 vector)
    {
        if (IsInteractValid())
        {
            Vector3 worldPos = detectionService.GetCachedDragWorldPos(vector);
            OnDragEnded?.Invoke(currentInteract, worldPos);
        }

        detectionService.SetInteractObjectUsed(false);
    }
}