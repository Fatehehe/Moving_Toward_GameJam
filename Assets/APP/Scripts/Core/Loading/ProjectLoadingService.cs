using System;
using System.Collections; // Wajib ditambahkan
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

public class ProjectLoadingService : IInitializable, IDisposable
{
    private ProjectLoadingView loadingView;

    public ProjectLoadingService(GameObject viewSample)
    {
        loadingView = UnityEngine.Object.Instantiate(viewSample).GetComponent<ProjectLoadingView>();
    }

    void IInitializable.Initialize()
    {
    }

    void IDisposable.Dispose()
    {
    }

    public void ShowLoading(string text)
    {
        loadingView.ShowLoading(text);
    }

    public void HideLoading()
    {
        loadingView.HideLoading();
    }

    // --- TAMBAHAN BARU UNTUK SCENE LOADER ---
    public void StartLoadingScene(string sceneName, float minDuration, string customMessage)
    {
        // Jalankan Coroutine menggunakan loadingView (karena ia adalah MonoBehaviour)
        loadingView.StartCoroutine(LoadSceneCoroutine(sceneName, minDuration, customMessage));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float minDuration, string customMessage)
    {
        float startTime = Time.time;

        ShowLoading(customMessage);

        // Asumsikan AudioEvents sudah ada di kodemu
        // AudioEvents.TriggerStopBGM(); 

        // Ganti await Task.Delay(1500) menjadi ini:
        yield return new WaitForSeconds(1.5f);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        // Ganti await Task.Yield() menjadi ini:
        while (!op.isDone)
        {
            yield return null;
        }

        float elapsed = Time.time - startTime;
        if (elapsed < minDuration)
        {
            float delayTime = minDuration - elapsed;
            // Ganti await Task.Delay menjadi ini:
            yield return new WaitForSeconds(delayTime);
        }

        HideLoading();
    }
}