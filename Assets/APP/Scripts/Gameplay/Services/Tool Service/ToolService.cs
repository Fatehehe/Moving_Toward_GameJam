using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ToolService : IInitializable, IDisposable, ITickable
{
    private readonly ObjectDetectionService objectDetectionService;
    private readonly SurfaceDetectionService surfaceDetectionService;
    private readonly CleaningService cleaningService;
    private readonly InputSystemService inputSystemService;

    public bool isCleaning { get; private set; }
    public bool IsOnToolMode => currentToolObject != null;

    private IToolObject currentToolObject;
    private IInteractable currentInteract;
    private Vector2 mousePos;

    private float movementTimer = 0f;
    private const float MOVEMENT_TIMEOUT = 0.15f;

    private bool isGameEnded = false;

    // Tambahan state untuk SFX (VFX sudah dihapus)
    private bool isSfxPlaying = false;

    [Inject]
    public ToolService(
        ObjectDetectionService objectDetectionService,
        SurfaceDetectionService surfaceDetectionService,
        CleaningService cleaningService,
        InputSystemService inputSystemService)
    {
        this.objectDetectionService = objectDetectionService;
        this.surfaceDetectionService = surfaceDetectionService;
        this.cleaningService = cleaningService;
        this.inputSystemService = inputSystemService;
    }

    public void Initialize()
    {
        GameEvents.OnAssemblingFinished += HandleAssemblingFinished;

        objectDetectionService.OnInteractDetected += HandleObjectDetected;

        inputSystemService.OnLeftPressStarted += HandlePressStart;
        inputSystemService.OnLeftPressEnded += HandlePressEnd;
        inputSystemService.OnMouseMoved += HandleMouseMove;
    }

    public void Dispose()
    {
        GameEvents.OnAssemblingFinished -= HandleAssemblingFinished;

        objectDetectionService.OnInteractDetected -= HandleObjectDetected;

        inputSystemService.OnLeftPressStarted -= HandlePressStart;
        inputSystemService.OnLeftPressEnded -= HandlePressEnd;
        inputSystemService.OnMouseMoved -= HandleMouseMove;
    }

    private void HandleAssemblingFinished()
    {
        isGameEnded = true;
        ReturnCurrentTool();
    }

    public void Tick()
    {
        if (isGameEnded) return;
        if (currentToolObject == null) return;

        if (isCleaning)
        {
            if (movementTimer > 0f)
            {
                movementTimer -= Time.deltaTime;
                if (movementTimer <= 0f)
                {
                    PlayToolSfx(false);
                }
            }
        }
    }

    private void HandleObjectDetected(IInteractable obj)
    {
        if (isGameEnded) return;
        currentInteract = obj;
    }

    private void HandlePressStart(Vector2 vec)
    {
        if (isGameEnded) return;

        if (currentInteract is IToolObject clickedTool)
        {
            if (currentToolObject == clickedTool)
            {
                ReturnCurrentTool();
            }
            else
            {
                EquipTool(clickedTool);
            }
            return;
        }

        if (IsOnToolMode)
        {
            mousePos = inputSystemService.GetMousePosition();
            if (surfaceDetectionService.DetectSurface(mousePos))
            {
                isCleaning = true;
                movementTimer = MOVEMENT_TIMEOUT;
                ProcessCleaning();
            }
        }
    }

    private void HandleMouseMove(Vector2 newMousePos)
    {
        if (isGameEnded) return;

        float delta = Vector2.Distance(mousePos, newMousePos);
        mousePos = newMousePos;

        if (isCleaning && delta > 1.0f)
        {
            movementTimer = MOVEMENT_TIMEOUT;
            ProcessCleaning();
        }
    }

    private void HandlePressEnd(Vector2 vec)
    {
        if (isGameEnded) return;
        if (!IsOnToolMode) return;

        isCleaning = false;

        PlayToolSfx(false);
    }

    private void EquipTool(IToolObject tool)
    {
        ReturnCurrentTool();

        currentToolObject = tool;
        currentToolObject.Use();

        CursorController.instance?.LockCursorState(CursorState.Crosshair);
    }

    private void ReturnCurrentTool()
    {
        if (currentToolObject == null) return;

        currentToolObject.Return();

        // Panggil SEBELUM currentToolObject diset null
        PlayToolSfx(false);

        currentToolObject = null;
        isCleaning = false;

        CursorController.instance?.UnlockCursorState();
        CursorController.instance?.SetCursorState(CursorState.DefaultRounded);
    }

    private void ProcessCleaning()
    {
        if (!surfaceDetectionService.DetectSurface(mousePos)) return;
        if (currentToolObject is IToolBrush brushTool)
        {
            CleanSurface(brushTool);
        }
    }

    private void CleanSurface(IToolBrush brushTool)
    {
        var surface = surfaceDetectionService.CleanableSurface;
        if (surface == null) return;

        PlayToolSfx(true);

        cleaningService.CleanSurface(
            surface,
            brushTool.GetBrush,
            surfaceDetectionService.RaycastPos,
            surfaceDetectionService.RaycastNormal,
            brushTool.BrushTransform.up,
            brushTool.BrushScale,
            brushTool.BrushStrength,
            brushTool.BrushColor,
            brushTool.BrushDepth
        );
    }

    // --- Implementasi Fungsi PlayToolSfx ---

    private void PlayToolSfx(bool play)
    {
        if (currentToolObject != null)
        {
            if (play && !isSfxPlaying)
            {
                currentToolObject.PlaySfx(true);
                isSfxPlaying = true;
            }
            else if (!play && isSfxPlaying)
            {
                currentToolObject.PlaySfx(false);
                isSfxPlaying = false;
            }
        }
    }
}