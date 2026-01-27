using System;
using UnityEngine;

namespace Core
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance;

        [Tooltip("How many seconds one month takes")]
        public float secondsPerMonth = 15f;

        public int startYear = 1965;

        private int CurrentYear { get; set; }
        private int CurrentMonth { get; set; } = 1;

        private float _timer;
        public bool isPaused;

        public event Action<int, int> OnMonthChanged;
        public event Action<int> OnYearChanged;

        void Awake()
        {
            Instance = this;
            CurrentYear = startYear;
        }


        void FixedUpdate()
        {
            if (isPaused) return;

            _timer += Time.deltaTime;

            if (_timer >= secondsPerMonth)
            {
                AdvanceMonth();
            }
        }

        private void AdvanceMonth()
        {
            _timer = 0f;
            CurrentMonth++;

            if (CurrentMonth > 12)
            {
                CurrentMonth = 1;
                CurrentYear++;
                OnYearChanged?.Invoke(CurrentYear);
            }

            OnMonthChanged?.Invoke(CurrentYear, CurrentMonth);
        }
    }
}