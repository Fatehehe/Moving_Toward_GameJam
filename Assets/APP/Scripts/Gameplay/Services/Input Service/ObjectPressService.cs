using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ObjectPressService : IInitializable, IDisposable, ITickable
{
    private readonly InputSystemService inputSystemService;
    private readonly GameConfigData config;

    private bool isPressing;
    private bool isHolding;
    private float pressTime;

    private Vector2 pressPosition;
    private Vector2 currentMousePosition;

    public event Action OnPressStarted;
    public event Action OnPressEnded;

    public event Action<float, Vector2> OnHoldPerformed;
    public event Action OnHoldCompleted;
    public event Action OnHoldCanceled;

    [Inject]
    public ObjectPressService(InputSystemService inputSystemService, GameConfigData config)
    {
        this.inputSystemService = inputSystemService;
        this.config = config;
    }

    public void Initialize()
    {
        inputSystemService.OnLeftPressStarted += HandlePressStarted;
        inputSystemService.OnMouseMoved += HandleMouseMoved;
        inputSystemService.OnLeftPressEnded += HandlePressEnded;
    }

    public void Dispose()
    {
        inputSystemService.OnLeftPressStarted -= HandlePressStarted;
        inputSystemService.OnMouseMoved -= HandleMouseMoved;
        inputSystemService.OnLeftPressEnded -= HandlePressEnded;
    }

    public void Tick()
    {
        if (!isPressing) return;

        if (Vector2.Distance(currentMousePosition, pressPosition) > config.holdMoveTolerance)
        {
            CancelHold();
            return;
        }

        float heldTime = Time.time - pressTime;

        if (!isHolding && heldTime >= config.holdDelay)
        {
            isHolding = true;
        }

        if (!isHolding) return;

        float holdTime = heldTime - config.holdDelay;

        OnHoldPerformed?.Invoke(holdTime, inputSystemService.GetMousePosition());

        if (holdTime >= config.holdDuration)
        {
            OnHoldCompleted?.Invoke();
            CancelHold();
        }
    }

    private void HandlePressStarted(Vector2 currentPos)
    {
        isPressing = true;
        isHolding = false;

        pressTime = Time.time;

        pressPosition = currentPos;
        currentMousePosition = currentPos;

        OnPressStarted?.Invoke();
    }

    private void HandleMouseMoved(Vector2 pos)
    {
        currentMousePosition = pos;
    }

    private void HandlePressEnded(Vector2 currentPos)
    {
        currentMousePosition = currentPos;
        OnPressEnded?.Invoke();
        CancelHold();
    }

    private void CancelHold()
    {
        if (!isPressing) return;

        isPressing = false;
        isHolding = false;

        OnHoldCanceled?.Invoke();
    }

    public Vector2 GetCurrentPos() => currentMousePosition;
}