using Core;
using Data.Dialogs;
using UnityEngine;
using Utils;

namespace Data.Events
{
    public abstract class GameEvent : ScriptableObject
    {
        public abstract bool IsThresholdEvent { get; }
        
        public StatType thresholdStat; 
        public float thresholdValue;  
        public bool triggerOnLower;

        public int basePenalty;
        public abstract bool GetsWorsOverTime { get; }
        public float interval;
        public int intervalPenalty;

        [Range(0, 100)]
        public float randomChance;

        [HideInInspector] public bool isActive;

        public abstract void Execute(RegionController region);
        
            }
}