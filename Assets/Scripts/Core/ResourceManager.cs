using System.Collections.Generic;
using UnityEngine;
using Resource = Data.Resources.Resources;

namespace Core
{
    public class ResourceManager : MonoBehaviour
    {
        public List<Resource> resources = new();
        
        private readonly Dictionary<Resource, int> _resourcesAmount = new();

        void Awake()
        {
            AddRandomResource();
        }

        private void AddRandomResource()
        {
            foreach (Resource res in resources)
            {
                _resourcesAmount.Add(res, Random.Range(res.minValue, res.maxValue));
                Debug.Log($"Added {res} with amount {_resourcesAmount[res]}");
            }
        }
        
        public void AddResource(Resource resource, int amount)
        {
            _resourcesAmount[resource] += amount;
        }

        public void RemoveResourceU(Resource resource, int amount)
        {
            _resourcesAmount[resource] -= amount;
        }
    }
}