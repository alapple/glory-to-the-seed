using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI.Controllers
{
    public class MainMenuBtnController : MonoBehaviour
    {
        private UIDocument _document;
        private List<Button> _buttons = new List<Button>();

        private static readonly int StartButtonIndex = 0;
        private static readonly int CreditsButtonIndex = 1;
        private static readonly int QuitButtonIndex = 2;
        private Scene newScene;

        void Awake()
        {
            _document = GetComponent<UIDocument>();

            _buttons = _document.rootVisualElement.Query<Button>().ToList();

            foreach (var button in _buttons)
            {
                button.RegisterCallback<ClickEvent>(OnButtonClick);
            }

            SceneManager.LoadSceneAsync("Scenes/Game", LoadSceneMode.Additive);
            newScene = SceneManager.GetSceneByName("Scenes/Game");

            foreach (var root in newScene.GetRootGameObjects())
            {
                UIDocument uiDoc = root.GetComponentInChildren<UIDocument>();
                if (uiDoc != null)
                {
                    uiDoc.rootVisualElement.style.display = DisplayStyle.None;
                }
            }
        }

        private void OnButtonClick(ClickEvent evt)
        {
            if (evt.target.Equals(_buttons[StartButtonIndex]))
            {
                SceneManager.UnloadSceneAsync("Main Menu");
            }
            else if (evt.target.Equals(_buttons[CreditsButtonIndex]))
            {
                SceneManager.LoadScene("Scenes/Credits");
            }
            else if (evt.target.Equals(_buttons[QuitButtonIndex]))
            {
            }
        }
    }
}