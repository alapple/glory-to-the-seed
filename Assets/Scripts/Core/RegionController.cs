using System;
using System.Collections;
using System.Collections.Generic;
using Data.Events;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;
using Resources = Data.Resources.Resources;

namespace Core
{
    public class RegionController : MonoBehaviour
    {
        public int baseHappiness;
        public int baseProduction;
        public int baseFood;
        
        private int _happiness;
        private int _production;
        private int _food;
        
        public List<GameEvent> events;
        
        private readonly Dictionary<GameEvent, int> _activePenalties = new();
        private readonly Dictionary<GameEvent, Coroutine> _activeTimers = new();
        private readonly Dictionary<GameEvent, int> _eventResolvers = new();

        void Awake()
        {
            _happiness = baseHappiness;
            _production = baseProduction;
            _food = baseFood;
        }

        public void FixedUpdate()
        {
            CheckThresholdEvent();
            CheckRandomEvent();
            CalculateProduction();
        }

        private void CheckThresholdEvent()
        {
            foreach (var evt in events)
            {
                if (!evt.IsThresholdEvent) continue;
                if (_activePenalties.ContainsKey(evt)) continue;

                bool conditionMet;

                float currentVal = GetStatValue(evt.thresholdStat);

                if (evt.triggerOnLower) conditionMet = currentVal < evt.thresholdValue;
                else conditionMet = currentVal > evt.thresholdValue;

                if (conditionMet)
                {
                    AddEvent(evt);
                }
            }
        }

        private void CheckRandomEvent()
        {
            foreach (var evt in events)
            {
                if (evt.IsThresholdEvent) continue;
                if (evt.isActive) continue;

                float roll = Random.Range(0f, 100f);

                if (roll <= evt.randomChance)
                {
                    AddEvent(evt);
                }
            }
        }


        void AddEvent(GameEvent evt)
        {
            if (_activePenalties.ContainsKey(evt)) return;
            
            _activePenalties.Add(evt, evt.basePenalty);
            if (evt.GetsWorsOverTime)
            {
                Coroutine timer = StartCoroutine(WorsenRoutine(evt));
                _activeTimers.Add(evt, timer);
            }
        }

        IEnumerator WorsenRoutine(GameEvent evt)
        {
            while (true)
            {
                yield return new WaitForSeconds(evt.interval);

                if (_activePenalties.ContainsKey(evt))
                {
                    _activePenalties[evt] += evt.intervalPenalty;
                }
            }
        }

        public void AllocateResources(GameEvent evt, int amount, Resources resource)
        {
            if (_eventResolvers.ContainsKey(evt))
            {
                foreach (Resources res in evt.resourceToResolve)
                {
                    if (res == resource)
                    {
                        _eventResolvers[evt] += amount;
                        if (_eventResolvers[evt] >= evt.resourceToResolve.Count) ResolveEvent(evt);
                    }
                }
            }
        }

        void ResolveEvent(GameEvent evt)
        {
            if (_activePenalties.ContainsKey(evt))
            {
                StopCoroutine(_activeTimers[evt]);
                _activeTimers.Remove(evt);
            }

            _activePenalties.Remove(evt);
        }

        void CalculateProduction()
        {
            foreach (float penalty in _activePenalties.Values)
            {
                Math.Clamp(_production - penalty, 0, int.MaxValue);
            }
        }

        float GetStatValue(StatType type)
        {
            switch (type)
            {
                case StatType.Happiness: return _happiness;
                case StatType.Food: return _food;
                case StatType.Production: return _production;
                default: return 0;
            }
        }
    }
}