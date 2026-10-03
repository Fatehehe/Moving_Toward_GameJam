using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope
{
    [SerializeField] private Inspection inspect;
    [SerializeField] private GameplayUIManager gameplayUIManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(gameplayUIManager);

        builder.RegisterInstance(Camera.main);
        builder.RegisterComponent(inspect);

        builder.RegisterEntryPoint<GameplayManager>(Lifetime.Scoped).AsSelf();

        builder.RegisterEntryPoint<ObjectDetectionService>(Lifetime.Scoped).AsSelf();
        builder.RegisterEntryPoint<ObjectDragService>(Lifetime.Scoped).AsSelf();
        builder.RegisterEntryPoint<ObjectPressService>(Lifetime.Scoped).AsSelf();
        builder.RegisterEntryPoint<ObjectRotateService>(Lifetime.Scoped).AsSelf();
        builder.RegisterEntryPoint<ObjectZoomService>(Lifetime.Scoped).AsSelf();

        builder.RegisterEntryPoint<ObjectInteractionManager>(Lifetime.Scoped).AsSelf();

        builder.RegisterEntryPoint<PartService>(Lifetime.Scoped).AsSelf();
        builder.RegisterEntryPoint<AssembleService>(Lifetime.Scoped).AsSelf();
        builder.RegisterEntryPoint<ToyManager>(Lifetime.Scoped).AsSelf();

        builder.RegisterEntryPoint<SurfaceDetectionService>(Lifetime.Scoped).AsSelf();
        builder.RegisterEntryPoint<ToolService>(Lifetime.Scoped).AsSelf();
        builder.RegisterEntryPoint<CleaningService>(Lifetime.Scoped).AsSelf();

    }
}
