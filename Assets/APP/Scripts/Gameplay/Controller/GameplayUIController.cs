using TMPro;
using UnityEngine;

public class GameplayUIController : BaseMenuController
{
    [Header("UI Progressbars")]
    [SerializeField] private GameplayProgressBarUI cleanProgressBarUI;
    [SerializeField] private GameplayProgressBarUI assembleProgressBarUI;
    [SerializeField] private ProgressBarUI holdProgressUI;
    [SerializeField] private GameObject failedContainer;
    [SerializeField] private TextMeshProUGUI failedText;

    public override void SetActive(bool isActive)
    {
        base.SetActive(isActive);
    }

    public void ShowCleanProgress()
    {
        if (cleanProgressBarUI != null) cleanProgressBarUI.Show();
    }

    public void UpdateCleanProgress(float normalizedValue)
    {
        if (cleanProgressBarUI != null) cleanProgressBarUI.UpdateProgress(normalizedValue);
    }

    public void HideCleanProgress()
    {
        if (cleanProgressBarUI != null) cleanProgressBarUI.Hide();
    }


    public void ShowAssembleProgress()
    {
        if (assembleProgressBarUI != null) assembleProgressBarUI.Show();
    }

    public void UpdateAssembleProgress(float normalizedValue)
    {
        if (assembleProgressBarUI != null) assembleProgressBarUI.UpdateProgress(normalizedValue);
    }

    public void HideAssembleProgress()
    {
        if (assembleProgressBarUI != null) assembleProgressBarUI.Hide();
    }

    public void ShowHoldProgress(Vector2 screenPosition)
    {
        if (holdProgressUI != null) holdProgressUI.Show(screenPosition);
    }

    public void UpdateHoldProgress(float normalizedValue)
    {
        if (holdProgressUI != null) holdProgressUI.UpdateProgress(normalizedValue);
    }

    public void HideHoldProgress()
    {
        if (holdProgressUI != null) holdProgressUI.Hide();
    }
}