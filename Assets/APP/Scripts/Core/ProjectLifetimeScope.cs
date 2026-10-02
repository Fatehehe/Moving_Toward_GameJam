using Modules.SoundSystems;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    [SerializeField] private SoundSystem soundSystem;
    [SerializeField] private GameConfigData gameConfigData;
    [SerializeField] protected GameObject loadingPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(gameConfigData);

        builder.RegisterEntryPoint<ProjectLoadingService>(Lifetime.Singleton).AsSelf().WithParameter(loadingPrefab);
        builder.Register<SceneLoader>(Lifetime.Singleton);

        // Core systems
        SoundSystem soundSystemInstance = Instantiate(soundSystem, transform);
        builder.RegisterComponent(soundSystemInstance).AsSelf();


        // Audio Service
        builder.RegisterEntryPoint<ProjectAudioService>(Lifetime.Singleton).AsSelf();

        // Input System
        builder.RegisterEntryPoint<PlayerInputSystem>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<InputSystemService>(Lifetime.Singleton).AsSelf();
    }
}
