using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ObjectDetectionService : IInitializable, IDisposable
{
    private readonly Camera cam;
    private readonly InputSystemService inputSystemService;
    private IInteractable currentInteractable;
    private bool isUsed;
    private float cachedDragDepth;

    public Action<IInteractable> OnInteractDetected;

    [Inject]
    public ObjectDetectionService(InputSystemService inputSystemService, Camera cam)
    {
        this.inputSystemService = inputSystemService;
        this.cam = cam;
    }

    public void Initialize()
    {
        inputSystemService.OnMouseMoved += HandleMouseMove;
    }

    public void Dispose()
    {
        inputSystemService.OnMouseMoved -= HandleMouseMove;
    }

    private void HandleMouseMove(Vector2 screenPos)
    {
        if (!IsValidPosition(screenPos) || isUsed) return;

        IInteractable newTarget = null;
        if (TryRaycast(screenPos, out RaycastHit hit))
        {
            hit.collider.TryGetComponent(out newTarget);
        }

        if (newTarget != currentInteractable)
        {
            currentInteractable?.OnInteractEnded();
            currentInteractable = newTarget;

            // if (currentInteractable != null)
            // {
            //     Debug.Log("detecting object: " + currentInteractable.GetType().Name);
            // }

            currentInteractable?.OnInteractDetected();
            OnInteractDetected?.Invoke(currentInteractable);
        }
    }

    public void SetInteractObjectUsed(bool isUsed)
    {
        this.isUsed = isUsed;
        OnInteractDetected?.Invoke(currentInteractable);
    }

    public bool TryRaycast(Vector2 screenPos, out RaycastHit hit)
    {
        hit = default;
        if (!IsValidPosition(screenPos)) return false;

        Ray ray = cam.ScreenPointToRay(screenPos);
        return Physics.Raycast(ray, out hit);
    }

    public void CacheDragDepth(IInteractable target)
    {
        if (target is Component targetComp && targetComp != null)
        {
            cachedDragDepth = cam.WorldToScreenPoint(targetComp.transform.position).z;
        }
    }

    public Vector3 GetCachedDragWorldPos(Vector2 screenPos)
    {
        if (!IsValidPosition(screenPos)) return Vector3.zero;

        Vector3 screenPosWithDepth = new Vector3(screenPos.x, screenPos.y, cachedDragDepth);
        return cam.ScreenToWorldPoint(screenPosWithDepth);
    }

    public Vector3 ScreenToWorld(Vector2 screenPos, IInteractable target)
    {
        if (!IsValidPosition(screenPos) || target == null) return Vector3.zero;

        if (target is Component targetComp)
        {
            float zDistance = cam.WorldToScreenPoint(targetComp.transform.position).z;
            Vector3 screenPosWithDepth = new Vector3(screenPos.x, screenPos.y, zDistance);
            return cam.ScreenToWorldPoint(screenPosWithDepth);
        }
        return Vector3.zero;
    }

    private bool IsValidPosition(Vector2 pos)
    {
        return !float.IsInfinity(pos.x) && !float.IsInfinity(pos.y) && !float.IsNaN(pos.x) && !float.IsNaN(pos.y);
    }
}