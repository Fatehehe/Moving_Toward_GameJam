using DG.Tweening;
using UnityEngine;

public class ToyIdleState : ToyBaseState, IDraggable
{
    public ToyIdleState(ToyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.ResetTransform();
    }

    public override void Exit()
    {

    }
    public override void Tick(float deltaTime)
    {

    }

    public void OnDragStarted(Vector3 worldPos)
    {
        stateMachine.transform.DOKill();
        stateMachine.transform.DOMove(worldPos, .5f).SetEase(Ease.OutCubic);
    }

    public void OnDragPerformed(Vector3 worldPos)
    {
        stateMachine.transform.DOKill();
        stateMachine.transform.DOMove(worldPos, .5f).SetEase(Ease.OutCubic);
    }

    public void OnDragEnded(Vector3 worldPos)
    {
        stateMachine.SwitchState(new ToyReturnState(stateMachine));
    }

}
