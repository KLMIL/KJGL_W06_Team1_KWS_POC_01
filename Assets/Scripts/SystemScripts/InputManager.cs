using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    InputSystem_Actions _inputActions;
    public Vector2 LastMousePosition { get; private set; }


    /* Event Delegate */
    public event System.Action<Vector2> OnMousePositionChanged;
    public event System.Action OnLeftClickStarted;
    public event System.Action OnLeftClickCanceled;
    public event System.Action OnRightClick;
    public event System.Action<float> OnMouseWheel;

    public InputManager()
    {
        _inputActions = new InputSystem_Actions();
        SetupInputActions();
        Enable();
    }


    private void SetupInputActions()
    {
        _inputActions.Player.MousePosition.performed += ctx => OnMousePosition(ctx);
        _inputActions.Player.MouseLeftClick.started += ctx => OnLeftClickStarted?.Invoke();
        _inputActions.Player.MouseLeftClick.canceled += ctx => OnLeftClickCanceled?.Invoke();
        _inputActions.Player.MouseRightClick.performed += ctx => OnRightClick?.Invoke();
        _inputActions.Player.MouseWheel.performed += ctx => OnMouseWheel?.Invoke(ctx.ReadValue<Vector2>().y);
    }


    public void Enable()
    {
        _inputActions.Enable();
    }

    public void Disable()
    {
        _inputActions.Disable();
    }


    private void OnMousePosition(InputAction.CallbackContext context)
    {
        LastMousePosition = context.ReadValue<Vector2>();
        OnMousePositionChanged?.Invoke(LastMousePosition);
    }
    public void Dispose()
    {
        _inputActions?.Disable();
        _inputActions?.Dispose();
    }
}
