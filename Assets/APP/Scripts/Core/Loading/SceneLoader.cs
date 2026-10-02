using UnityEngine;
// Hapus using System.Threading.Tasks;

public class SceneLoader
{
    private readonly ProjectLoadingService _loadingService;

    public SceneLoader(ProjectLoadingService loadingService) => _loadingService = loadingService;

    /// <summary>
    /// <param name="minDuration">Minimum duration in seconds (e.g: 2.0f)</param>
    /// <param name="customMessage">Custom message to display</param>
    /// </summary>
    public void LoadSceneAsync(string sceneName, float minDuration = 0f, string customMessage = "")
    {
        // Alihkan tugas coroutine ke LoadingService
        _loadingService.StartLoadingScene(sceneName, minDuration, customMessage);
    }
}