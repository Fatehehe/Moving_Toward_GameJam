using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Pastikan namespace DOTween ditambahkan

public class ProjectLoadingView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private RectTransform loadingSpinner;

    [Header("Settings")]
    [Tooltip("Waktu yang dibutuhkan untuk 1 putaran penuh (detik). Semakin kecil nilainya, semakin cepat putarannya.")]
    [SerializeField] private float spinDuration = 1f;

    private string baseLoadingText = "Loading";
    private Coroutine loadingTextAnimation;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        loadingPanel.SetActive(false);
    }

    public void ShowLoading(string text)
    {
        loadingPanel.SetActive(true);

        if (text.EndsWith("...")) baseLoadingText = text.Substring(0, text.Length - 3);
        else if (text.EndsWith("..")) baseLoadingText = text.Substring(0, text.Length - 2);
        else if (text.EndsWith(".")) baseLoadingText = text.Substring(0, text.Length - 1);
        else baseLoadingText = text;

        if (loadingTextAnimation != null) StopCoroutine(loadingTextAnimation);
        loadingTextAnimation = StartCoroutine(AnimateLoadingText());

        // --- DOTween Animasi Spinner ---
        if (loadingSpinner != null)
        {
            loadingSpinner.DOKill(); // Matikan tween lama (jika ada) agar tidak double
            loadingSpinner.localEulerAngles = Vector3.zero; // Reset posisi awal

            // Putar 360 derajat di sumbu Z, durasi sesuai spinDuration, dan ulangi selamanya (-1)
            loadingSpinner.DORotate(new Vector3(0, 0, -360), spinDuration, RotateMode.FastBeyond360)
                          .SetEase(Ease.Linear) // Linear agar kecepatan putarannya rata (tidak melambat di akhir)
                          .SetLoops(-1, LoopType.Restart); // Restart membuat putaran nyambung terus
        }

        LoadingEvents.OnLoadingStarted?.Invoke();
    }

    public void HideLoading()
    {
        loadingPanel.SetActive(false);

        if (loadingTextAnimation != null) StopCoroutine(loadingTextAnimation);

        // Matikan putaran spinner saat disembunyikan
        if (loadingSpinner != null)
        {
            loadingSpinner.DOKill();
        }

        LoadingEvents.OnLoadingFinished?.Invoke();
    }

    private IEnumerator AnimateLoadingText()
    {
        string[] dotStates = { "", ".", "..", "..." };
        int i = 0;
        while (true)
        {
            loadingText.text = baseLoadingText + dotStates[i];
            i = (i + 1) % dotStates.Length;
            yield return new WaitForSeconds(0.5f);
        }
    }
}