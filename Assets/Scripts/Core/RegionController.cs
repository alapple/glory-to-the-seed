using System;
using System.Collections;
using System.Collections.Generic;
using Data.Events;
using Data.Region;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;
using Resources = Data.Resources.Resources;

namespace Core
{
    public class RegionController : MonoBehaviour
    {
        public Region region;

        public float randomEventCheckInterval = 5f;

        public int Happiness { get; set; }
        public int Production { get; set; }

        public List<GameEvent> events;

        public int starvingModifier;
        public int eatingModifier;

        private readonly Dictionary<GameEvent, int> _activePenalties = new();
        private readonly Dictionary<GameEvent, Coroutine> _activeTimers = new();
        private readonly Dictionary<GameEvent, int> _eventResolvers = new();

        public event Action<GameEvent, int> OnEventWorsened;
        public event Action OnEventResolved;
        public event Action<GameEvent, string> OnEventAppear;

        private float _randomCheckTimer;

        void Awake()
        {
            Happiness = region.baseHappiness;
            Production = (int)(region.baseProduction * region.productionModifier);
        }

        void Start()
        {
            TimeManager.Instance.OnStatsChange += () =>
            {
                AddPotatoes(Production);
                CalculateHappiness();
            };
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

                if (roll <= evt.randomChance * region.randomEventModifier)
                {
                    AddEvent(evt);
                }
            }
        }

        private void AddEvent(GameEvent evt)
        {
            if (_activePenalties.ContainsKey(evt)) return;
            OnEventAppear?.Invoke(evt, region.regionName);
            _activePenalties.Add(evt, evt.basePenalty);
            if (evt.GetsWorsOverTime)
            {
                Coroutine timer = StartCoroutine(WorsenRoutine(evt));
                _activeTimers.Add(evt, timer);
            }
        }

        private IEnumerator WorsenRoutine(GameEvent evt)
        {
            while (true)
            {
                yield return new WaitForSeconds(evt.interval);

                if (_activePenalties.ContainsKey(evt))
                {
                    _activePenalties[evt] += evt.intervalPenalty;
                    OnEventWorsened?.Invoke(evt, _activePenalties[evt]);
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

        private void PayForEvent(GameEvent evt, int amount, Resources resource)
        {
            ResourceManager.Instance.AddResource(resource, -amount);

            _eventResolvers.TryAdd(evt, 0);
            _eventResolvers[evt] += amount;

            if (_eventResolvers[evt] >= evt.resourcesNeeded)
            {
                ResolveEvent(evt);
            }
        }

        private void ResolveEvent(GameEvent evt)
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

        private void CalculateProduction()
        {
            int workerCount = CalculateTotalWorkers();
            float totalPenalty = 0;
            foreach (float penalty in _activePenalties.Values)
            {
                totalPenalty += penalty;
            }

            Production =
                (int)Math.Clamp((workerCount * region.baseProduction) * (0.5f + Happiness / 100f) * (1 - totalPenalty),
                    0, int.MaxValue);
        }

        private void AddPotatoes(int amount)
        {
            ResourceManager.Instance.AddPotatoes(amount);
        }

        private float GetStatValue(StatType type)
        {
            switch (type)
            {
                case StatType.Happiness: return Happiness;
                case StatType.Production: return Production;
                default: return 0;
            }
        }

        private void CalculateHappiness()
        {
            bool canEat = ResourceManager.Instance.ConsumePotatoes(-CalculateTotalWorkers());

            if (canEat)
            {
                Happiness += eatingModifier;
            }
            else
            {
                Happiness -= starvingModifier;
            }
        }

        public void AllocateResources(Resources resource)
        {
            switch (resource.statType)
            {
                case StatType.Happiness: Happiness += resource.statModifier; break;
                case StatType.Production: Production += resource.statModifier; break;
            }
        }

        private int CalculateTotalWorkers()
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