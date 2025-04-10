using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CraftManager : MonoBehaviour
{
    static CraftManager _instance;
    public static CraftManager Instance => _instance;

    [SerializeField] List<CraftItem> _craftableItems = new List<CraftItem>
    {
        new CraftItem("Plank",
            new Dictionary<string, int> {{"Wood", 5}}, 1f
            ),
        new CraftItem("StoneAxe",
            new Dictionary<string, int> { { "Wood", 3 }, { "Stone", 2} }, 2f
            ),
        new CraftItem("IronAxe",
            new Dictionary<string, int> { { "Wood", 5 }, {"Iorn", 3 } }, 3f
            )
    };

    Dictionary<string, int> _craftedItems = new Dictionary<string, int>();

    public List<CraftItem> GetCraftableItems() => _craftableItems;
    public int GetCraftedItemCount(string itemName) => _craftedItems.ContainsKey(itemName) ? _craftedItems[itemName] : 0;

    public UnityEvent<string> OnCraftingStarted;
    public UnityEvent<string> OnCraftingCompleted;
    public UnityEvent OnInventoryUpdated;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        foreach (var item in _craftableItems) {
            _craftedItems[item.ItemName] = 0;
        }
    }


    public bool CanCraft(CraftItem item)
    {
        foreach (var resource in item.RequiredResources)
        {
            if (GameManager.Instance.GetResource(resource.Key) < resource.Value)
            {
                return false;
            }
        }
        return true;
    }

    public void StartCrafting(CraftItem item)
    {
        if (CanCraft(item))
        {
            foreach(var resource in item.RequiredResources)
            {
                GameManager.Instance.UseResource(resource.Key, resource.Value);
            }
            StartCoroutine(CraftCoroutine(item));
        }
    }

    private IEnumerator CraftCoroutine(CraftItem item)
    {
        OnCraftingStarted?.Invoke(item.ItemName);
        yield return new WaitForSeconds(item.CraftingTime);
        _craftedItems[item.ItemName]++;
        OnCraftingCompleted?.Invoke(item.ItemName);
        OnInventoryUpdated?.Invoke();
    }
}
