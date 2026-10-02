using TMPro;
using UnityEngine;
using DG.Tweening; // Tambahkan DOTween

public class GameplayUIController : BaseMenuController
{
    [Header("UI Progressbars")]
    [SerializeField] private GameplayProgressBarUI cleanProgressBarUI;
    [SerializeField] private GameplayProgressBarUI assembleProgressBarUI;
    [SerializeField] private ProgressBarUI holdProgressUI;

    [Header("Caution UI")]
    [SerializeField] private GameObject failedContainer;
    [SerializeField] private TextMeshProUGUI failedText;

    private Sequence cautionSequence;

    protected override void Awake()
    {
        base.Awake();
        if (failedContainer != null)
        {
            failedContainer.SetActive(false);
        }
    }

    public override void SetActive(bool isActive)
    {
        base.SetActive(isActive);
    }

    public void ShowCautionText(string message)
    {
        if (failedContainer == null || failedText == null) return;

        // Set teks peringatan
        failedText.text = message;
        failedContainer.SetActive(true);

        // Ambil atau tambahkan CanvasGroup untuk efek Fade (Transparansi)
        CanvasGroup cg = failedContainer.GetComponent<CanvasGroup>();
        if (cg == null) cg = failedContainer.AddComponent<CanvasGroup>();

        // Reset kondisi awal (Transparan & Mengecil)
        cg.alpha = 0f;
        failedContainer.transform.localScale = Vector3.one * 0.8f;

        // Matikan animasi yang mungkin sedang berjalan agar tidak tumpang tindih
        cautionSequence?.Kill();
        cautionSequence = DOTween.Sequence();

        // 1. Muncul (Pop up & Fade In)
        cautionSequence.Append(cg.DOFade(1f, 0.3f))
                       .Join(failedContainer.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack))
        // 2. Tunggu 2 detik
                       .AppendInterval(2f)
        // 3. Hilang (Shrink & Fade Out)
                       .Append(cg.DOFade(0f, 0.3f))
                       .Join(failedContainer.transform.DOScale(0.8f, 0.3f).SetEase(Ease.InBack))
        // 4. Nonaktifkan GameObject
                       .OnComplete(() => failedContainer.SetActive(false));
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