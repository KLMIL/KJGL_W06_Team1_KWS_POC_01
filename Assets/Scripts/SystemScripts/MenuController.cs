using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    /* Manor Context Menu UI */
    [Header("Manor Context Menu")]
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


    /* Updown Game UI */
    [Header("Updown Game")]
    [SerializeField] GameObject _upDownGameCanvas;
    [SerializeField] TMP_InputField _numberInput;
    [SerializeField] Button _submitButton;
    [SerializeField] TextMeshProUGUI _hintText;


    [SerializeField] int _targetNumber = 0;
    [SerializeField] int _attemptCount = 0;

    [SerializeField] VillageActivation _currentVillage;

    /* Other states */
    Vector2 _lastRightClickPosition;

    

    private void Start()
    {
        _resourceButton.onClick.AddListener(OnResourceButtonClicked);
        _craftButton.onClick.AddListener(OnCraftButtonClicked);
        _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

        _resourceBackButton.onClick.AddListener(HideAllCanvases);
        _craftBackButton.onClick.AddListener(HideAllCanvases);
        _upgradeBackButton.onClick.AddListener(HideAllCanvases);

        _submitButton.onClick.AddListener(OnSubmitNumber);

        _contextMenu.SetActive(false);
    }


    #region Canvas Control
    private void ShowCanvas(GameObject canvasToShow)
    {
        _contextMenu.SetActive(false);

        _resourceCanvas.SetActive(false);
        _craftCanvas.SetActive(false);
        _upgradeCanvas.SetActive(false);
        
        _upDownGameCanvas.SetActive(false);
        
        canvasToShow.SetActive(true);
    }

    private void HideAllCanvases()
    {
        _resourceCanvas.SetActive(false);
        _craftCanvas.SetActive(false);
        _upgradeCanvas.SetActive(false);
        _upDownGameCanvas.SetActive(false);
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


    // Updown game logic should split later
    #region Updown Game
    public void StartUpDownGame(VillageActivation village)
    {
        _currentVillage = village;
        _targetNumber = Random.Range(1, 10);
        _attemptCount = 0;
        _hintText.text = "Select number between 1 to 9";
        _numberInput.text = "";
        ShowCanvas(_upDownGameCanvas);
    }

    private void OnSubmitNumber()
    {
        if (int.TryParse(_numberInput.text, out int guess))
        {
            _attemptCount++;
            if (guess < 1 || guess > 9)
            {
                _hintText.text = "Select number between 1 to 9";
            }
            else if (guess < _targetNumber)
            {
                _hintText.text = "UP";
            }
            else if (guess > _targetNumber)
            {
                _hintText.text = "DOWN";
            }
            else
            {
                _hintText.text = "Correct!";
                _currentVillage.Activate();
                Invoke(nameof(HideAllCanvases), 1f);
            }
        }
        else
        {
            _hintText.text = "Input valid number";
        }
        _numberInput.text = "";
    }

    #endregion
}
