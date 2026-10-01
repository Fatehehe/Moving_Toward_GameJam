using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PartService : IInitializable, IDisposable
{
    private readonly HashSet<ToyStateMachine> registry = new();
    public event Action<int, int, float> OnProgressUpdate;

    public void Initialize() => ToyStateMachine.OnCreated += Register;
    public void Dispose() => ToyStateMachine.OnCreated -= Register;

    public void Register(ToyStateMachine sm)
    {
        registry.Add(sm);
    }

    public void ProgressUpdate(int assembledCount)
    {
        int totalParts = registry.Count;
        if (totalParts == 0) return;
        float progressPercentage = (float)assembledCount / totalParts;
        Debug.Log($"Progress: {assembledCount}/{totalParts} ({(progressPercentage * 100):0.##}%)");
        OnProgressUpdate?.Invoke(assembledCount, totalParts, progressPercentage);
    }
}