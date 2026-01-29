using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UIElements;
using Resource = Data.Resources.Resources;

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
            
            var addPotatoBtn = uiDocument.rootVisualElement.Q<Button>("GivePotato");
            addPotatoBtn.RegisterCallback<ClickEvent>(evt => GiveResourceToCurrentRegion("Potato"));
            
            var addVodkaBtn = uiDocument.rootVisualElement.Q<Button>("GiveVodka");
            addVodkaBtn.RegisterCallback<ClickEvent>(evt => GiveResourceToCurrentRegion("Vodka"));
            
            var addWorkerBtn = uiDocument.rootVisualElement.Q<Button>("GiveWorker");
            addWorkerBtn.RegisterCallback<ClickEvent>(evt => GiveResourceToCurrentRegion("Workers"));
        }

        private void GiveResourceToCurrentRegion(string resourceName)
        {
            if (_currentRegion == null) return;

            Resource res = resourceManager.GetResourceByName(resourceName);
            if (res != null)
            {
                _currentRegion.HandleResourceDrop(res, 1);
            }
        }

        private void FixedUpdate()
        {
            var amounts = resourceManager.GetResourcesAmount();
            SetupResourcesUI(amounts);
            
            if (_currentRegion != null)
            {
                UpdatePotatoPerSecond(_currentRegion.production);
                UpdateHappiness(_currentRegion.happiness);
                UpdateAssignedWorkers(_currentRegion.assignedWorkers);
            }
        }

        private void CacheLabels()
        {
            foreach (var label in _resourceLabels)
            {
                _labelCache[label.name] = label;
            }
        }

        private void SetupResourcesUI(Dictionary<Resource, int> resources)
        {
            foreach (var resource in resources)
            {
                if (_labelCache.TryGetValue(resource.Key.resourceName, out var label))
                {
                    label.text = resource.Value.ToString();
                }
            }
        }

        public void UpdatePotatoPerSecond(int amount)
        {
            if (_labelCache.TryGetValue("PotatoPerSecond", out var potatoPerSecond))
            {
                potatoPerSecond.text = amount.ToString("0000") + "/s";
            }
        }

        public void UpdateHappiness(int amount)
        {
            if (_labelCache.TryGetValue("Happiness", out var happiness))
            {
                happiness.text = "Happiness: " + amount.ToString();
            }
        }
        
        private void UpdateAssignedWorkers(int amount)
        {
            // Update a label showing assigned workers if it exists in the UI
            if (_labelCache.TryGetValue("AssignedWorkers", out var assignedWorkers))
            {
                assignedWorkers.text = $"Workers: {amount}";
            }
            else
            {
                Debug.LogWarning("Happiness label not found in cache!");
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
        
        public bool IsPointerOverUI(Vector2 screenPosition)
        {
            IPanel panel = uiDocument.rootVisualElement.panel;
            Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(panel, screenPosition);
            VisualElement pickedElement = panel.Pick(panelPosition);
            return pickedElement != null && pickedElement != uiDocument.rootVisualElement;
        }
    }
}