using UnityEngine;

[CreateAssetMenu(fileName = nameof(GameConfigData), menuName = "App/Data/Game Config Data")]
public class GameConfigData : ScriptableObject
{
    [Header("Input & Game Feel")]
    public float scrollSensitivity = 10f;
    public float rotateSensitivity = 0.2f;
    public float dragThreshold = 25f;
    public float holdDelay = 0.2f;
    public float holdDuration = 0.5f;
    public float holdMoveTolerance = 5f;

    [Header("System Settings")]
    public float autoSaveCooldown = 1.0f;
    public float minLoadingScreenDuration = 2.0f;
    public float bgmFadeDuration = 2.0f;
}