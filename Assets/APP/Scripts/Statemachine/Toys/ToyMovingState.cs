using UnityEngine;
using DG.Tweening;

public class ToyMovingState : ToyBaseState
{
    private readonly Transform targetTransform;
    private Sequence moveSequence;

    public ToyMovingState(ToyStateMachine stateMachine, Transform targetPos) : base(stateMachine)
    {
        targetTransform = targetPos;
    }

    public override void Enter()
    {
        moveSequence = DOTween.Sequence();

        moveSequence.Join(stateMachine.transform.DOMove(targetTransform.position, .5f).SetEase(Ease.OutBack));
        moveSequence.Join(stateMachine.transform.DORotateQuaternion(targetTransform.rotation, .5f).SetEase(Ease.OutBack));

        moveSequence.OnComplete(() =>
        {
            stateMachine.SwitchState(new ToyAssembledState(stateMachine));
        });
    }

    public override void Tick(float deltaTime) { }
    public override void Exit() { moveSequence?.Kill(); }
}