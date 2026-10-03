using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayProgressBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fill;

    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Settings")]
    [SerializeField] private string suffix = "%";

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateProgress(0f);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateProgress(float normalizedValue)
    {
        float clampedValue = Mathf.Clamp01(normalizedValue);

        if (fill != null)
        {
            fill.fillAmount = clampedValue;
        }

        if (progressText != null)
        {
            int percentage = Mathf.RoundToInt(clampedValue * 100f);
            progressText.text = $"{percentage}{suffix}";
        }
    }

}