using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI.Controllers
{
    public class MainMenuBtnController : MonoBehaviour
    {
        [SerializeField] private UIDocument _mainMenuDocument;

        private UIDocument _document;

        private event Action OnGameStart;

        void Awake()
        {
            var startBtn = _mainMenuDocument.rootVisualElement.Q<Button>("StartButton");
            var creditsBtn = _mainMenuDocument.rootVisualElement.Q<Button>("CreditButton");

            startBtn.RegisterCallback<ClickEvent>(OnStartButtonClick);
            creditsBtn.RegisterCallback<ClickEvent>(OnCreditButtonclick);
        }

        private void OnCreditButtonclick(ClickEvent evt)
        {
            SceneManager.LoadScene("Credits");
        }

        private void OnStartButtonClick(ClickEvent evt)
        {
            _mainMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            OnGameStart?.Invoke();
        }
    }
}