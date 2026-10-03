using UnityEngine;

public interface IToolObject
{
    bool IsUsed { get; }
    void Use();
    void Return();
    void PlaySfx(bool isPlaying);
    void PlayVfx(bool isPlaying);
}

public interface IToolBrush
{
    Texture2D GetBrush { get; }
    float BrushScale { get; }
    float BrushStrength { get; }
    Color BrushColor { get; }
    Transform BrushTransform { get; }
    float BrushDepth { get; }
}