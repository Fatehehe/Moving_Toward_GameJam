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
        objectDetectionService.OnInteractDetected += HandleObjectDetected;

        inputSystemService.OnLeftPressStarted += HandlePressStart;
        inputSystemService.OnLeftPressEnded += HandlePressEnd;
        inputSystemService.OnMouseMoved += HandleMouseMove;
    }

    public void Dispose()
    {
        objectDetectionService.OnInteractDetected -= HandleObjectDetected;

        inputSystemService.OnLeftPressStarted -= HandlePressStart;
        inputSystemService.OnLeftPressEnded -= HandlePressEnd;
        inputSystemService.OnMouseMoved -= HandleMouseMove;
    }

    public void Tick()
    {
        if (currentToolObject == null) return;
        if (isCleaning)
        {
            if (movementTimer > 0f)
            {
                movementTimer -= Time.deltaTime;
                if (movementTimer <= 0f)
                {
                    currentToolObject.PlaySfx(false);
                    currentToolObject.PlayVfx(false);
                }
            }
        }
    }

    private void HandleObjectDetected(IInteractable obj)
    {
        currentInteract = obj;
    }

    private void HandlePressStart(Vector2 vec)
    {
        // 1. Cek apakah yang diklik adalah sebuah Tool (Untuk masuk/keluar Tool Mode)
        if (currentInteract is IToolObject clickedTool)
        {
            // Jika tool yang diklik SAMA dengan yang sedang dipakai -> Keluar mode tool
            if (currentToolObject == clickedTool)
            {
                ReturnCurrentTool();
            }
            // Jika beda atau belum pegang tool -> Masuk mode tool (Equip)
            else
            {
                EquipTool(clickedTool);
            }
            return; // Selesai evaluasi klik pada tool
        }

        // 2. Jika SEDANG di dalam Tool Mode, dan menekan di layar (mulai membersihkan)
        if (IsOnToolMode)
        {
            // Gunakan posisi mouse saat ini untuk mendeteksi apakah mengenai kotoran
            mousePos = inputSystemService.GetMousePosition();
            Debug.Log("Cleaning before");
            if (surfaceDetectionService.DetectSurface(mousePos))
            {
                Debug.Log("Cleaning after");
                isCleaning = true;
                movementTimer = MOVEMENT_TIMEOUT;
                ProcessCleaning();
            }
        }
    }

    private void HandleMouseMove(Vector2 newMousePos)
    {
        float delta = Vector2.Distance(mousePos, newMousePos);
        mousePos = newMousePos;

        // Jika sedang menahan klik (isCleaning) dan mouse bergerak -> Proses pembersihan
        if (isCleaning && delta > 1.0f)
        {
            movementTimer = MOVEMENT_TIMEOUT;
            ProcessCleaning();
        }
    }

    private void HandlePressEnd(Vector2 vec)
    {
        if (!IsOnToolMode) return;

        // Berhenti membersihkan saat klik dilepas
        isCleaning = false;
        if (currentToolObject != null)
        {
            currentToolObject.PlaySfx(false);
            currentToolObject.PlayVfx(false);
        }
    }

    private void EquipTool(IToolObject tool)
    {
        // Kembalikan tool sebelumnya jika ada
        ReturnCurrentTool();

        currentToolObject = tool;
        currentToolObject.Use();

        // TODO: Ubah kursor menjadi gambar tool (Texture2D Brush-mu)
        // Cursor.SetCursor(((IToolBrush)tool).GetBrush, Vector2.zero, CursorMode.Auto);

        Debug.Log("Masuk Mode Tool: " + tool.GetType().Name);
    }

    private void ReturnCurrentTool()
    {
        if (currentToolObject == null) return;

        currentToolObject.Return();

        // Matikan efek jika masih menyala
        currentToolObject.PlaySfx(false);
        currentToolObject.PlayVfx(false);

        currentToolObject = null;
        isCleaning = false;

        // TODO: Kembalikan kursor ke default
        // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        Debug.Log("Keluar Mode Tool.");
    }

    private void ProcessCleaning()
    {
        // 1. Raycast ke arah kursor mouse
        if (!surfaceDetectionService.DetectSurface(mousePos)) return;

        // 2. Jika yang digunakan adalah Brush
        if (currentToolObject is IToolBrush brushTool)
        {
            CleanSurface(brushTool);
        }
    }

    private void CleanSurface(IToolBrush brushTool)
    {
        var surface = surfaceDetectionService.CleanableSurface;
        if (surface == null) return;

        // Nyalakan efek suara & partikel
        currentToolObject.PlaySfx(true);
        currentToolObject.PlayVfx(true);

        // Lakukan pembersihan tekstur
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
}