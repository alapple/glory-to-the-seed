using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace UI.Controllers
{
    public class MainMenuBtnController : MonoBehaviour
    {
        public static MainMenuBtnController Instance;

        [FormerlySerializedAs("_mainMenuDocument")] [SerializeField]
        private UIDocument mainMenuDocument;

        [FormerlySerializedAs("_gameUIDocument")] [SerializeField]
        private UIDocument gameUIDocument;

        private UIDocument _document;

        public event Action OnGameStart;

        void Awake()
        {
            Instance = this;
            var startBtn = mainMenuDocument.rootVisualElement.Q<Button>("StartButton");
            var creditsBtn = mainMenuDocument.rootVisualElement.Q<Button>("CreditButton");
            var quitBtn = mainMenuDocument.rootVisualElement.Q<Button>("QuitButton");

            startBtn.RegisterCallback<ClickEvent>(OnStartButtonClick);
            creditsBtn.RegisterCallback<ClickEvent>(OnCreditButtonclick);
            quitBtn.RegisterCallback<ClickEvent>(OnQuitButtonClick);

            SetupDisplay();
        }

        private void SetupDisplay()
        {
            gameUIDocument.rootVisualElement.style.display = DisplayStyle.None;
        }

        private void OnQuitButtonClick(ClickEvent evt)
        {
            Application.Quit();
        }

        private void OnCreditButtonclick(ClickEvent evt)
        {
            SceneManager.LoadScene("Credits");
        }

        private void OnStartButtonClick(ClickEvent evt)
        {
            mainMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            gameUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            OnGameStart?.Invoke();
        }
    }
}