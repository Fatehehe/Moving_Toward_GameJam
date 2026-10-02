using System;
using System.Collections.Generic;
using DG.Tweening;
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

    [Inject]
    public AssembleService(Inspection inspection, PartService partService, GameConfigData config, Camera cam)
    {
        this.inspection = inspection;
        this.partService = partService;
        this.config = config;
        this.camera = cam;
    }

    public void Initialize() { }
    public void Dispose() { }

    public bool TryCheckSlot(IToyPart toyPart, Vector3 worldPos)
    {
        isToySlotAvailable = false;

        if (IsInspectEmpty())
        {
            float distance = GetFlattenedDistance(worldPos, inspection.transform.position);
            isToySlotAvailable = distance < 2f;
            return isToySlotAvailable;
        }

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

            return true;
        }
        else
        {
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
}