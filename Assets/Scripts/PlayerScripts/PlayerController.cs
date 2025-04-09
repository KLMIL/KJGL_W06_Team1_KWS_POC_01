using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    /* Assign in scripts */
    InputSystem_Actions inputActions;

    /* Drag variables */
    bool isDragging = false;
    Vector2 lastMousePosition;
    [SerializeField] float dragSpeed = 0.01f;

    /* Assign on script */
    Camera mainCamera;



    #region Initialization
    private void Awake()
    {
        AddInputActions();
        mainCamera = Camera.main;
    }

    private void AddInputActions()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Player.MousePosition.performed += ctx => OnMousePosition(ctx);
        inputActions.Player.MouseLeftClick.performed += ctx => OnMouseLeftClick();
        inputActions.Player.MouseLeftClick.started += ctx => OnMouseLeftClickStarted(ctx);
        inputActions.Player.MouseLeftClick.canceled += ctx => OnMouseLeftClickCancled();
        inputActions.Player.MouseRightClick.performed += ctx => OnMouseRightClick();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    #endregion


    private void Update()
    {
       
    }


    private void OnMousePosition(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
        //Debug.Log($"Mouse Position: {mousePosition}");

        if (isDragging)
        {
            Vector2 delta = mousePosition - lastMousePosition;
            MoveMap(delta);
        }

        lastMousePosition = mousePosition;

        // Raycast functions
        //Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        //if (Physics.Raycast(ray, out RaycastHit hit))
        //{
        //    Debug.Log($"Hovering over: {hit.collider.gameObject.name}");
        //}
    }

    private void OnMouseLeftClick()
    {
        Debug.Log("Left clicked");
    }

    private void OnMouseLeftClickStarted(InputAction.CallbackContext context)
    {
        isDragging = true;
        //lastMousePosition = context.ReadValue<Vector2>();
    }

    private void OnMouseLeftClickCancled()
    {
        isDragging = false;
    }

    private void OnMouseRightClick()
    {
        Debug.Log("Right clicked");
    }


    private void MoveMap(Vector2 delta)
    {
        if (mainCamera == null) return;

        Vector3 moveDirection = new Vector3(-delta.x, -delta.y, 0) * dragSpeed;
        mainCamera.transform.position += moveDirection;

        //Debug.Log($"Camera moved by {moveDirection}");
    }
}
