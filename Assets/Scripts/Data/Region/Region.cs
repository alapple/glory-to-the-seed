using UnityEngine;

namespace Data.Region
{
    public class Region : ScriptableObject
    {
        public string regionName;
        
        public int baseHappiness;
        public int baseProduction;
        
        public float productionModifier;
        
        public float randomEventModifier;
    }
}