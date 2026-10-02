using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ToyManager : IInitializable, IDisposable
{
    private readonly ObjectInteractionManager objectInteractionManager;
    private readonly AssembleService assemblyService;
    private readonly ToolService toolService;
    private readonly Inspection inspection;

    private IToyPart currentDraggedPart;

    [Inject]
    public ToyManager(ObjectInteractionManager objectInteractionManager, AssembleService assemblyService,
    Inspection inspection, ToolService toolService)
    {
        this.objectInteractionManager = objectInteractionManager;
        this.assemblyService = assemblyService;
        this.inspection = inspection;
        this.toolService = toolService;
    }

    public void Initialize()
    {
        objectInteractionManager.OnHoldCompleted += HandleHoldCompleted;
        objectInteractionManager.OnHoldCanceled += HandleHoldCanceled;

        objectInteractionManager.OnDragStarted += HandleDragStarted;
        objectInteractionManager.OnDragPerformed += HandleDragPerformed;
        objectInteractionManager.OnDragEnded += HandleDragEnded;
    }

    public void Dispose()
    {
        objectInteractionManager.OnHoldCompleted -= HandleHoldCompleted;
        objectInteractionManager.OnHoldCanceled -= HandleHoldCanceled;

        objectInteractionManager.OnDragStarted -= HandleDragStarted;
        objectInteractionManager.OnDragPerformed -= HandleDragPerformed;
        objectInteractionManager.OnDragEnded -= HandleDragEnded;
    }

    private IToyPart ResolveToyPart(IInteractable interact)
    {
        if (interact == null) return null;
        if (interact is MonoBehaviour mono)
        {
            if (mono == null) return null;
            return mono.GetComponentInParent<IToyPart>();
        }

        if (interact is IToyPart part) return part;
        return null;
    }

    private void HandleHoldCompleted(IInteractable interactable)
    {
        if (toolService.IsOnToolMode) return;

        IToyPart partToDetach = ResolveToyPart(interactable);

        if (partToDetach != null)
        {
            bool isDetached = assemblyService.TryDetach(partToDetach);
            if (isDetached) return;
        }

        if (interactable is IPressable pressable) { pressable.OnHoldCompleted(); }
    }

    private void HandleHoldCanceled(IInteractable interactable)
    {
        if (toolService.IsOnToolMode) return;

        if (interactable is IPressable pressable) { pressable.OnHoldCanceled(); }
    }

    private void HandleDragStarted(IInteractable interactable, Vector3 vector)
    {
        if (toolService.IsOnToolMode) return;

        IToyPart part = ResolveToyPart(interactable);
        if (part != null && !assemblyService.IsPartAssembled(part))
        {
            currentDraggedPart = part;
        }
        else
        {
            currentDraggedPart = null;
        }

        if (interactable is IDraggable drag) { drag.OnDragStarted(vector); }
    }

    private void HandleDragPerformed(IInteractable interactable, Vector3 vector)
    {
        if (toolService.IsOnToolMode) return;

        if (interactable is IDraggable drag) { drag.OnDragPerformed(vector); }
        if (currentDraggedPart != null)
        {
            assemblyService.TryCheckSlot(currentDraggedPart, vector);
        }
    }

    private void HandleDragEnded(IInteractable interactable, Vector3 vector)
    {
        if (toolService.IsOnToolMode) return;

        IToyPart toyPart = currentDraggedPart;
        currentDraggedPart = null;

        bool isSuccessfullyAssembled = false;

        if (toyPart != null && assemblyService.isToySlotAvailable)
        {
            if (assemblyService.TryAssemble(toyPart))
            {
                isSuccessfullyAssembled = true;
            }
        }

        if (isSuccessfullyAssembled) return;
        if (interactable is IDraggable drag) { drag.OnDragEnded(vector); }
    }
}