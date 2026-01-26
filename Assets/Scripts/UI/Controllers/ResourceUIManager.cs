using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UIElements;
using Resource = Data.Resources.Resources;

/*
 * Manage the resource bar and the potato amount display of the quest
 */

//TODO: Change once ui is ready
public class ResourceUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private ResourceManager resourceManager;

    private VisualElement _root;

    private List<Label> _resourceLabels = new List<Label>();

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        _resourceLabels = uiDocument.rootVisualElement.Query<Label>().ToList();

        var amounts = resourceManager.GetResourcesAmount();
        SetupResourcesUI(amounts);
    }

    //The name of the ScriptableObject name must match the Labels name
    private void SetupResourcesUI(Dictionary<Resource, int> resources)
    {
        foreach (var resourceLabel in _resourceLabels)
        {
            foreach (var resource in resources)
            {
                if (resourceLabel.name == resource.Key.resourceName)
                {
                    resourceLabel.text = resource.Value.ToString();
                }
            }
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