using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /* Singleton instance */
    static GameManager _instance;
    public static GameManager Instance => _instance;

    /* Resource States */
    Dictionary<string, int> resources = new Dictionary<string, int>
    {
        {"Wood", 0 },
        {"Stone", 0 },
        {"Iron", 0 }
    };

    List<Village> villages = new List<Village>();
    [SerializeField] TextMeshProUGUI resourceText;



    #region Initialization

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        FindAllVillages();
    }

    private void FindAllVillages()
    {
        villages.AddRange(FindObjectsOfType<Village>());
        StartCoroutine(ResourceProductionCoroutine());
        UpdateResourceUI();
    }

    #endregion


    #region Resource functions

    public void AddVillage(Village village)
    {
        if (!villages.Contains(village))
        {
            villages.Add(village);
        }
    }

    private IEnumerator ResourceProductionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            foreach (Village village in villages)
            {
                if (village.IsActived)
                {
                    AddResource(village.ResourceType, village.ResourcePerSecond);
                }
            }
            UpdateResourceUI();
        }
    }

    public void AddResource(string resourceName, int amount = 1)
    {
        if (resources.ContainsKey(resourceName))
        {
            resources[resourceName] += amount;
        }
        else
        {
            resources[resourceName] = amount;
        }
        UpdateResourceUI();
    }

    public bool UseResource(string resourceName, int amount)
    {
        if (resources.ContainsKey(resourceName) && resources[resourceName] >= amount)
        {
            resources[resourceName] -= amount;
            UpdateResourceUI();
            return true;
        }
        return false;
    }

    public int GetResource(string resourceName)
    {
        return resources.ContainsKey(resourceName) ? resources[resourceName] : 0;   
    }

    private void UpdateResourceUI()
    {
        if (resourceText != null)
        {
            resourceText.text =
                $"Wood: {resources["Wood"]}\n" +
                $"Stone: {resources["Stone"]}\n" +
                $"Iron: {resources["Iron"]}";
        }
    }
    
    #endregion
}
