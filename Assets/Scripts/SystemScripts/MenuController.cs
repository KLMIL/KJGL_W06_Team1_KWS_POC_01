using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] Button _resourceButton;
    [SerializeField] Button _craftButton;
    [SerializeField] Button _upgradeButton;

    //[SerializeField] GameObject _mainCanvas;
    [SerializeField] GameObject _resourceCanvas;
    [SerializeField] GameObject _craftCanvas;
    [SerializeField] GameObject _upgradeCanvas;

    [SerializeField] Button _resourceBackButton;
    [SerializeField] Button _craftBackButton;
    [SerializeField] Button _upgradeBackButton;

    [SerializeField] GameObject _contextMenu;

    Vector2 _lastRightClickPosition;


    private void Start()
    {
        _resourceButton.onClick.AddListener(OnResourceButtonClicked);
        _craftButton.onClick.AddListener(OnCraftButtonClicked);
        _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

        _resourceBackButton.onClick.AddListener(HideAllCanvases);
        _craftBackButton.onClick.AddListener(HideAllCanvases);
        _upgradeBackButton.onClick.AddListener(HideAllCanvases);

        _contextMenu.SetActive(false);
    }


    #region Canvas Control
    private void ShowCanvas(GameObject canvasToShow)
    {
        _contextMenu.SetActive(false);

        _resourceCanvas.SetActive(false);
        _craftCanvas.SetActive(false);
        _upgradeCanvas.SetActive(false);
        
        canvasToShow.SetActive(true);
    }

    private void HideAllCanvases()
    {
        _resourceCanvas.SetActive(false);
        _craftCanvas.SetActive(false);
        _upgradeCanvas.SetActive(false);
        _contextMenu.SetActive(false);
    }

    private void OnResourceButtonClicked() => ShowCanvas(_resourceCanvas);
    private void OnCraftButtonClicked() => ShowCanvas(_craftCanvas);
    private void OnUpgradeButtonClicked() => ShowCanvas(_upgradeCanvas);
    #endregion

    #region Context Menu
    public void ShowContextMenu(Vector2 screenPosition)
    {
        _lastRightClickPosition = screenPosition;
        _contextMenu.SetActive(true);

        RectTransform menuRect = _contextMenu.GetComponent<RectTransform>();
        Vector2 clampedPosition = new Vector2(
                Mathf.Clamp(_lastRightClickPosition.x, 0, Screen.width - menuRect.sizeDelta.x),
                Mathf.Clamp(_lastRightClickPosition.y, 0, Screen.height - menuRect.sizeDelta.y)
            );
        menuRect.position = clampedPosition;
    }

    public void HideContext()
    {
        _contextMenu.SetActive(false);
    }
    #endregion
}
