using UnityEngine;

public abstract class ToyBaseState : State
{
    protected ToyStateMachine stateMachine;
    public ToyBaseState(ToyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
}
