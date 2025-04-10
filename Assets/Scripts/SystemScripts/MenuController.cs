using System.Collections;
using System.Linq;
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
    [SerializeField] Button _startButton;
    [SerializeField] TextMeshProUGUI _resourceRequirementText;
    [SerializeField] Button _closeButton;

    [SerializeField] int _targetNumber = 0;
    [SerializeField] int _attemptCount = 0;
    [SerializeField] Village _currentVillage;


    [Header("Crafting")]
    [SerializeField] string[] _resources = { "Wood", "Stone", "Iron" };

    [SerializeField] TextMeshProUGUI _plankText;
    [SerializeField] Button _plankCraftButton;
    [SerializeField] Slider _plankProgressBar;

    [SerializeField] TextMeshProUGUI _stoneAxeText;
    [SerializeField] Button _stoneAxeCraftButton;
    [SerializeField] Slider _stoneAxeProgressBar;

    [SerializeField] TextMeshProUGUI _IronAxeText;
    [SerializeField] Button _IronAxeCraftButton;
    [SerializeField] Slider _IronAxeProgressBar;


    /* Other states */
    Vector2 _lastRightClickPosition;
    Coroutine _resourceUpdateCoroutine;

    

    private void Start()
    {
        _resourceButton.onClick.AddListener(OnResourceButtonClicked);
        _craftButton.onClick.AddListener(OnCraftButtonClicked);
        _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

        _resourceBackButton.onClick.AddListener(HideAllCanvases);
        _craftBackButton.onClick.AddListener(HideAllCanvases);
        _upgradeBackButton.onClick.AddListener(HideAllCanvases);

        _submitButton.onClick.AddListener(OnSubmitNumber);
        _startButton.onClick.AddListener(StartUpDownGameWithCost);

        _closeButton.onClick.AddListener(CloseUpDownGameCanvas);

        _contextMenu.SetActive(false);
        InitializeCraftingUI();

        CraftManager.Instance.OnInventoryUpdated.AddListener(UpdateCraftingUI);
    }

    private void OnDestroy()
    {
        CraftManager.Instance.OnInventoryUpdated.RemoveListener(UpdateCraftingUI);
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

    public void CloseUpDownGameCanvas()
    {
        _upDownGameCanvas.SetActive(false);
    }
    #endregion


    // Updown game logic should split later
    #region Updown Game
    public void StartUpDownGame(Village village)
    {
        _currentVillage = village;
        _hintText.text = $"Select number between {village.MinValue} to {village.MaxValue}";
        _numberInput.text = "";
        UpdateResourceRequirement();
        ShowCanvas(_upDownGameCanvas);
        _submitButton.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(true);

        if (_resourceUpdateCoroutine != null)
        {
            StopCoroutine(_resourceUpdateCoroutine);
        }
        _resourceUpdateCoroutine = StartCoroutine(ResourceRequirementUpdateCoroutine());
    }

    private void StartUpDownGameWithCost()
    {
        if (GameManager.Instance.UseResource(_currentVillage.PurchaseResourceType, _currentVillage.PurchaseResourceCost))
        {
            _targetNumber = Random.Range(_currentVillage.MinValue, _currentVillage.MaxValue);
            _attemptCount = 0;
            _hintText.text = $"Select number between {_currentVillage.MinValue} to {_currentVillage.MaxValue}";
            _submitButton.gameObject.SetActive(true);
            _startButton.gameObject.SetActive(false);
            UpdateResourceRequirement();
        }
        else
        {
            _hintText.text = "More resource are require!!";
        }
    }

    private void OnSubmitNumber()
    {
        if (int.TryParse(_numberInput.text, out int guess))
        {
            _attemptCount++;
            if (guess < _currentVillage.MinValue || guess > _currentVillage.MaxValue)
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
                GameManager.Instance.AddVillage(_currentVillage);
                Invoke(nameof(HideAllCanvases), 1f);
            }
        }
        else
        {
            _hintText.text = "Input valid number";
        }
        _numberInput.text = "";
    }

    private void UpdateResourceRequirement()
    {
        int currentAmount = GameManager.Instance.GetResource(_currentVillage.PurchaseResourceType);
        int requiredAmount = _currentVillage.PurchaseResourceCost;
        _resourceRequirementText.text = $"{_currentVillage.PurchaseResourceType}: {currentAmount} / {requiredAmount}";
        _startButton.interactable = currentAmount >= requiredAmount;
    }

    private IEnumerator ResourceRequirementUpdateCoroutine()
    {
        while (_upDownGameCanvas.activeSelf && _currentVillage != null)
        {
            UpdateResourceRequirement();
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion


    #region Crafting

    private void InitializeCraftingUI()
    {
        var items = CraftManager.Instance.GetCraftableItems();

        // Make Plank 
        UpdateCraftItemText(_plankText, items[0]);
        _plankCraftButton.onClick.AddListener(() => StartCrafting(0));
        _plankProgressBar.value = 0f;

        CraftManager.Instance.OnCraftingStarted.AddListener((name) =>
        {
            if (name == items[0].ItemName) StartCoroutine(UpdateProgressBar(_plankProgressBar, items[0].CraftingTime));
        });
        CraftManager.Instance.OnCraftingCompleted.AddListener((name) =>
        {
            if (name == items[0].ItemName) _plankProgressBar.value = 0f;
        });

        // Make Stone Axe
        UpdateCraftItemText(_stoneAxeText, items[1]);
        _stoneAxeCraftButton.onClick.AddListener(() => StartCrafting(1));
        _stoneAxeProgressBar.value = 0f;

        CraftManager.Instance.OnCraftingStarted.AddListener((name) =>
        {
            if (name == items[1].ItemName) StartCoroutine(UpdateProgressBar(_stoneAxeProgressBar, items[1].CraftingTime));
        });
        CraftManager.Instance.OnCraftingCompleted.AddListener((name) =>
        {
            if (name == items[1].ItemName) _stoneAxeProgressBar.value = 0f;
        });

        // Make Iron Axe
        UpdateCraftItemText(_IronAxeText, items[2]);
        _IronAxeCraftButton.onClick.AddListener(() => StartCrafting(2));
        _IronAxeProgressBar.value = 0f;

        CraftManager.Instance.OnCraftingStarted.AddListener((name) =>
        {
            if (name == items[2].ItemName) StartCoroutine(UpdateProgressBar(_IronAxeProgressBar, items[2].CraftingTime));
        });
        CraftManager.Instance.OnCraftingCompleted.AddListener((name) =>
        {
            if (name == items[2].ItemName) _IronAxeProgressBar.value = 0f;
        });
    }

    private void StartCrafting(int index)
    {
        var item = CraftManager.Instance.GetCraftableItems()[index];
        CraftManager.Instance.StartCrafting(item);
    }

    private void UpdateCraftingUI()
    {
        var items = CraftManager.Instance.GetCraftableItems();
        UpdateCraftItemText(_plankText, items[0]);
        UpdateCraftItemText(_stoneAxeText, items[1]);
        UpdateCraftItemText(_IronAxeText, items[2]);
    }

    private void UpdateCraftItemText(TextMeshProUGUI itemText, CraftItem item)
    {
        int count = CraftManager.Instance.GetCraftedItemCount(item.ItemName);
        itemText.text = $"{item.ItemName}\n" +
                        string.Join("\n", item.RequiredResources.Select(r => $"{r.Key}: {r.Value}")) +
                        $"\nOwned: {count}";
    }

    private IEnumerator UpdateProgressBar(Slider progressBar, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            progressBar.value = elapsed / duration;
            yield return null;
        }
        progressBar.value = 1f;
    }
    #endregion
}
