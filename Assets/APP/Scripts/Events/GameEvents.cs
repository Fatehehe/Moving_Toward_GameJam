using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action OnGameStarted;
    public static event Action OnGameEnded;
    public static event Action OnCleaningFinished;
    public static event Action OnAssemblingFinished;

    public static void RaiseGameStarted()
    {
        OnGameStarted?.Invoke();
    }

    public static void RaiseGameEnded()
    {
        OnGameEnded?.Invoke();
    }

    public static void RaiseCleaningFinished()
    {
        OnCleaningFinished?.Invoke();
    }

    public static void RaiseAssemblingFinished()
    {
        OnAssemblingFinished?.Invoke();
    }
}
