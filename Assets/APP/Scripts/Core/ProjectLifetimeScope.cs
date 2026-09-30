using Modules.SoundSystems;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    [SerializeField] private SoundSystem soundSystem;
    [SerializeField] private GameConfigData gameConfigData;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(gameConfigData);

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
