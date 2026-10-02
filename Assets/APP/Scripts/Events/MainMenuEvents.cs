using System;
using UnityEngine;

public static class MainMenuEvents
{
    public static event Action OnNewGame;
    public static event Action OnOpenCredits;
    public static event Action OnCloseCredits;
    public static void TriggerNewGame() => OnNewGame?.Invoke();
    public static void TriggerOpenCredits() => OnOpenCredits?.Invoke();
    public static void TriggerCloseCredits() => OnCloseCredits?.Invoke();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        OnNewGame = null;
        OnOpenCredits = null;
        OnCloseCredits = null;
    }
}
