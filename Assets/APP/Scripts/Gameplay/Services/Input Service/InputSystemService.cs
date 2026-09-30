using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

public class InputSystemService : IInitializable, IDisposable, ITickable
{
    private readonly PlayerInputSystem inputSystem;
    private GameInput Input => inputSystem.Input;

    public event Action<Vector2> OnMouseMoved;
    public event Action<Vector2> OnLeftPressStarted;
    public event Action<Vector2> OnLeftPressEnded;

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
        Input.Player.LeftPress.started += HandleLeftPressStarted;
        Input.Player.LeftPress.canceled += HandleLeftPressCanceled;

        Input.Player.RightPress.started += HandleRightPressStarted;
        Input.Player.RightPress.canceled += HandleRightPressCanceled;

        Input.Player.ScreenPos.performed += HandleMouseMove;
    }

    public void Dispose()
    {
        Input.Player.LeftPress.started -= HandleLeftPressStarted;
        Input.Player.LeftPress.canceled -= HandleLeftPressCanceled;

        Input.Player.RightPress.started -= HandleRightPressStarted;
        Input.Player.RightPress.canceled -= HandleRightPressCanceled;

        Input.Player.ScreenPos.performed -= HandleMouseMove;
    }

    public void Tick()
    {

    }

    private void HandleMouseMove(InputAction.CallbackContext context) => OnMouseMoved.Invoke(context.ReadValue<Vector2>());

    private void HandleLeftPressStarted(InputAction.CallbackContext context) => OnLeftPressStarted.Invoke(context.ReadValue<Vector2>());
    private void HandleLeftPressCanceled(InputAction.CallbackContext context) => OnLeftPressEnded.Invoke(context.ReadValue<Vector2>());

    private void HandleRightPressStarted(InputAction.CallbackContext context) => OnRightPressStarted.Invoke();
    private void HandleRightPressCanceled(InputAction.CallbackContext context) => OnRightPressEnded.Invoke();

}
