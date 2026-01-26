using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Resource = Data.Resources.Resources;

namespace Core
{
    public class ResourceManager : MonoBehaviour
    {
        public List<Resource> resources = new();

        private readonly Dictionary<Resource, int> _resourcesAmount = new();

        public event Action<Resource, int> OnResourceChanged;

        void Awake()
        {
            GenerateRandomResources();
        }

        void Start()
        {
            TimeManager.Instance.OnYearChanged += AddRandomResources;
        }

        private void GenerateRandomResources()
        {
            foreach (Resource res in resources)
            {
                _resourcesAmount.Add(res, Random.Range(res.minValue, res.maxValue));
                Debug.Log($"Added {res} with amount {_resourcesAmount[res]}");
            }
        }

        private void AddRandomResources(int year)
        {
            foreach (var resource in new List<Resource>(_resourcesAmount.Keys))
            {
                _resourcesAmount[resource] += Random.Range(resource.minValue, resource.maxValue);
                OnResourceChanged?.Invoke(resource, _resourcesAmount[resource]);
            }
        }

        public void AddResource(Resource resource, int amount)
        {
            _resourcesAmount[resource] += amount;
            OnResourceChanged?.Invoke(resource, _resourcesAmount[resource]);
        }

        public void RemoveResourceU(Resource resource, int amount)
        {
            _resourcesAmount[resource] -= amount;
            OnResourceChanged?.Invoke(resource, _resourcesAmount[resource]);
        }
    }
}