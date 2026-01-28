using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI.Controllers
{
    public class EnvelopeManager : MonoBehaviour
    {
        public static EnvelopeManager Instance;

        [Header("UI Documents")]
        [SerializeField] private UIDocument envelopeDocument;

        [Header("Animation Settings")]
        [SerializeField] private Sprite[] envelopeAnimationFrames;

        [Header("Scene Settings")]
        [SerializeField] private string declineSceneName = "DeclineScene";

        private VisualElement _envelopeContainer;
        private VisualElement _envelopeImage;
        private Button _acceptButton;
        private Button _declineButton;

        public event Action OnQuestAccepted;
        public event Action OnQuestDeclined;

        private int _currentFrame;
        private bool _isPlayingAnimation;
        private bool _isOnLastFrame;

        void Awake()
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
            if (envelopeAnimationFrames != null && frameIndex < envelopeAnimationFrames.Length && _envelopeImage != null)
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
        }

        private void OnAcceptClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            HideEnvelope();
            OnQuestAccepted?.Invoke();
            MainMenuBtnController.Instance?.ContinueGameStart();
        }

        private void OnDeclineClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            HideEnvelope();
            OnQuestDeclined?.Invoke();
            SceneManager.LoadScene(declineSceneName);
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
