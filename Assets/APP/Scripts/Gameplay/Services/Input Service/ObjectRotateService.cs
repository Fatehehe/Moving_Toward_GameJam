using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ObjectRotateService : IInitializable, IDisposable
{
    private readonly InputSystemService inputSystemService;
    private readonly GameConfigData config;

    private bool isRotating;
    private Vector2 lastMousePos;
    private Vector2 currentMousePos;
    public event Action<Vector2> OnRotatePerformed;

    public event Action OnRotateStarted;
    public event Action OnRotateEnded;

    [Inject]
    public ObjectRotateService(InputSystemService inputSystemService, GameConfigData config)
    {
        this.inputSystemService = inputSystemService;
        this.config = config;
    }

    public void Initialize()
    {
        inputSystemService.OnMouseMoved += HandleMouseMove;
        inputSystemService.OnRightPressStarted += HandleRightStarted;
        inputSystemService.OnRightPressEnded += HandleRightEnded;
    }

    public void Dispose()
    {
        inputSystemService.OnMouseMoved -= HandleMouseMove;
        inputSystemService.OnRightPressStarted -= HandleRightStarted;
        inputSystemService.OnRightPressEnded -= HandleRightEnded;
    }

    private void HandleRightStarted()
    {
        StartRotation(currentMousePos);
    }

    private void HandleRightEnded()
    {
        StopRotation();
    }

    private void HandleMouseMove(Vector2 currentPos)
    {
        currentMousePos = currentPos;

        if (!isRotating) return;

        Vector2 delta = currentPos - lastMousePos;
        lastMousePos = currentPos;

        Vector2 rotateDelta = delta * config.rotateSensitivity;
        OnRotatePerformed?.Invoke(rotateDelta);
    }

    private void StartRotation(Vector2 pos)
    {
        isRotating = true;
        lastMousePos = pos;

        OnRotateStarted?.Invoke();
        CursorController.instance?.SetOverrideCursor(CursorState.Rotate);
    }

    private void StopRotation()
    {
        if (isRotating)
        {
            isRotating = false;

            OnRotateEnded?.Invoke();
            CursorController.instance?.ClearOverrideCursor();
        }
    }
}