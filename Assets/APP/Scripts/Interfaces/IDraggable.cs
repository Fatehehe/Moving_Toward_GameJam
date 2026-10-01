using UnityEngine;

public interface IDraggable
{
    void OnDragStarted(Vector3 worldPos);
    void OnDragPerformed(Vector3 worldPos);
    void OnDragEnded(Vector3 worldPos);
}