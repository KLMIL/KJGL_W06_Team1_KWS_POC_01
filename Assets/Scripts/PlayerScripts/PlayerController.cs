using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /* Assign on inspector */
    [Header("Assign Field")]
    [SerializeField] MenuController _menuController;

    /* Assign in scripts */
    InputManager _inputManager;
    Camera _mainCamera;

    /* Player state */
    [Header("Player State Field")]
    [SerializeField] bool _isDragging = false;
    [SerializeField] float _dragSpeed = 0.01f;

    Vector2 _previousMousePosition;




    #region Initialization
    private void Awake()
    {
        _mainCamera = Camera.main;
        _inputManager = new InputManager();
    }

    private void Start()
    {
        AddInputActions();
    }

    private void OnDestroy()
    {
        DeleteInputActions();
    }

    private void AddInputActions()
    {
        _inputManager.OnMousePositionChanged += OnMousePositionChanged;
        _inputManager.OnLeftClickStarted += OnLeftClickStarted;
        _inputManager.OnLeftClickCanceled += OnLeftClickCanceled;
        _inputManager.OnRightClick += OnRightClick;
    }
    private void DeleteInputActions()
    {
        _inputManager.OnMousePositionChanged -= OnMousePositionChanged;
        _inputManager.OnLeftClickStarted -= OnLeftClickStarted;
        _inputManager.OnLeftClickCanceled -= OnLeftClickCanceled;
        _inputManager.OnRightClick -= OnRightClick;

        _inputManager.Disable();
        _inputManager.Dispose();
    }
    #endregion



    private void Update()
    {

    }


    #region Action Events
    private void OnMousePositionChanged(Vector2 mousePosition)
    {
        if (_isDragging)
        {
            Vector2 delta = mousePosition - _previousMousePosition;
            MoveMap(delta);
        }
        _previousMousePosition = mousePosition;
    }
    private void MoveMap(Vector2 delta)
    {
        if (_mainCamera == null) return;
        Vector3 moveDirection = new Vector3(-delta.x, -delta.y, 0) * _dragSpeed;
        _mainCamera.transform.position += moveDirection;
    }

    private void OnLeftClickStarted()
    { 
        Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(new Vector3(
                _inputManager.LastMousePosition.x, 
                _inputManager.LastMousePosition.y,
                _mainCamera.nearClipPlane
            ));
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && (hit.collider.CompareTag("Manor") || hit.collider.CompareTag("Village")))
        {
            VillageActivation villageActivation = hit.collider.GetComponent<VillageActivation>();
            if (villageActivation != null && !villageActivation.IsActived)
            {
                _menuController.StartUpDownGame(villageActivation);
                return;
            }
        }
        _isDragging = true;
        _previousMousePosition = _inputManager.LastMousePosition;
    }

    private void OnLeftClickCanceled()
    {
        _isDragging = false;
    }

    private void OnRightClick()
    {
        Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(
            new Vector3(_inputManager.LastMousePosition.x,
                        _inputManager.LastMousePosition.y,
                        _mainCamera.nearClipPlane)
            );
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Manor"))
        {
            _menuController.ShowContextMenu(_inputManager.LastMousePosition);
        }
    }
    #endregion



}
