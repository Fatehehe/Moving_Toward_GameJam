using System;
using UnityEngine;

public static class GameEvents
{
    // ... event yang sudah ada sebelumnya
    public static event Action OnGameStarted;
    public static event Action OnGameEnded;
    public static event Action OnCleaningFinished;
    public static event Action OnAssemblingFinished;
    public static event Action OnGameFinished;
    public static event Action<int> OnTutorialCompleted;

    // --- TAMBAHAN BARU ---
    public static event Action OnFirstInspect;
    public static event Action OnFirstSurfaceCleaned;

    public static void RaiseFirstInspect() => OnFirstInspect?.Invoke();
    public static void RaiseFirstSurfaceCleaned() => OnFirstSurfaceCleaned?.Invoke();
    public static void RaiseTutorialCompleted(int index) => OnTutorialCompleted?.Invoke(index);
    // ---------------------

    public static void RaiseGameStarted() => OnGameStarted?.Invoke();
    public static void RaiseGameEnded() => OnGameEnded?.Invoke();
    public static void RaiseCleaningFinished() => OnCleaningFinished?.Invoke();
    public static void RaiseAssemblingFinished() => OnAssemblingFinished?.Invoke();
    public static void RaiseGameFinished()
    {
        OnGameFinished?.Invoke();
    }
}