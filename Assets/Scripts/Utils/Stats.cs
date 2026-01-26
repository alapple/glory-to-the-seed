using UnityEngine;

namespace Utils
{
    public class Stats
    {
        public int Happiness { get; set;}

        public int Production { get; set; }
        
        public int Food { get; set; }
        
        private int HappinessThreshold { get; set; }
        
        private int FoodThreshold { get; set; }
        
        public bool IsStarving => Food <= FoodThreshold;
        
        public bool IsUnHappy => Happiness <= HappinessThreshold;
    }
}