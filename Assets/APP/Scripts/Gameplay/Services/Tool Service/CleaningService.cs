using System;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using System.Collections.Generic;
using Modules;

public class CleaningService : IInitializable, IDisposable
{
    private readonly HashSet<ICleanable> cleanSurfaces = new();
    private GameplayUIManager gameplayUIManager;
    public bool isCleaning;
    private bool isCleaningFinished = false;

    public event Action<float> OnSurfaceCleaningUpdate;

    [Inject]
    public void Construct(GameplayUIManager uiManager)
    {
        gameplayUIManager = uiManager;
    }

    public void Initialize()
    {
        CleaningSurface.OnCreated += RegisterSurface;
        CleaningSurface.OnForceClean += HandleOnForceClean;
    }

    public void Dispose()
    {
        CleaningSurface.OnCreated -= RegisterSurface;
        CleaningSurface.OnForceClean -= HandleOnForceClean;
    }

    private void EvaluateCleaningProgress()
    {
        float progress = CalculateSurfaceProgress();
        gameplayUIManager.GameplayUIController.UpdateCleanProgress(progress);
        OnSurfaceCleaningUpdate?.Invoke(progress);

        if (progress >= 1f && !isCleaningFinished)
        {
            isCleaningFinished = true;
            GameEvents.RaiseCleaningFinished();
        }
    }

    private void HandleOnForceClean(ICleanable surface)
    {
        EvaluateCleaningProgress();
    }

    public void CleanSurface(ICleanable surface, Texture2D brush, Vector3 hitPoint, Vector3 hitNormal, Vector3 direction, float scale, float strength, Color color, float brushDepth)
    {
        if (!cleanSurfaces.Contains(surface)) return;
        surface?.CleanSurface(hitPoint, brush, hitNormal, direction, scale, strength);

        EvaluateCleaningProgress();
    }

    public void ForceCleanAll()
    {
        foreach (ICleanable surface in cleanSurfaces)
        {
            if (surface != null) surface.ForceClean();
        }
        EvaluateCleaningProgress();
    }

    private void RegisterSurface(ICleanable surface)
    {
        cleanSurfaces.Add(surface);
    }

    private float CalculateSurfaceProgress()
    {
        if (cleanSurfaces.Count == 0)
            return 1f;

        float total = 0;

        foreach (var surface in cleanSurfaces)
        {
            total += surface.GetCleaningProgress();
        }
        float averagePercentage = total / cleanSurfaces.Count;
        return averagePercentage / 100;
    }

    public bool IsDustRemoved(ICleanable surface)
    {
        return surface.IsDustRemoved();
    }
}