using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class AssembleService : IInitializable, IDisposable
{
    private readonly Inspection inspection;
    private readonly PartService partService;
    private readonly Camera camera;
    public Transform GetInspectPoint() => inspection.transform;

    private readonly GameConfigData config;
    private readonly List<IToyPart> currentAssembleList = new();

    public bool isToySlotAvailable = false;
    private bool isAssemblePhase = false;
    private bool hasTriggeredFirstInspect = false;

    [Inject]
    public AssembleService(Inspection inspection, PartService partService, GameConfigData config, Camera cam)
    {
        this.inspection = inspection;
        this.partService = partService;
        this.config = config;
        this.camera = cam;
    }

    public void Initialize()
    {
        GameEvents.OnCleaningFinished += EnableAssemblePhase;
    }

    public void Dispose()
    {
        GameEvents.OnCleaningFinished -= EnableAssemblePhase;
    }

    private void EnableAssemblePhase()
    {
        isAssemblePhase = true;
    }

    public bool TryCheckSlot(IToyPart toyPart, Vector3 worldPos)
    {
        isToySlotAvailable = false;

        if (IsInspectEmpty())
        {
            float distance = GetFlattenedDistance(worldPos, inspection.transform.position);
            isToySlotAvailable = distance < 2f;
            return isToySlotAvailable;
        }

        if (!isAssemblePhase) return false;

        IToyPart outermostPart = currentAssembleList[currentAssembleList.Count - 1];

        if (outermostPart.IsParentAvailable(toyPart.PieceId))
        {
            float distance = GetFlattenedDistance(worldPos, outermostPart.GetTransform().position);
            isToySlotAvailable = distance <= 2f;
            return isToySlotAvailable;
        }

        return false;
    }

    public bool IsPartAssembled(IToyPart part)
    {
        return currentAssembleList.Contains(part);
    }

    public bool TryAssemble(IToyPart toyPart)
    {
        if (IsPartAssembled(toyPart)) return false;

        if (IsInspectEmpty())
        {
            currentAssembleList.Add(toyPart);
            toyPart.GetTransform().SetParent(inspection.transform);
            toyPart.OnAssembled(inspection.transform);
            inspection.SetInspectionUsage(true);
            partService.ProgressUpdate(currentAssembleList.Count);

            if (!hasTriggeredFirstInspect)
            {
                Debug.Log("[AssembleService] Pemain berhasil merakit part pertama!");
                hasTriggeredFirstInspect = true;
                GameEvents.RaiseFirstInspect();
            }
            return true;
        }
        else
        {
            if (!isAssemblePhase) return false;

            IToyPart outermostPart = currentAssembleList[currentAssembleList.Count - 1];
            if (outermostPart.IsParentAvailable(toyPart.PieceId))
            {
                currentAssembleList.Add(toyPart);
                toyPart.GetTransform().SetParent(inspection.transform);
                toyPart.OnAssembled(outermostPart.GetTransform());
                partService.ProgressUpdate(currentAssembleList.Count);
                return true;
            }
        }

        return false;
    }

    public bool TryDetach(IToyPart toyPart)
    {
        if (IsInspectEmpty()) return false;

        int lastIndex = currentAssembleList.Count - 1;
        IToyPart outermostPart = currentAssembleList[lastIndex];

        if (outermostPart != toyPart)
        {
            return false;
        }

        currentAssembleList.RemoveAt(lastIndex);
        toyPart.GetTransform().SetParent(null);
        toyPart.OnDetached();

        if (IsInspectEmpty())
        {
            inspection.ResetTransform();
            inspection.SetInspectionUsage(false);
        }

        partService.ProgressUpdate(currentAssembleList.Count);

        return true;
    }

    // (Sisa kode AssembleService tetap sama) ...

    private float GetFlattenedDistance(Vector3 posA, Vector3 posB)
    {
        if (camera == null) return Vector3.Distance(posA, posB);

        Vector3 localA = camera.transform.InverseTransformPoint(posA);
        Vector3 localB = camera.transform.InverseTransformPoint(posB);
        localA.z = localB.z;

        return Vector3.Distance(localA, localB);
    }

    public bool IsInspectEmpty()
    {
        return currentAssembleList.Count == 0;
    }

    // --- FUNGSI BARU UNTUK MENDETEKSI PELANGGARAN ---
    public string GetCautionMessage(IToyPart toyPart, Vector3 worldPos)
    {
        // Jika meja kosong, pemain boleh menaruh part pertama (meski belum selesai cleaning)
        if (IsInspectEmpty()) return string.Empty;

        IToyPart outermostPart = currentAssembleList[currentAssembleList.Count - 1];
        float distance = GetFlattenedDistance(worldPos, outermostPart.GetTransform().position);

        // Hanya munculkan pesan kalau pemain mencoba "merakit" (drop di dekat target)
        if (distance <= 2f)
        {
            if (!isAssemblePhase) return "Bersihkan semua kotoran terlebih dahulu!";
            if (!outermostPart.IsParentAvailable(toyPart.PieceId)) return "Urutan pemasangan salah!";
        }

        return string.Empty; // Kosong berarti aman / tidak ada pelanggaran
    }
}