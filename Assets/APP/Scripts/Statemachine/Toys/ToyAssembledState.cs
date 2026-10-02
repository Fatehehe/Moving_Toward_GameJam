using DG.Tweening;
using UnityEngine;

public class ToyAssembledState : ToyBaseState, IToyPart, ICleanPart
{
    public Vector3 punchRotation = new Vector3(5, 5, 0);

    public ToyAssembledState(ToyStateMachine stateMachine) : base(stateMachine) { }


    public override void Enter()
    {
        stateMachine.transform.DOPunchRotation(punchRotation, .4f);

    }
    public override void Tick(float deltaTime) { }
    public override void Exit() { }

    //IToyPart
    public string PieceId => stateMachine.PieceId;
    public ParentSlot ParentSlot => stateMachine.ParentSlot;
    public Transform GetTransform() => stateMachine.GetTransform();
    public bool IsParentAvailable(string id) => stateMachine.IsParentAvailable(id);
    public void ReleaseSlotWith(string otherId) => stateMachine.ReleaseSlotWith(otherId);
    public bool IsSlotEmpty() => stateMachine.IsSlotEmpty();
    public void OnAssembled(Transform targetTransform) { }

    public void OnDetached()
    {
        stateMachine.SwitchState(new ToyReturnState(stateMachine));
    }

    // ICleanPart
    public bool IsCleanable() => true;
}
