using UnityEngine;

public class ToyStateMachine : StateMachine, IInteractable, IDraggable
{
    public void OnDragStarted(Vector3 worldPos) => (currentState as IDraggable)?.OnDragStarted(worldPos);
    public void OnDragEnded(Vector3 worldPos) => (currentState as IDraggable)?.OnDragEnded(worldPos);
    public void OnDragPerformed(Vector3 worldPos) => (currentState as IDraggable)?.OnDragPerformed(worldPos);

    public void OnInteractDetected() => (currentState as IInteractable)?.OnInteractDetected();
    public void OnInteractEnded() => (currentState as IInteractable)?.OnInteractEnded();
}
