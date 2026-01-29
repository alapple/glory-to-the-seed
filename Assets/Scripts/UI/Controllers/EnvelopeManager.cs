using System;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.Controllers
{
    public class EnvelopeManager : MonoBehaviour
    {
        public static EnvelopeManager Instance;

        [Header("UI Documents")] [SerializeField]
        private UIDocument envelopeDocument;

        [Header("Animation Settings")] [SerializeField]
        private Sprite[] envelopeAnimationFrames;

        [Header("Scene Settings")] [SerializeField]
        private SceneAsset declineScene;

        [SerializeField, HideInInspector] private string declineSceneName;

        private void OnValidate()
        {
            if (declineScene != null)
            {
                declineSceneName = declineScene.name;
            }
        }

        private VisualElement _envelopeContainer;
        private VisualElement _envelopeImage;
        private Button _acceptButton;
        private Button _declineButton;
        private Label _potatoLabel;

        public event Action OnQuestAccepted;
        public event Action OnQuestDeclined;

        private int _currentFrame;
        private bool _isPlayingAnimation;
        private bool _isOnLastFrame;
        private AsyncOperation _preloadedScene;

        void Start()
        {
            Instance = this;
            SetupEnvelopeUI();
        }

        private void SetupEnvelopeUI()
        {
            if (envelopeDocument == null) return;

            var root = envelopeDocument.rootVisualElement;
            _envelopeContainer = root.Q<VisualElement>("EnvelopeContainer");
            _envelopeImage = root.Q<VisualElement>("EnvelopeImage");
            _acceptButton = root.Q<Button>("AcceptButton");
            _declineButton = root.Q<Button>("DeclineButton");
            _potatoLabel = root.Q<Label>("PotatoLabel");

            if (_envelopeContainer != null)
            {
                _envelopeContainer.style.display = DisplayStyle.None;
                _envelopeContainer.RegisterCallback<ClickEvent>(OnEnvelopeClicked);
            }

            if (_acceptButton != null)
            {
                _acceptButton.style.display = DisplayStyle.None;
                _acceptButton.RegisterCallback<ClickEvent>(OnAcceptClicked);
            }

            if (_declineButton != null)
            {
                _declineButton.style.display = DisplayStyle.None;
                _declineButton.RegisterCallback<ClickEvent>(OnDeclineClicked);
            }
        }

        public void StartEnvelopeSequence()
        {
            _currentFrame = 0;
            _isPlayingAnimation = true;
            _isOnLastFrame = false;
            
            

            if (_envelopeContainer != null)
                _envelopeContainer.style.display = DisplayStyle.Flex;

            ShowFrame(0);

            // Preload the decline scene in the background
            PreloadDeclineScene();
        }

        private void PreloadDeclineScene()
        {
            if (string.IsNullOrEmpty(declineSceneName)) return;

            _preloadedScene = SceneManager.LoadSceneAsync(declineSceneName);
            if (_preloadedScene != null)
            {
                _preloadedScene.allowSceneActivation = false;
            }
        }

        private void OnEnvelopeClicked(ClickEvent evt)
        {
            if (_isOnLastFrame || !_isPlayingAnimation) return;

            _currentFrame++;

            if (envelopeAnimationFrames != null && _currentFrame < envelopeAnimationFrames.Length)
            {
                ShowFrame(_currentFrame);

                if (_currentFrame == envelopeAnimationFrames.Length - 1)
                {
                    ShowButtons();
                }
            }
        }

        private void ShowFrame(int frameIndex)
        {
            if (envelopeAnimationFrames != null && frameIndex < envelopeAnimationFrames.Length &&
                _envelopeImage != null)
            {
                _envelopeImage.style.backgroundImage = new StyleBackground(envelopeAnimationFrames[frameIndex]);
            }
        }

        private void ShowButtons()
        {
            _isOnLastFrame = true;
            _isPlayingAnimation = false;

            if (_acceptButton != null)
                _acceptButton.style.display = DisplayStyle.Flex;

            if (_declineButton != null)
                _declineButton.style.display = DisplayStyle.Flex;
            if (_potatoLabel != null && QuestManager.Instance != null)
            {
                _potatoLabel.text = QuestManager.Instance.potatoGoal.ToString();
                Debug.Log($"Potato count: {QuestManager.Instance.potatoGoal}");
                Debug.Log(_potatoLabel.text);
            }
        }

        private void OnAcceptClicked(ClickEvent evt)
        {
            ButtonSoundManager.Instance?.PlayButtonClick();
            evt.StopPropagation();
            HideEnvelope();
            OnQuestAccepted?.Invoke();
            MainMenuBtnController.Instance?.ContinueGameStart();
        }

        private void OnDeclineClicked(ClickEvent evt)
        {
            ButtonSoundManager.Instance?.PlayButtonClick();
            evt.StopPropagation();
            HideEnvelope();
            OnQuestDeclined?.Invoke();

            if (string.IsNullOrEmpty(declineSceneName))
            {
                Debug.LogError(
                    "EnvelopeManager: Decline scene name is not set! Please assign a scene in the Inspector.");
                return;
            }

            // Use preloaded scene if available for instant switching
            if (_preloadedScene != null)
            {
                _preloadedScene.allowSceneActivation = true;
            }
            else
            {
                SceneManager.LoadScene(declineSceneName);
            }
        }

        private void HideEnvelope()
        {
            if (_envelopeContainer != null)
                _envelopeContainer.style.display = DisplayStyle.None;
        }

        void OnDestroy()
        {
            _acceptButton?.UnregisterCallback<ClickEvent>(OnAcceptClicked);
            _declineButton?.UnregisterCallback<ClickEvent>(OnDeclineClicked);
            _envelopeContainer?.UnregisterCallback<ClickEvent>(OnEnvelopeClicked);
        }
    }
}