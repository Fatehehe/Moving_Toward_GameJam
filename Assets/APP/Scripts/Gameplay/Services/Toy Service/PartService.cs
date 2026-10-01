using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;


public class PartService : IInitializable, IDisposable
{
    private readonly HashSet<ToyStateMachine> registry = new();
    public event Action<float> OnProgressUpdate;

    public void Initialize() => ToyStateMachine.OnCreated += Register;
    public void Dispose() => ToyStateMachine.OnCreated -= Register;

    public void Register(ToyStateMachine sm)
    {
        Debug.Log($"Register {sm}");
        registry.Add(sm);
    }
    public float GetAssemblyProgress()
    {
        if (registry.Count == 0) return 0f;
        if (registry.Count == 1) return 1f;

        int connectedPieces = 0;

        foreach (var piece in registry)
        {
            bool isAttached = false;

            if (piece.IsSlotEmpty())
            {
                isAttached = true;
                break;
            }

            if (isAttached)
            {
                connectedPieces++;
            }
        }

        float progress = (float)connectedPieces / registry.Count;

        return progress;
    }

    public void ProgressUpdate()
    {
        OnProgressUpdate?.Invoke(GetAssemblyProgress());
    }

}
