using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private RectTransform rectTransform;

    private bool isShowing;

    public void Show(Vector2 screenPosition)
    {
        gameObject.SetActive(true);
        rectTransform.position = screenPosition;

        fill.fillAmount = 0f;
        isShowing = true;
    }

    public void UpdateProgress(float normalized)
    {
        if (!isShowing) return;
        fill.fillAmount = normalized;
    }

    public void Hide()
    {
        isShowing = false;
        fill.fillAmount = 0f;
        gameObject.SetActive(false);
    }
}