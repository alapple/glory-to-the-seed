using Core;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace UI.Controllers
{
    public class ClockController : MonoBehaviour
    {
        [SerializeField] private UIDocument clockDocument;

        private MonthConverter _monthConverter;
        private string _date;
        private Label _clock;

        void Awake()
        {
            _clock = clockDocument.rootVisualElement.Q<Label>("Time");
            Debug.Log(_clock.text);
            _monthConverter = new MonthConverter();
        }

        void Start()
        {
            CalcDate(TimeManager.Instance.startYear, 1);
            TimeManager.Instance.OnMonthChanged += CalcDate;
        }

        private void CalcDate(int year, int month)
        {
            _date = _monthConverter.DateConverter(year, month);
            DisplayDate(_date);
        }

        private void DisplayDate(string date)
        {
            _clock.text = date;
        }
    }
}