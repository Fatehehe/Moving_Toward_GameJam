using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StoryLifetimeScope : LifetimeScope
{
    [SerializeField] private StoryManager storyManager;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(storyManager);
    }
}
