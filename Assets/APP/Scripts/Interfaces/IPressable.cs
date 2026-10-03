using UnityEngine;

public interface IPressable
{
    void OnPressStarted();
    void OnPressEnded();

    void OnHoldCompleted();
    void OnHoldCanceled();
}