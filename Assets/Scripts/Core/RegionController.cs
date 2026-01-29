using System;
using System.Collections;
using System.Collections.Generic;
using Data.Dialogs;
using Data.Events;
using Data.Region;
using UnityEngine;
using Utils;
using Resource = Data.Resources.Resources;
using Random = UnityEngine.Random;

namespace Core
{
    public class RegionController : MonoBehaviour
    {
        public Region region;

        public float randomEventCheckInterval = 5f;

        public int happiness;
        public int production;

        public List<GameEvent> events;

        public int  starvingModifier;
        public int eatingModifier;

        [Header("Population")]
        public int assignedWorkers;
        
        private readonly Dictionary<GameEvent, int> _activePenalties = new();
        private readonly Dictionary<GameEvent, Coroutine> _activeTimers = new();
        private readonly Dictionary<GameEvent, int> _eventResolvers = new();

        public event Action<GameEvent, int> OnEventWorsened;
        public event Action OnEventResolved;
        public event Action<GameEvent, string> OnEventAppear;
        public static event Action<Dialog, string> OnDialogTriggered;

        private float _randomCheckTimer;

        void Awake()
        {
            happiness = region.baseHappiness;
            production = (int)(region.baseProduction * region.productionModifier);
        }

        void Start()
        {
            TimeManager.Instance.OnStatsChange += () =>
            {
                AddPotatoes(production);
                CalculateHappiness();
            };
            
            ResourceManager.Instance.TryAssignWorkerToRegion(this, 5);
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

        public void HandleResourceDrop(Resource resource, int amount)
        {
            if (resource.resourceName == "Workers")
            {
                ResourceManager.Instance.TryAssignWorkerToRegion(this, amount);
                return;
            }

            if (DoesEventNeedResource(resource))
            {
                if (ResourceManager.Instance.TryConsumeResource(resource, amount))
                {
                    TrySolveEvent(resource, amount);
                }
                return;
            }

            if (ResourceManager.Instance.TryConsumeResource(resource, amount))
            {
                AllocateResources(resource);
            }
        }

        private bool DoesEventNeedResource(Resource resource)
        {
            foreach (var evt in _activePenalties.Keys)
            {
                if (evt.resourcesToResolve.Contains(resource)) return true;
            }
            return false;
        }

        private void CheckThresholdEvent()
        {
            foreach (var evt in events)
            {
                if (!evt.isThresholdEvent) continue;
                if (_activePenalties.ContainsKey(evt)) continue;

                bool conditionMet;
                float currentVal = GetStatValue(evt.thresholdStat);

                if (evt.triggerOnLower) conditionMet = currentVal < evt.thresholdValue;
                else conditionMet = currentVal > evt.thresholdValue;

                if (conditionMet) AddEvent(evt);
            }
        }

        private void CheckRandomEvent()
        {
            foreach (var evt in events)
            {
                if (evt.isThresholdEvent) continue;
                if (_activePenalties.ContainsKey(evt)) continue;

                float roll = Random.Range(0f, 100f);
                if (roll <= evt.randomChance * region.randomEventModifier) AddEvent(evt);
            }
        }

        private void AddEvent(GameEvent evt)
        {
            if (_activePenalties.ContainsKey(evt)) return;
    
            OnEventAppear?.Invoke(evt, region.regionName);
            _activePenalties.Add(evt, evt.basePenalty);

            if (evt.dialog != null)
            {
                OnDialogTriggered?.Invoke(evt.dialog, region.regionName);
                Debug.Log($"Dialog triggered for region {region.regionName}");
            }

            if (evt.getsWorsOverTime)
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
                else yield break;
            }
        }

        private void TrySolveEvent(Resource resource, int amountToAdd)
        {
            foreach (var evt in _activePenalties.Keys)
            {
                if (evt.resourcesToResolve.Contains(resource))
                {
                    PayForEvent(evt, amountToAdd);
                    return;
                }
            }
        }

        private void PayForEvent(GameEvent evt, int amount)
        {
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
                if(_activeTimers.ContainsKey(evt))
                {
                    StopCoroutine(_activeTimers[evt]);
                    _activeTimers.Remove(evt);
                }
                _activePenalties.Remove(evt);
                _eventResolvers.Remove(evt);
                OnEventResolved?.Invoke();
            }
        }

        private void CalculateProduction()
        {
            float totalPenalty = 0;
            foreach (float penalty in _activePenalties.Values)
            {
                totalPenalty += penalty;
            }

            production = (int)Math.Clamp((assignedWorkers * region.baseProduction) * (0.5f + happiness / 100f) - totalPenalty, 0, int.MaxValue);
        }

        private static void AddPotatoes(int amount)
        {
            ResourceManager.Instance.AddPotatoes(amount);
        }

        private float GetStatValue(StatType type)
        {
            switch (type)
            {
                case StatType.Happiness: return happiness;
                case StatType.Production: return production;
                default: return 0;
            }
        }

        private void CalculateHappiness()
        {
            // Workers no longer automatically consume food
            // They must be fed manually using the GivePotato button
            // Happiness decreases over time if not fed
            int oldHappiness = happiness;
            happiness -= starvingModifier;
            happiness = Math.Clamp(happiness, 0, 100);
            
            Debug.Log($"{region.regionName} - Happiness: {oldHappiness} -> {happiness} (modifier: -{starvingModifier})");
            
            // Workers die if happiness reaches 0
            if (happiness <= 0 && assignedWorkers > 0)
            {
                int workersToDie = Math.Max(1, assignedWorkers / 10); // 10% of workers die
                assignedWorkers = Math.Max(0, assignedWorkers - workersToDie);
                Debug.LogWarning($"{region.regionName} - {workersToDie} workers died from starvation! Remaining: {assignedWorkers}");
            }
        }

        public void FeedWorkers(int potatoAmount)
        {
            // Called when player uses GivePotato button
            // Increase happiness when workers are fed
            happiness += eatingModifier * potatoAmount;
            happiness = Math.Clamp(happiness, 0, 100);
        }

        public void AllocateResources(Resource resource)
        {
            // Special handling for Potato - feeds workers
            if (resource.resourceName == "Potato")
            {
                FeedWorkers(1);
                return;
            }

            // Vodka is 10x more effective than potatoes at solving hunger
            if (resource.resourceName == "Vodka")
            {
                FeedWorkers(10);
                return;
            }

            switch (resource.statType)
            {
                case StatType.Happiness: 
                    happiness = Math.Clamp(happiness + resource.statModifier, 0, 100); 
                    break;
                case StatType.Production: 
                    production = Math.Clamp(production + resource.statModifier, 0, int.MaxValue); 
                    break;
            }
        }
    }
}