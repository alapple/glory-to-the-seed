using System;
using System.Collections;
using System.Collections.Generic;
using Data.Events;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core
{
    public class RegionController : MonoBehaviour
    {
        public Stats Stats;

        public List<GameEvent> events;

        private Dictionary<GameEvent, int> _activePenalties = new Dictionary<GameEvent, int>();

        private Dictionary<GameEvent, Coroutine> _activeTimers = new Dictionary<GameEvent, Coroutine>();

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

                if (evt.triggerOnLower)
                    conditionMet = currentVal < evt.thresholdValue;
                else
                    conditionMet = currentVal > evt.thresholdValue;

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
                Math.Clamp(Stats.Production - penalty, 0, int.MaxValue);
            }
        }

        float GetStatValue(StatType type)
        {
            switch (type)
            {
                case StatType.Happiness: return Stats.Happiness;
                case StatType.Food: return Stats.Food;
                case StatType.Production: return Stats.Production;
                default: return 0;
            }
        }
    }
}