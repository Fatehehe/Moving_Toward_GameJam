using VContainer;
using VContainer.Unity;
using UnityEngine;
using UnityEngine.UI;

public class SplashLifetimeScope : LifetimeScope
{
    [SerializeField] private string targetScene;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image splashImage;
    [SerializeField] private Sprite[] splashSprites;

    [Header("Animation Settings")]
    [SerializeField] private SplashSettings splashSettings;

    protected override void Configure(IContainerBuilder builder)
    {
        // Perhatikan tambahan .WithParameter<MonoBehaviour>(this) di bawah ini
        builder.RegisterEntryPoint<SplashService>(Lifetime.Scoped).AsSelf()
            .WithParameter<MonoBehaviour>(this)
            .WithParameter(targetScene)
            .WithParameter(canvasGroup)
            .WithParameter(splashSprites)
            .WithParameter(splashImage)
            .WithParameter(splashSettings);

        bool isMobile = Application.isMobilePlatform;
#if UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        isMobile = true;
#endif

        if (isMobile)
        {
            Application.targetFrameRate = 60;
        }
    }
}