using DG.Tweening;
using UnityEngine;

public class ToyReturnState : ToyBaseState
{
    private Sequence returnSequence;

    public ToyReturnState(ToyStateMachine stateMachine) : base(stateMachine) { }


    public override void Enter()
    {
        returnSequence = DOTween.Sequence();

        returnSequence.Join(stateMachine.transform.DOMove(stateMachine.InitialPosition, .5f).SetEase(Ease.OutBack));
        returnSequence.Join(stateMachine.transform.DORotateQuaternion(stateMachine.InitialRotation, .5f).SetEase(Ease.OutBack));

        returnSequence.OnComplete(() =>
        {
            stateMachine.SwitchState(new ToyIdleState(stateMachine));
        });
    }

    public override void Tick(float deltaTime) { }

    public override void Exit()
    {
        returnSequence?.Kill();
    }
}
