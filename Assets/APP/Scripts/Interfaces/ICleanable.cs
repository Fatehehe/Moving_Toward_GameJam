using UnityEngine;

public interface ICleanable
{
    bool IsCleanable();
    void CleanSurface(Vector3 hitPoint, Texture2D brush, Vector3 hitNormal, Vector3 direction, float scale, float strength);
    float GetCleaningProgress();
    void ShowClue(bool isShowing);
    bool IsDustRemoved();
    public void ForceClean();
}

public interface ICleanPart
{
    bool IsCleanable();
}