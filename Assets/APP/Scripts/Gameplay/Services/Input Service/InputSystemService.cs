using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class InputSystemService : IInitializable, IDisposable, ITickable
{
    private readonly PlayerInputSystem inputSystem;
    private GameInput Input => inputSystem.Input;

    public event Action<Vector2> OnMouseMoved;
    public event Action OnLeftPressStarted;
    public event Action OnLeftPressEnded;

    public event Action OnRightPressStarted;
    public event Action OnRightPressEnded;

    public event Action<float> OnScrollPerformed;

    [Inject]
    public InputSystemService(PlayerInputSystem inputSystem)
    {
        this.inputSystem = inputSystem;
    }

    public void Initialize()
    {

    }

    public void Dispose()
    {

    }

    public void Tick()
    {

    }
}
