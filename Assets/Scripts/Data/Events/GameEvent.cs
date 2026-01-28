using System.Collections.Generic;
using Data.Dialogs;
using UnityEngine;
using Utils;

namespace Data.Events
{
    [CreateAssetMenu(menuName = "Event")]
    public class GameEvent : ScriptableObject
    {
        public bool isThresholdEvent;

        public StatType thresholdStat;
        public float thresholdValue;
        public bool triggerOnLower;

        public int basePenalty;
        public bool getsWorsOverTime;
        public float interval;
        public int intervalPenalty;

        public List<Resources.Resources> resourcesToResolve;
        public int resourcesNeeded;

        public Dialog dialog;

        [Range(0, 100)] public float randomChance; 
    }
}