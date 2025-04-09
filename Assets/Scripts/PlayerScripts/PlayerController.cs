using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    /* Assign on inspector */
    [SerializeField] GameObject contextMenu;
    [SerializeField] Button resourceButton;
    [SerializeField] Button craftButton;
    [SerializeField] Button upgradeButton;
    Vector2 lastRightClickPosition;


    #region Initialization
    private void Awake()
    {
        AddInputActions();
        mainCamera = Camera.main;
        InitializeContextMenu();
    }

    private void AddInputActions()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Player.MousePosition.performed += ctx => OnMousePosition(ctx);
        inputActions.Player.MouseLeftClick.performed += ctx => OnMouseLeftClick();
        inputActions.Player.MouseLeftClick.started += ctx => OnMouseLeftClickStarted(ctx);
        inputActions.Player.MouseLeftClick.canceled += ctx => OnMouseLeftClickCanceled();
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


    #region Action Events
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

    private void OnMouseLeftClickCanceled()
    {
        isDragging = false;
    }

    private void OnMouseRightClick()
    {
        lastRightClickPosition = lastMousePosition;

        // 화면 좌표를 월드 좌표로 변환
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(lastMousePosition.x, lastMousePosition.y, mainCamera.nearClipPlane));
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero); // 2D Raycast

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Manor"))
            {
                ToggleContextMenu(); // "Manor"에 충돌했을 때만 메뉴 표시
            }
        }
    }
    #endregion


    private void MoveMap(Vector2 delta)
    {
        if (mainCamera == null) return;

        Vector3 moveDirection = new Vector3(-delta.x, -delta.y, 0) * dragSpeed;
        mainCamera.transform.position += moveDirection;

        //Debug.Log($"Camera moved by {moveDirection}");
    }



    #region Context Menu

    private void InitializeContextMenu()
    {
        if (contextMenu == null || resourceButton == null || craftButton == null || upgradeButton == null)
        {
            Debug.LogError("Context menu or buttons not assigned in inspector");
            return;
        }

        resourceButton.onClick.AddListener(OnResourceSelected);
        craftButton.onClick.AddListener(OnCraftSelected);
        upgradeButton.onClick.AddListener(OnUpgradeSelected);

        contextMenu.SetActive(false);
    }

    private void ToggleContextMenu()
    {
        bool isActive = contextMenu.activeSelf;
        contextMenu.SetActive(!isActive);

        if (!isActive)
        {
            RectTransform menuRect = contextMenu.GetComponent<RectTransform>();
            Vector2 clampedPosition = new Vector2(
                Mathf.Clamp(lastRightClickPosition.x, 0, Screen.width - menuRect.sizeDelta.x),
                Mathf.Clamp(lastRightClickPosition.y, 0, Screen.height - menuRect.sizeDelta.y)
            );
            menuRect.position = clampedPosition;
        }
    }

    private void OnResourceSelected()
    {
        Debug.Log("Resource selected");
        contextMenu.SetActive(false);
    }

    private void OnCraftSelected()
    {
        Debug.Log("Craft selected");
        contextMenu.SetActive(false);
    }

    private void OnUpgradeSelected()
    {
        Debug.Log("Upgrade selected");
        contextMenu.SetActive(false);
    }
    #endregion
}
