using UnityEngine;

public interface IToyPart
{
    string PieceId { get; }
    ParentSlot ParentSlot { get; }
    Transform GetTransform();

    bool IsParentAvailable(string id);
    void ReleaseSlotWith(string otherId);

    bool IsSlotEmpty();

    void OnAssembled(Transform targetTransform);
    void OnDetached();
}