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
        
        // Field is dead when happiness reaches 0 - cannot be fed or produce
        public bool isDead;
        
        private readonly Dictionary<GameEvent, int> _activePenalties = new();
        private readonly Dictionary<GameEvent, Coroutine> _activeTimers = new();
        private readonly Dictionary<GameEvent, int> _eventResolvers = new();
        private readonly Dictionary<GameEvent, GameObject> _activeEventVisuals = new();

        public event Action<GameEvent, int> OnEventWorsened;
        public event Action OnEventResolved;
        public static event Action<GameEvent, string> OnEventAppear;
        public static event Action<Dialog, string> OnDialogTriggered;

        private float _randomCheckTimer;

        void Awake()
        {
            happiness = 100;
            production = (int)(region.baseProduction * region.productionModifier);
        }

        void Start()
        {
            TimeManager.Instance.OnStatsChange += () =>
            {
                CalculateProduction();
                AddPotatoes(production);
                CalculateHappiness();
            };
            
            ResourceManager.Instance.TryAssignWorkerToRegion(this, 5);
            CalculateProduction(); 
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
            if (isDead)
            {
                Debug.LogWarning($"{region.regionName} - Field is DEAD! Cannot receive resources.");
                return;
            }
            
            if (resource.resourceName == "Workers")
            {
                ResourceManager.Instance.TryAssignWorkerToRegion(this, amount);
                return;
            }

            if (resource.resourceName == "Potato" || resource.resourceName == "Vodka")
            {
                if (ResourceManager.Instance.TryConsumeResource(resource, amount))
                {
                    AllocateResources(resource);
                }
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

                float currentVal = GetStatValue(evt.thresholdStat);
                bool conditionMet;
                
                if (evt.triggerOnLower) conditionMet = currentVal < evt.thresholdValue;
                else conditionMet = currentVal > evt.thresholdValue;

                if (conditionMet && !_activePenalties.ContainsKey(evt))
                {
                    AddEvent(evt);
                }
                else if (!conditionMet && _activePenalties.ContainsKey(evt))
                {
                    ResolveEvent(evt);
                }
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

            if (evt.dialog is not null)
            {
                OnDialogTriggered?.Invoke(evt.dialog, region.regionName);
            }

            if (evt.visualPrefab != null)
            {
                GameObject visualInstance = Instantiate(evt.visualPrefab, transform);
                visualInstance.transform.localPosition = Vector3.zero; 
                _activeEventVisuals.Add(evt, visualInstance);
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

                if (_activeEventVisuals.TryGetValue(evt, out GameObject visualInstance))
                {
                    if (visualInstance != null) Destroy(visualInstance);
                    _activeEventVisuals.Remove(evt);
                }

                _activePenalties.Remove(evt);
                _eventResolvers.Remove(evt);
                OnEventResolved?.Invoke();
            }
        }

        private void CalculateProduction()
        {
            if (isDead)
            {
                production = 0;
                return;
            }
            
            float totalPenalty = 0;
            foreach (float penalty in _activePenalties.Values)
            {
                totalPenalty += penalty;
            }

            float baseProduction = assignedWorkers * region.baseProduction * region.productionModifier;
            float adjustedProduction = baseProduction - totalPenalty;

            if (happiness < 30)
            {
                adjustedProduction *= 0.5f;
            }
            
            production = (int)Math.Clamp(adjustedProduction, 0, int.MaxValue);
            
            Debug.Log($"{region.regionName} - Production: workers={assignedWorkers}, base={region.baseProduction}, modifier={region.productionModifier}, happiness={happiness}, penalty={totalPenalty}, happinessModifier={(happiness < 30 ? 0.5f : 1f)}, final={production}");
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
            if (isDead)
            {
                return;
            }
            
            int oldHappiness = happiness;
            happiness -= starvingModifier;
            happiness = Math.Clamp(happiness, 0, 100);
            
            
            if (happiness <= 0)
            {
                isDead = true;
                production = 0;
                Debug.LogWarning($"{region.regionName} - Field is now DEAD! Cannot be fed or produce anymore.");
            }
        }

        public void FeedWorkers(int potatoAmount)
        {
            if (isDead)
            {
                return;
            }
            
            int oldHappiness = happiness;
            happiness += eatingModifier * potatoAmount;
            happiness = Math.Clamp(happiness, 0, 100);
        }

        public void AllocateResources(Resource resource)
        {
            if (isDead)
            {
                return;
            }
            
            if (resource.resourceName == "Potato")
            {
                FeedWorkers(1);
                return;
            }

            if (resource.resourceName == "Vodka")
            {
                int oldHappiness = happiness;
                happiness += 14;
                happiness = Math.Clamp(happiness, 0, 100);
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