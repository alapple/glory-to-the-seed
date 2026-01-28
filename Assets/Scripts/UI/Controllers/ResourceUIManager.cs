using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UIElements;
using Resource = Data.Resources.Resources;

/*
 * Manage the resource bar and the potato amount display of the quest
 */
namespace UI.Controllers
{
    public class ResourceUIManager : MonoBehaviour
    {
        public static ResourceUIManager Instance;
        
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private ResourceManager resourceManager;

        private VisualElement _root;

        private List<Label> _resourceLabels = new List<Label>();
        private Dictionary<string, Label> _labelCache = new Dictionary<string, Label>();
        
        private VisualElement _resourceAllocator;
        private Label _potatoPerSecond;
        
        private RegionController _currentRegion;

        void Awake()
        {
            Instance = this;
            _resourceLabels = uiDocument.rootVisualElement.Query<Label>().ToList();
            CacheLabels();
            var amounts = resourceManager.GetResourcesAmount();
            SetupResourcesUI(amounts);
            _resourceAllocator = uiDocument.rootVisualElement.Q<VisualElement>("Field_Manager");
            HideResourceAllocator();
        }

        private void FixedUpdate()
        {
            // Update resource UI always
            var amounts = resourceManager.GetResourcesAmount();
            SetupResourcesUI(amounts);
            
            // Wenn ein Feld ausgewählt ist, zeige dessen Stats an
            if (_currentRegion != null)
            {
                UpdatePotatoPerSecond(_currentRegion.production);
            }
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
            /*
            if (_labelCache["Potato"] != null)
            {
                _labelCache.TryGetValue("Potato", out var potato);
                potato.text = "0";
            }
            */
        }

        public void UpdatePotatoPerSecond(int amount)
        {
            if (_labelCache.TryGetValue("PotatoPerSecond", out var potatoPerSecond))
            {
                potatoPerSecond.text = amount.ToString("0000") + "/s";
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

        public void ShowResourceAllocator(RegionController regionController)
        {
            _currentRegion = regionController;
            _resourceAllocator.style.display = DisplayStyle.Flex;
        }

        public void HideResourceAllocator()
        {
            _resourceAllocator.style.display = DisplayStyle.None;
            _currentRegion = null;
        }
        
        public RegionController GetCurrentRegion()
        {
            return _currentRegion;
        }

        // Check if the pointer is over the UI
        public bool IsPointerOverUI(Vector2 screenPosition)
        {
            IPanel panel = uiDocument.rootVisualElement.panel;
    
            Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(panel, screenPosition);

            VisualElement pickedElement = panel.Pick(panelPosition);

            return pickedElement != null && pickedElement != uiDocument.rootVisualElement;
        }
    }
}