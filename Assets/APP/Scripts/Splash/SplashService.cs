using System.Collections; // Wajib untuk IEnumerator
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using DG.Tweening;
// using Modules; // Aktifkan jika AppLogger ada di dalam namespace ini

[System.Serializable]
public class SplashSettings
{
    public float fadeDuration = 1f;
    public int holdDurationMs = 1500;
    public float targetScale = 1.1f;
    public int endDelayMs = 250;
}

public class SplashService : IStartable
{
    private readonly MonoBehaviour coroutineRunner; // Tambahan untuk menjalankan Coroutine
    private readonly SceneLoader sceneLoader;
    private readonly string targetScene;
    private readonly CanvasGroup canvasGroup;
    private readonly Sprite[] splashSprites;
    private readonly Image splashImage;
    private readonly SplashSettings settings;

    [Inject]
    public SplashService(
        MonoBehaviour coroutineRunner, // Disuntikkan dari LifetimeScope
        SceneLoader sceneLoader,
        string targetScene,
        CanvasGroup canvasGroup,
        Sprite[] splashSprites,
        Image splashImage,
        SplashSettings settings)
    {
        this.coroutineRunner = coroutineRunner;
        this.sceneLoader = sceneLoader;
        this.targetScene = targetScene;
        this.canvasGroup = canvasGroup;
        this.splashSprites = splashSprites;
        this.splashImage = splashImage;
        this.settings = settings;
    }

    void IStartable.Start()
    {
        // Jalankan Coroutine meminjam MonoBehaviour dari Scope
        coroutineRunner.StartCoroutine(PlaySplashSequenceRoutine());
    }

    private IEnumerator PlaySplashSequenceRoutine()
    {
        if (canvasGroup == null || splashImage == null || splashSprites == null || splashSprites.Length == 0)
        {
            Debug.LogWarning("[SplashService] UI references are incomplete. Skipping Splash animation.");
            LoadNextScene();
            yield break;
        }

        canvasGroup.alpha = 0f;

        float totalAnimationDuration = (settings.fadeDuration * 2) + (settings.holdDurationMs / 1000f);

        for (int i = 0; i < splashSprites.Length; i++)
        {
            if (splashSprites[i] == null) continue;

            splashImage.sprite = splashSprites[i];
            splashImage.transform.localScale = Vector3.one;

            splashImage.transform.DOScale(settings.targetScale, totalAnimationDuration).SetEase(Ease.Linear).SetLink(splashImage.gameObject);

            // Fase Fade In
            canvasGroup.DOFade(1f, settings.fadeDuration).SetLink(canvasGroup.gameObject);
            yield return new WaitForSeconds(settings.fadeDuration);

            // Fase Hold (Tunggu di layar)
            yield return new WaitForSeconds(settings.holdDurationMs / 1000f);

            // Fase Fade Out
            canvasGroup.DOFade(0f, settings.fadeDuration).SetLink(canvasGroup.gameObject);
            yield return new WaitForSeconds(settings.fadeDuration);

            splashImage.transform.DOKill();

            // End delay sebelum sprite berikutnya
            yield return new WaitForSeconds(settings.endDelayMs / 1000f);
        }

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        string sceneToLoad = string.IsNullOrEmpty(targetScene) ? "MainMenu" : targetScene;
        // Panggil LoadSceneAsync tanpa "await" atau "_ = " karena sudah menjadi void
        sceneLoader.LoadSceneAsync(sceneToLoad);
    }
}