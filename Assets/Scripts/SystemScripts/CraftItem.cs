using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CraftItem 
{
    public string ItemName;
    public Dictionary<string, int> RequiredResources;
    public float CraftingTime;

    public CraftItem(string name, Dictionary<string, int> resources, float time)
    {
        ItemName = name;
        RequiredResources = resources;
        CraftingTime = time;
    }
}
