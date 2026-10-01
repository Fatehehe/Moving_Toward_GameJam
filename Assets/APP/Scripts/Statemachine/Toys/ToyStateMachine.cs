using UnityEngine;

public class ToyStateMachine : StateMachine, IInteractable, IDraggable
{
    private void Start()
    {
        InitialPosition = transform.position;
        InitialRotation = transform.rotation;
        SwitchState(new ToyIdleState(this));
    }

    public void OnDragStarted(Vector3 worldPos) => (currentState as IDraggable).OnDragStarted(worldPos);
    public void OnDragEnded(Vector3 worldPos) => (currentState as IDraggable).OnDragEnded(worldPos);
    public void OnDragPerformed(Vector3 worldPos) => (currentState as IDraggable).OnDragPerformed(worldPos);

    public void OnInteractDetected() => (currentState as IInteractable)?.OnInteractDetected();
    public void OnInteractEnded() => (currentState as IInteractable)?.OnInteractEnded();
}
