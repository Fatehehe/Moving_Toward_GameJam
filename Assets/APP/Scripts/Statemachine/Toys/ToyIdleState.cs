using DG.Tweening;
using UnityEngine;

public class ToyIdleState : ToyBaseState, IDraggable, IToyPart
{
    public ToyIdleState(ToyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.ResetTransform();
    }
    public override void Exit() { }
    public override void Tick(float deltaTime) { }

    //IDraggable
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

    //IToyPart
    public string PieceId => stateMachine.PieceId;
    public ParentSlot ParentSlot => stateMachine.ParentSlot;
    public Transform GetTransform() => stateMachine.GetTransform();
    public bool IsParentAvailable(string id) => stateMachine.IsParentAvailable(id);
    public void ReleaseSlotWith(string otherId) => stateMachine.ReleaseSlotWith(otherId);
    public bool IsSlotEmpty() => stateMachine.IsSlotEmpty();

    public void OnAssembled(Transform targetTransform)
    {
        stateMachine.SwitchState(new ToyAssembledState(stateMachine));
    }

    public void OnDetached() { }
}
