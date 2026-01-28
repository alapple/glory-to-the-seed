using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Controllers
{
    /// <summary>
    /// Updates the worker amount and production rate display in the UI
    /// </summary>
    public class WorkerStatsController : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        private Label _workerAmountLabel;
        private Label _productionRateLabel;

        void Awake()
        {
            if (uiDocument == null)
            {
                Debug.LogError("WorkerStatsController: UIDocument is not assigned!");
                return;
            }

            // Find the labels in the UI - they are inside the WorkerInfoText element
            var root = uiDocument.rootVisualElement;
            var workerInfoText = root.Q<VisualElement>("WorkerInfoText");
            
            if (workerInfoText != null)
            {
                var labels = workerInfoText.Query<Label>().ToList();
                
                // First label should be worker amount, second should be production rate
                if (labels.Count >= 2)
                {
                    _workerAmountLabel = labels[0];
                    _productionRateLabel = labels[1];
                }
                else
                {
                    Debug.LogWarning("WorkerStatsController: Could not find worker stat labels!");
                }
            }
            else
            {
                Debug.LogWarning("WorkerStatsController: WorkerInfoText element not found!");
            }
        }

        void Start()
        {
            UpdateWorkerStats();
        }

        void FixedUpdate()
        {
            UpdateWorkerStats();
        }

        private void UpdateWorkerStats()
        {
            if (_workerAmountLabel != null)
            {
                int totalWorkers = GetTotalWorkers();
                _workerAmountLabel.text = $"Workers: {totalWorkers}";
            }

            if (_productionRateLabel != null)
            {
                int totalProduction = GetTotalProduction();
                _productionRateLabel.text = $"Production: {totalProduction}/sek";
            }
        }

        private int GetTotalWorkers()
        {
            if (ResourceManager.Instance == null) return 0;

            foreach (var res in ResourceManager.Instance.GetResourcesAmount())
            {
                if (res.Key.resourceName == "Workers")
                {
                    return res.Value;
                }
            }

            return 0;
        }

        private int GetTotalProduction()
        {
            var regions = FindObjectsOfType<RegionController>();
            int totalProduction = 0;

            foreach (var region in regions)
            {
                totalProduction += region.production;
            }

            return totalProduction;
        }
    }
}
