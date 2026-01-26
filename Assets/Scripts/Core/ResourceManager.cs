using System.Collections.Generic;
using UnityEngine;
using Resource = Data.Resources.Resources;

namespace Core
{
    public class ResourceManager : MonoBehaviour
    {
        public Dictionary<Resource, int> ResourcesAmount = new();

        void Awake()
        {
            AddRandomResource();
        }

        private void AddRandomResource()
        {
            foreach (var (key, value) in ResourcesAmount)
            {
                ResourcesAmount[key] = Random.Range(key.minValue, key.maxValue);
            }
        }
        
        public void AddResource(Resource resource, int amount)
        {
            ResourcesAmount[resource] += amount;
        }

        public void RemoveResourceU(Resource resource, int amount)
        {
            ResourcesAmount[resource] -= amount;
        }
    }
}