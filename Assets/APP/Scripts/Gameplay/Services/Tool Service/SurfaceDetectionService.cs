using UnityEngine;
using VContainer;

public class SurfaceDetectionService
{
    private readonly Camera cam;
    private readonly int pieceLayerMask;
    public Vector3 RaycastNormal { get; private set; }
    public Vector3 RaycastPos { get; private set; } = Vector3.positiveInfinity;
    public float TipRotation { get; private set; }
    public Vector2 TextureSurface { get; private set; }
    public bool HasHit { get; private set; }

    public ICleanable CleanableSurface { get; private set; }

    [Inject]
    public SurfaceDetectionService(Camera cam)
    {
        this.cam = cam;
        pieceLayerMask = LayerMask.GetMask("Dirts");
    }

    public bool DetectSurface(Vector2 screenPos)
    {
        ResetDetection();

        Ray ray = cam.ScreenPointToRay(screenPos);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, pieceLayerMask))
        {
            return false;
        }

        CleanableSurface = hit.collider.GetComponentInParent<ICleanable>();

        if (CleanableSurface != null && CleanableSurface.IsCleanable())
        {
            HasHit = true;
            EssentialDetecting(hit);
            return true;
        }

        return false;
    }

    private void ResetDetection()
    {
        HasHit = false;
        CleanableSurface = null;
        RaycastPos = Vector3.positiveInfinity;
    }

    private void EssentialDetecting(RaycastHit hit)
    {
        RaycastNormal = hit.normal;
        RaycastPos = hit.point;

        Vector3 projectedUp = Vector3.ProjectOnPlane(Vector3.up, hit.normal);
        TipRotation = Vector3.SignedAngle(Vector3.up, projectedUp, hit.normal);
        TextureSurface = hit.textureCoord;
    }
}