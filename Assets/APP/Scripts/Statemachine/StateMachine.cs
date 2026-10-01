using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public Vector3 InitialPosition;
    public Quaternion InitialRotation;
    protected State currentState;

    public void SwitchState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    private void Update()
    {
        currentState?.Tick(Time.deltaTime);
    }

    public void ResetTransform()
    {
        transform.SetPositionAndRotation(InitialPosition, InitialRotation);
    }
}