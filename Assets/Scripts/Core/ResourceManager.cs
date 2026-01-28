using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Resource = Data.Resources.Resources;

namespace Core
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance;
        public List<Resource> resources = new();

        private readonly Dictionary<Resource, int> _resourcesAmount = new();

        public event Action<Resource, int> OnResourceChanged;

        void Awake()
        {
            GenerateRandomResources();
            Instance = this;
        }

        void Start()
        {
            TimeManager.Instance.OnYearChanged += AddRandomResources;
            GetResourcesAmount();
        }

        private void GenerateRandomResources()
        {
            foreach (Resource res in resources)
            {
                _resourcesAmount.Add(res, Random.Range(res.minValue, res.maxValue));
            }
        }

        private void AddRandomResources(int year)
        {
            foreach (var resource in new List<Resource>(_resourcesAmount.Keys))
            {
                if (resource.resourceName == "Potato") continue;
                AddResource(resource, Random.Range(resource.minValue, resource.maxValue));
            }
        }

        public void AddResource(Resource resource, int amount)
        {
            int current = _resourcesAmount.ContainsKey(resource) ? _resourcesAmount[resource] : 0;
            _resourcesAmount[resource] = Math.Clamp(current + amount, 0, int.MaxValue);
            OnResourceChanged?.Invoke(resource, _resourcesAmount[resource]);
        }

        public void AddPotatoes(int amount)
        {
            foreach (var resource in _resourcesAmount.Keys)
            {
                if (resource.resourceName == "Potato")
                {
                    AddResource(resource, amount);
                    return;
                }
            }
        }

        public bool ConsumePotatoes(int amount)
        {
            foreach (var resource in _resourcesAmount.Keys)
            {
                if (resource.resourceName == "Potato")
                {
                    return TryConsumeResource(resource, amount);
                }
            }
            return false;
        }
        
        public Dictionary<Resource, int> GetResourcesAmount()
        {
            return _resourcesAmount;
        }
        
        public bool TryConsumeResource(Resource resource, int amount)
        {
            if (_resourcesAmount.ContainsKey(resource) && _resourcesAmount[resource] >= amount)
            {
                _resourcesAmount[resource] -= amount;
                OnResourceChanged?.Invoke(resource, _resourcesAmount[resource]);
                return true;
            }
            return false;
        }
        
        public Resource GetResourceByName(string resourceName)
        {
            return resources.Find(r => r.resourceName == resourceName);
        }

        private Resource GetWorkerResource()
        {
            return GetResourceByName("Workers");
        }

        public bool TryAssignWorkerToRegion(RegionController region, int amount)
        {
            Resource workerRes = GetWorkerResource();
        
            if (TryConsumeResource(workerRes, amount))
            {
                region.assignedWorkers += amount;
                return true;
            }
            return false;
        }
        
        public void RemoveWorkerFromRegion(RegionController region, int amount)
        {
            Resource workerRes = GetWorkerResource();

            if (region.assignedWorkers >= amount)
            {
                region.assignedWorkers -= amount;
                AddResource(workerRes, amount);
            }
        }
    }
}