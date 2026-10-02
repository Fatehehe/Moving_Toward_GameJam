using UnityEditor.EditorTools;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameplayLifetimeScope : LifetimeScope
{
    [SerializeField] private Inspection inspect;
    [SerializeField] private ProgressBarUI holdProgressUI;


    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(Camera.main);
        builder.RegisterComponent(inspect);
        builder.RegisterComponent(holdProgressUI);

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
