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
        
        [Tooltip("Has to be greater than 1 to increase the chance of random events")]
        public float randomEventModifier;
        [Tooltip("Has to be greater than 1 to increase the chance of random events")]
        public float productionModifier;

        public float randomEventCheckInterval = 5f;

        private int Happiness { get; set; }
        private int Production { get; set; }
        private int Food { get; set; }

        public List<GameEvent> events;

        private readonly Dictionary<GameEvent, int> _activePenalties = new();
        private readonly Dictionary<GameEvent, Coroutine> _activeTimers = new();
        private readonly Dictionary<GameEvent, int> _eventResolvers = new();

        public event Action<int> OnEventWorsened;
        public event Action OnEventResolved;

        private float _randomCheckTimer;

        void Awake()
        {
            Happiness = baseHappiness;
            Production = (int)(baseProduction * productionModifier);
            Food = baseFood;
        }

        public void FixedUpdate()
        {
            CheckThresholdEvent();
            CalculateProduction();
            
            _randomCheckTimer += Time.deltaTime;
            if (_randomCheckTimer >= randomEventCheckInterval)
            {
                CheckRandomEvent();
                _randomCheckTimer = 0;
            }
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
                if (_activePenalties.ContainsKey(evt)) continue;

                float roll = Random.Range(0f, 100f);

                if (roll <= evt.randomChance * randomEventModifier)
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
                    OnEventWorsened?.Invoke(_activePenalties[evt]);
                }
            }
        }

        public void TrySolveEvent(Resources resource, int amountToAdd)
        {
            foreach (var evt in _activePenalties.Keys)
            {
                if (evt.resourcesToResolve.Contains(resource))
                {
                    PayForEvent(evt, amountToAdd, resource);
                    return;
                }
            }
        }

        void PayForEvent(GameEvent evt, int amount, Resources resource)
        {
            ResourceManager.Instance.AddResource(resource, -amount);

            _eventResolvers.TryAdd(evt, 0);
            _eventResolvers[evt] += amount;

            if (_eventResolvers[evt] >= evt.resourcesNeeded)
            {
                ResolveEvent(evt);
            }
        }

        void ResolveEvent(GameEvent evt)
        {
            if (_activePenalties.ContainsKey(evt))
            {
                StopCoroutine(_activeTimers[evt]);
                _activeTimers.Remove(evt);
                _activePenalties.Remove(evt);
                _eventResolvers.Remove(evt);
                OnEventResolved?.Invoke();
            }
        }

        void CalculateProduction()
        {
            foreach (float penalty in _activePenalties.Values)
            {
                Math.Clamp(Production - penalty, 0, int.MaxValue);
            }
        }

        float GetStatValue(StatType type)
        {
            switch (type)
            {
                case StatType.Happiness: return Happiness;
                case StatType.Food: return Food;
                case StatType.Production: return Production;
                default: return 0;
            }
        }

        float CalculateFoodConsumption()
        {
            foreach (var res in ResourceManager.Instance.GetResourcesAmount())
            {
                if (res.Key.resourceName == "Workers")
                {
                    return res.Value;
                }
            }
            return 0;
        }
    }
}