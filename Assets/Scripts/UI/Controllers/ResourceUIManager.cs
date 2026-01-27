using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UIElements;
using Resource = Data.Resources.Resources;

/*
 * Manage the resource bar and the potato amount display of the quest
 */
public class ResourceUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private ResourceManager resourceManager;

    private VisualElement _root;

    private List<Label> _resourceLabels = new List<Label>();
    private Dictionary<string, Label> _labelCache = new Dictionary<string, Label>();

    void Awake()
    {
        _resourceLabels = uiDocument.rootVisualElement.Query<Label>().ToList();
        CacheLabels();
        var amounts = resourceManager.GetResourcesAmount();
        SetupResourcesUI(amounts);
    }

    private void CacheLabels()
    {
        foreach (var label in _resourceLabels)
        {
            _labelCache[label.name] = label;
        }
    }

    //The name of the ScriptableObject name must match the Labels name
    private void SetupResourcesUI(Dictionary<Resource, int> resources)
    {
        foreach (var resource in resources)
        {
            if (_labelCache.TryGetValue(resource.Key.resourceName, out var label))
            {
                label.text = resource.Value.ToString();
            }
        }

        if (_labelCache["Potato"] != null)
        {
            _labelCache.TryGetValue("Potato", out var potato);
            potato.text = "0";
        }
    }

    public void ChangeResources(string uiTreeName, int amount)
    {
        var resources = resourceManager.resources;
        Resource resourceChanged = null;
        foreach (var resource in resources)
        {
            if (resource.name == uiTreeName)
            {
                Debug.Log(resource.name);
                resourceChanged = resource;
            }
        }

        foreach (var resourceLabel in _resourceLabels)
        {
            if (resourceLabel.name == uiTreeName)
            {
                resourceLabel.text = (Int32.Parse(resourceLabel.text) + amount).ToString();
                resourceManager.AddResource(resourceChanged, amount);
            }
        }
    }
}