using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PartService : IInitializable, IDisposable
{
    private readonly HashSet<ToyStateMachine> registry = new();
    private readonly GameplayUIManager gameplayUIManager;
    public event Action<int, int, float> OnProgressUpdate;

    [Inject]
    public PartService(GameplayUIManager gameplayUIManager)
    {
        this.gameplayUIManager = gameplayUIManager;
    }

    public void Initialize() => ToyStateMachine.OnCreated += Register;
    public void Dispose() => ToyStateMachine.OnCreated -= Register;

    public void Register(ToyStateMachine sm)
    {
        registry.Add(sm);
    }

    private bool isAssembleFinished = false;

    public void ProgressUpdate(int assembledCount)
    {
        int totalParts = registry.Count;
        if (totalParts == 0) return;

        float progressPercentage = (float)assembledCount / totalParts;
        // Debug.Log($"Assemble Progress: {assembledCount}/{totalParts} ({(progressPercentage * 100):0.##}%)");

        gameplayUIManager.GameplayUIController.UpdateAssembleProgress(progressPercentage);
        OnProgressUpdate?.Invoke(assembledCount, totalParts, progressPercentage);

        if (progressPercentage >= 1f && !isAssembleFinished)
        {
            isAssembleFinished = true;
            GameEvents.RaiseAssemblingFinished();
        }
    }
}