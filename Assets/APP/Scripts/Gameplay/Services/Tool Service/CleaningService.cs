using System;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using System.Collections.Generic;
using Modules;

public class CleaningService : IInitializable, IDisposable
{
    private readonly HashSet<ICleanable> cleanSurfaces = new();
    private InputSystemService inputSystemService;

    public bool isCleaning;
    private float overallProgress;

    public event Action<float> OnSurfaceCleaningUpdate;

    [Inject]
    public void Construct(InputSystemService input)
    {
        inputSystemService = input;
    }

    public void Initialize()
    {
        CleaningSurface.OnCreated += RegisterSurface;
        CleaningSurface.OnForceClean += HandleOnForceClean;

        // InteractionEvents.OnTabPerformed += HandleKeycodeTabPerformed;
        // InteractionEvents.OnTabCanceled += HandleKeycodeTabCanceled;

    }

    public void Dispose()
    {
        CleaningSurface.OnCreated -= RegisterSurface;
        CleaningSurface.OnForceClean -= HandleOnForceClean;

        // InteractionEvents.OnTabPerformed -= HandleKeycodeTabPerformed;
        // InteractionEvents.OnTabCanceled -= HandleKeycodeTabCanceled;
    }

    private void HandleOnForceClean(ICleanable surface)
    {
        OnSurfaceCleaningUpdate?.Invoke(CalculateSurfaceProgress());
    }

    private void HandleOveralProgressUpdated(float obj)
    {
        overallProgress = obj;
    }

    private void RegisterSurface(ICleanable surface)
    {
        // Debug.Log("Registering clean surface: " + surface);
        cleanSurfaces.Add(surface);
    }

    private void HandleKeycodeTabPerformed()
    {
        // AppLogger.Log("Tab kepencet di celaning service " + overallProgress + " < " + GameplayUIManager.clueEnableTreshold);
        // if (overallProgress < gameplayManager.clueTreshold) return;
        // AppLogger.Log("[HARUSNYA] Tab nyala cleaning");
        // foreach (ICleanable surface in cleanSurfaces)
        // {
        //     surface.ShowClue(true);
        // }
    }

    private void HandleKeycodeTabCanceled()
    {
        // if (overallProgress < gameplayManager.clueTreshold) return;
        // foreach (ICleanSurface surface in cleanSurfaces)
        // {
        //     surface.ShowClue(false);
        // }
    }

    public void CleanSurface(ICleanable surface, Texture2D brush, Vector3 hitPoint, Vector3 hitNormal, Vector3 direction, float scale, float strength, Color color, float brushDepth)
    {
        if (!cleanSurfaces.Contains(surface)) return;
        surface?.CleanSurface(hitPoint, brush, hitNormal, direction, scale, strength);
        OnSurfaceCleaningUpdate?.Invoke(CalculateSurfaceProgress());
    }

    public void ForceCleanAll()
    {
        foreach (ICleanable surface in cleanSurfaces)
        {
            if (surface != null) surface.ForceClean();
        }

        OnSurfaceCleaningUpdate?.Invoke(1f);
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