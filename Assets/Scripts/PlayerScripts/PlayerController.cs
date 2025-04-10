using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /* Assign on inspector */
    [Header("Assign Field")]
    [SerializeField] MenuController _menuController;
    [SerializeField] SpriteRenderer _background;

    /* Assign in scripts */
    InputManager _inputManager;
    Camera _mainCamera;

    /* Player state */
    [Header("Player State Field")]
    [SerializeField] bool _isDragging = false;
    [SerializeField] float _dragSpeed = 0.01f;

    Vector2 _previousMousePosition;

    /* Other state */
    Vector2 _mapMinBounds;
    Vector2 _mapMaxBounds;

    [Header("Mouse Wheel Event State Field")]
    [SerializeField] float _zoomSpeed = 1f;
    [SerializeField] float _minZoom = 2f;
    [SerializeField] float _maxZoom = 10f;



    #region Initialization
    private void Awake()
    {
        _mainCamera = Camera.main;
        _inputManager = new InputManager();
    }

    private void Start()
    {
        AddInputActions();
        SetupMapBounds();
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
        _inputManager.OnMouseWheel += OnMouseWheel;
    }
    private void DeleteInputActions()
    {
        _inputManager.OnMousePositionChanged -= OnMousePositionChanged;
        _inputManager.OnLeftClickStarted -= OnLeftClickStarted;
        _inputManager.OnLeftClickCanceled -= OnLeftClickCanceled;
        _inputManager.OnRightClick -= OnRightClick;
        _inputManager.OnMouseWheel -= OnMouseWheel;

        _inputManager.Disable();
        _inputManager.Dispose();
    }

    private void SetupMapBounds()
    {
        if (_background == null)
        {
            Debug.LogError("Background spriterenderer is not assigned");
            return;
        }

        Bounds bounds = _background.bounds;
        Vector2 camSize = GetCameraSize();

        _mapMinBounds = new Vector2(
                bounds.min.x + camSize.x / 2,
                bounds.min.y + camSize.y / 2
            );
        _mapMaxBounds = new Vector2(
                bounds.max.x - camSize.x / 2,
                bounds.max.y - camSize.y / 2
            );
    }

    #endregion



    private void Update()
    {

    }


    #region Action Events
    private void OnMousePositionChanged(Vector2 mousePosition)
    {
        Vector2 clampedPosition = new Vector2(
                Mathf.Clamp(mousePosition.x, 0, Screen.width),
                Mathf.Clamp(mousePosition.y, 0, Screen.height)
            );

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
        Vector3 newPosition = _mainCamera.transform.position + moveDirection;

        SetupMapBounds();
        newPosition.x = Mathf.Clamp(newPosition.x, _mapMinBounds.x, _mapMaxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, _mapMinBounds.y, _mapMaxBounds.y);
        
        _mainCamera.transform.position = newPosition;
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
            Village villageActivation = hit.collider.GetComponent<Village>();
            if (villageActivation != null && !villageActivation.IsActived)
            {
                _menuController.StartUpDownGame(villageActivation);
                return;
            }
        }
        _isDragging = true;
        _previousMousePosition = new Vector2(
                Mathf.Clamp(_inputManager.LastMousePosition.x, 0, Screen.width),
                Mathf.Clamp(_inputManager.LastMousePosition.y, 0, Screen.height)
            );
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

    private void OnMouseWheel(float delta)
    {
        if (_mainCamera == null) return;

        float newSize = _mainCamera.orthographicSize - delta * _zoomSpeed;
        _mainCamera.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);

        Vector3 newPosition = _mainCamera.transform.position;
        SetupMapBounds();

        newPosition.x = Mathf.Clamp(newPosition.x, _mapMinBounds.x, _mapMaxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, _mapMinBounds.y, _mapMaxBounds.y);
        _mainCamera.transform.position = newPosition;
    }

    #endregion



    #region Utility
    private Vector2 GetCameraSize()
    {
        float height = _mainCamera.orthographicSize * 2;
        float width = height * _mainCamera.aspect;

        return new Vector2(width, height);
    }
    #endregion
}
