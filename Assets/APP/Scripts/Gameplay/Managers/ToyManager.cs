using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ToyManager : IInitializable, IDisposable
{
    private readonly ObjectInteractionManager objectInteractionManager;

    [Inject]
    public ToyManager(ObjectInteractionManager objectInteractionManager)
    {
        this.objectInteractionManager = objectInteractionManager;
    }

    public void Initialize()
    {
        objectInteractionManager.OnDragStarted += HandleDragStarted;
        objectInteractionManager.OnDragPerformed += HandleDragPerformed;
        objectInteractionManager.OnDragEnded += HandleDragEnded;
    }
    public void Dispose()
    {
        objectInteractionManager.OnDragStarted -= HandleDragStarted;
        objectInteractionManager.OnDragPerformed -= HandleDragPerformed;
        objectInteractionManager.OnDragEnded -= HandleDragEnded;
    }

    private void HandleDragStarted(IInteractable interactable, Vector3 vector)
    {
        if (interactable is IDraggable drag) { drag.OnDragStarted(vector); }
    }

    private void HandleDragPerformed(IInteractable interactable, Vector3 vector)
    {
        if (interactable is IDraggable drag) { drag.OnDragPerformed(vector); }
    }

    private void HandleDragEnded(IInteractable interactable, Vector3 vector)
    {
        if (interactable is IDraggable drag) { drag.OnDragEnded(vector); }
    }
}
