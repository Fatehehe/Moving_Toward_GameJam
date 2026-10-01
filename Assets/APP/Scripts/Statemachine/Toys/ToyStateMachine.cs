using System;
using UnityEngine;

public class ToyStateMachine : StateMachine, IInteractable, IDraggable, IToyPart
{
    [SerializeField] private string pieceId;
    [SerializeField] private ParentSlot parentSlot;

    public string PieceId => pieceId;
    public ParentSlot ParentSlot => parentSlot;

    public static event Action<ToyStateMachine> OnCreated;

    private void Start()
    {
        InitialPosition = transform.position;
        InitialRotation = transform.rotation;
        SwitchState(new ToyIdleState(this));
        OnCreated?.Invoke(this);
    }

    public void OnDragStarted(Vector3 worldPos) => (currentState as IDraggable).OnDragStarted(worldPos);
    public void OnDragEnded(Vector3 worldPos) => (currentState as IDraggable).OnDragEnded(worldPos);
    public void OnDragPerformed(Vector3 worldPos) => (currentState as IDraggable).OnDragPerformed(worldPos);

    public void OnInteractDetected() => (currentState as IInteractable)?.OnInteractDetected();
    public void OnInteractEnded() => (currentState as IInteractable)?.OnInteractEnded();

    public void OnAssembled(Transform targetPosition) => (currentState as IToyPart)?.OnAssembled(targetPosition);
    public void OnDetached() => (currentState as IToyPart)?.OnDetached();

    public Transform GetTransform() => transform;
    public bool IsParentAvailable(string id) => (parentSlot.parentId == id) && (!parentSlot.isOccupied);
    public void ReleaseSlotWith(string otherId) => parentSlot.isOccupied = false;
    public bool IsSlotEmpty() => parentSlot.isOccupied;

}
