using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    InputSystem_Actions _inputActions;
    public Vector2 LastMousePosition { get; private set; }


    /* Event Delegate */
    public System.Action<Vector2> OnMousePositionChanged;
    public System.Action OnLeftClickStarted;
    public System.Action OnLeftClickCanceled;
    public System.Action OnRightClick;

    public InputManager()
    {
        SetupInputActions();
        Enable();
    }


    private void SetupInputActions()
    {
        _inputActions = new InputSystem_Actions();

        _inputActions.Player.MousePosition.performed += ctx => OnMousePosition(ctx);
        _inputActions.Player.MouseLeftClick.started += ctx => OnLeftClickStarted?.Invoke();
        _inputActions.Player.MouseLeftClick.canceled += ctx => OnLeftClickCanceled?.Invoke();
        _inputActions.Player.MouseRightClick.performed += ctx => OnRightClick?.Invoke();
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
