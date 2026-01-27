using UnityEngine;
using Utils;

namespace Data.Resources
{
    [CreateAssetMenu(menuName = "Resources")]

    public class Resources : ScriptableObject
    {
        public string resourceName;
        
        public Sprite sprite;

        public int minValue;
        
        public int maxValue;
        
        public StatType statModifier;
    }
}