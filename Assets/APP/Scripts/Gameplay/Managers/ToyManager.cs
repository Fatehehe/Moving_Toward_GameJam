using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ToyManager : IInitializable, IDisposable
{
    private readonly ObjectInteractionManager objectInteractionManager;
    private readonly AssemblyService assemblyService;
    private readonly Inspection inspection;

    private IToyPart currentDraggedPart;

    [Inject]
    public ToyManager(ObjectInteractionManager objectInteractionManager, AssemblyService assemblyService, Inspection inspection)
    {
        this.objectInteractionManager = objectInteractionManager;
        this.assemblyService = assemblyService;
        this.inspection = inspection;
    }

    public void Initialize()
    {
        objectInteractionManager.OnHoldCompleted += HandleHoldCompleted;
        objectInteractionManager.OnHoldCanceled += HandleHoldCanceled;

        objectInteractionManager.OnDragStarted += HandleDragStarted;
        objectInteractionManager.OnDragPerformed += HandleDragPerformed;
        objectInteractionManager.OnDragEnded += HandleDragEnded;
    }

    public void Dispose()
    {
        objectInteractionManager.OnHoldCompleted -= HandleHoldCompleted;
        objectInteractionManager.OnHoldCanceled -= HandleHoldCanceled;

        objectInteractionManager.OnDragStarted -= HandleDragStarted;
        objectInteractionManager.OnDragPerformed -= HandleDragPerformed;
        objectInteractionManager.OnDragEnded -= HandleDragEnded;
    }

    // Mengganti nama ResolveArtefactPart menjadi ResolveToyPart agar lebih sesuai
    private IToyPart ResolveToyPart(IInteractable interact)
    {
        if (interact == null) return null;
        if (interact is MonoBehaviour mono)
        {
            if (mono == null) return null;
            return mono.GetComponentInParent<IToyPart>();
        }

        if (interact is IToyPart part) return part;
        return null;
    }

    private void HandleHoldCompleted(IInteractable interactable)
    {
        IToyPart partToDetach = ResolveToyPart(interactable);

        if (partToDetach != null)
        {
            // Coba detach jika itu adalah part terluar di meja inspect
            bool isDetached = assemblyService.TryDetach(partToDetach);

            // Jika berhasil detach, abaikan logika hold biasa (karena sudah lepas)
            if (isDetached) return;
        }

        if (interactable is IPressable pressable) { pressable.OnHoldCompleted(); }
    }

    private void HandleHoldCanceled(IInteractable interactable)
    {
        if (interactable is IPressable pressable) { pressable.OnHoldCanceled(); }
    }

    private void HandleDragStarted(IInteractable interactable, Vector3 vector)
    {
        currentDraggedPart = ResolveToyPart(interactable);

        if (interactable is IDraggable drag) { drag.OnDragStarted(vector); }
    }

    private void HandleDragPerformed(IInteractable interactable, Vector3 vector)
    {
        if (interactable is IDraggable drag) { drag.OnDragPerformed(vector); }

        // Setiap pergerakan drag, cek jarak Matrioska ke slot/meja inspect
        if (currentDraggedPart != null)
        {
            assemblyService.TryCheckSlot(currentDraggedPart, vector);
        }
    }

    private void HandleDragEnded(IInteractable interactable, Vector3 vector)
    {
        IToyPart toyPart = currentDraggedPart;
        currentDraggedPart = null;

        // Jika slot tersedia dan matrioska valid untuk digabungkan
        if (toyPart != null && assemblyService.isToySlotAvailable)
        {
            if (assemblyService.TryAssemble(toyPart))
            {
                // Jika berhasil masuk/dirakit, RETURN agar DragEnded (kembali ke meja) tidak dipanggil
                return;
            }
        }

        // Jika gagal dirakit atau dilepas jauh dari meja inspect, kembalikan posisi / panggil DragEnded biasa
        if (interactable is IDraggable drag) { drag.OnDragEnded(vector); }
    }
}