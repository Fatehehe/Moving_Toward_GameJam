using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ToyManager : IInitializable, IDisposable
{
    private readonly ObjectInteractionManager objectInteractionManager;
    private readonly AssembleService assemblyService;
    private readonly ToolService toolService;
    private readonly GameplayUIManager gameplayUIManager;
    private readonly GameConfigData config;

    private IToyPart currentDraggedPart;

    private bool isHoldingUI = false;
    private IInteractable currentHoldInteract;
    private IToyPart currentHoldPart;

    [Inject]
    public ToyManager(ObjectInteractionManager objectInteractionManager, AssembleService assemblyService,
    ToolService toolService, GameplayUIManager gameplayUIManager, GameConfigData config)
    {
        this.objectInteractionManager = objectInteractionManager;
        this.assemblyService = assemblyService;
        this.toolService = toolService;
        this.gameplayUIManager = gameplayUIManager;
        this.config = config;
    }

    public void Initialize()
    {
        objectInteractionManager.OnHoldCompleted += HandleHoldCompleted;
        objectInteractionManager.OnHoldPerformed += HandleHoldPerformed;
        objectInteractionManager.OnHoldCanceled += HandleHoldCanceled;

        objectInteractionManager.OnDragStarted += HandleDragStarted;
        objectInteractionManager.OnDragPerformed += HandleDragPerformed;
        objectInteractionManager.OnDragEnded += HandleDragEnded;
    }

    public void Dispose()
    {
        objectInteractionManager.OnHoldCompleted -= HandleHoldCompleted;
        objectInteractionManager.OnHoldPerformed -= HandleHoldPerformed;
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

    private void HandleHoldPerformed(IInteractable interactable, float holdTime, Vector2 screenPos)
    {
        if (toolService.IsOnToolMode) return;
        if (currentHoldInteract != interactable)
        {
            currentHoldInteract = interactable;
            currentHoldPart = ResolveToyPart(interactable);
        }

        if (currentHoldPart == null) return;

        if (!isHoldingUI)
        {
            isHoldingUI = true;
            ShowHoldProgress(currentHoldPart, screenPos);
        }

        float normalized = Mathf.Clamp01(holdTime / config.holdDuration);
        UpdateHoldProgress(currentHoldPart, normalized);
    }

    private void HandleHoldCompleted(IInteractable interactable)
    {
        if (toolService.IsOnToolMode) return;

        IToyPart partToDetach = currentHoldPart ?? ResolveToyPart(interactable);

        currentHoldInteract = null;
        currentHoldPart = null;

        HideHoldProgress(partToDetach);
        isHoldingUI = false;

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

        HideHoldProgress(currentHoldPart);
        isHoldingUI = false;

        currentHoldInteract = null;
        currentHoldPart = null;

        if (interactable is IPressable pressable) { pressable.OnHoldCanceled(); }
    }

    private void ShowHoldProgress(IToyPart part, Vector2 screenPos)
    {
        if (part == null) return;
        if (!assemblyService.IsPartAssembled(part)) return;
        gameplayUIManager.GameplayUIController.ShowHoldProgress(screenPos);
    }

    private void UpdateHoldProgress(IToyPart part, float normalized)
    {
        if (part == null) return;
        if (!assemblyService.IsPartAssembled(part)) return;
        gameplayUIManager.GameplayUIController.UpdateHoldProgress(normalized);
    }

    private void HideHoldProgress(IToyPart part)
    {
        if (part == null) return;
        if (!assemblyService.IsPartAssembled(part)) return;
        gameplayUIManager.GameplayUIController.HideHoldProgress();
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

        if (toyPart != null)
        {
            string cautionMsg = assemblyService.GetCautionMessage(toyPart, vector);

            if (!string.IsNullOrEmpty(cautionMsg))
            {
                gameplayUIManager.GameplayUIController.ShowCautionText(cautionMsg);
            }
            else if (assemblyService.isToySlotAvailable)
            {
                if (assemblyService.TryAssemble(toyPart))
                {
                    isSuccessfullyAssembled = true;
                }
            }
        }

        if (isSuccessfullyAssembled) return;
        if (interactable is IDraggable drag) { drag.OnDragEnded(vector); }
    }
}