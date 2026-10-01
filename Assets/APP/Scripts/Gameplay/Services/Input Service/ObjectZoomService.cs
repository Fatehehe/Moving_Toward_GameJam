using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ObjectZoomService : IInitializable, IDisposable
{
    private readonly InputSystemService inputSystemService;
    private readonly GameConfigData config;
    public event Action<float> OnZoomPerformed;

    [Inject]
    public ObjectZoomService(InputSystemService inputSystemService, GameConfigData config)
    {
        this.inputSystemService = inputSystemService;
        this.config = config;
    }

    public void Initialize()
    {
        inputSystemService.OnScrollPerformed += HandleScrollPerformed;
    }

    public void Dispose()
    {
        inputSystemService.OnScrollPerformed -= HandleScrollPerformed;
    }

    private void HandleScrollPerformed(float value)
    {
        float zoomDelta = value * config.scrollSensitivity;
        OnZoomPerformed.Invoke(zoomDelta);
    }
}