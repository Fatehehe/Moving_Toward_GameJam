using UnityEngine;
using VContainer;
using VContainer.Unity;

public class EndingLifetimeScope : LifetimeScope
{
    [SerializeField] private EndingManager endingManager;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(endingManager);
    }
}
