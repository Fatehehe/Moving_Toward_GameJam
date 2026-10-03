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

    public Vector2 mousePosition = Vector2.zero;

    [Inject]
    public InputSystemService(PlayerInputSystem inputSystem)
    {
        this.inputSystem = inputSystem;
    }

    public void Initialize()
    {
        ChangeInputState(InputStateType.Player);

        Input.Player.Press.started += HandleLeftPressStarted;
        Input.Player.Press.canceled += HandleLeftPressCanceled;

        Input.Player.RightPress.started += HandleRightPressStarted;
        Input.Player.RightPress.canceled += HandleRightPressCanceled;

        Input.Player.ScreenPos.performed += HandleMouseMove;

        Input.Player.Scroll.performed += HandleScrollPerformed;
    }

    public void Dispose()
    {
        Input.Player.Press.started -= HandleLeftPressStarted;
        Input.Player.Press.canceled -= HandleLeftPressCanceled;

        Input.Player.RightPress.started -= HandleRightPressStarted;
        Input.Player.RightPress.canceled -= HandleRightPressCanceled;

        Input.Player.ScreenPos.performed -= HandleMouseMove;

        Input.Player.Scroll.performed -= HandleScrollPerformed;
    }


    public void Tick()
    {

    }

    private void HandleMouseMove(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        OnMouseMoved?.Invoke(context.ReadValue<Vector2>());
    }

    private void HandleLeftPressStarted(InputAction.CallbackContext context)
    {
        Vector2 currentPos = Input.Player.ScreenPos.ReadValue<Vector2>();
        OnLeftPressStarted?.Invoke(currentPos);
    }

    private void HandleLeftPressCanceled(InputAction.CallbackContext context)
    {
        Vector2 currentPos = Input.Player.ScreenPos.ReadValue<Vector2>();
        OnLeftPressEnded?.Invoke(currentPos);
    }

    private void HandleScrollPerformed(InputAction.CallbackContext context) => OnScrollPerformed?.Invoke(context.ReadValue<float>());
    private void HandleRightPressStarted(InputAction.CallbackContext context) => OnRightPressStarted?.Invoke();
    private void HandleRightPressCanceled(InputAction.CallbackContext context) => OnRightPressEnded?.Invoke();

    public void ChangeInputState(InputStateType state) => inputSystem.ChangeInputState(state);
    public Vector2 GetMousePosition() => mousePosition;
}
