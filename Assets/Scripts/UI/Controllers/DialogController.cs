using UnityEngine;
using UnityEngine.UIElements;
using Data.Dialogs;
using Core;

namespace UI.Controllers
{
    public class NotificationManager : MonoBehaviour
    {
        private VisualElement _notificationContainer;
        [SerializeField]
        private UIDocument uiDocument;

        void OnEnable()
        {
            var root = uiDocument.rootVisualElement;
            _notificationContainer = root.Q<VisualElement>("NotificationArea");

            if (_notificationContainer != null)
            {
                _notificationContainer.pickingMode = PickingMode.Ignore;
            }

            RegionController.OnDialogTriggered += CreateNotification;
        }

        void OnDisable()
        {
            RegionController.OnDialogTriggered -= CreateNotification;
        }

        private void CreateNotification(Dialog dialogData, string regionName)
        {
            if (dialogData == null || _notificationContainer == null) return;

            VisualElement notificationBox = new VisualElement();
            notificationBox.AddToClassList("notification-item");
            
            Label title = new Label($"{dialogData.title} ({regionName})");
            title.AddToClassList("notification-title");
            notificationBox.Add(title);

            Label msg = new Label(dialogData.text);
            msg.AddToClassList("notification-text");
            notificationBox.Add(msg);

            Button closeBtn = new Button(() => _notificationContainer.Remove(notificationBox));
            closeBtn.text = "X";
            closeBtn.AddToClassList("notification-close-btn");
            notificationBox.Add(closeBtn);

            _notificationContainer.Add(notificationBox);
        }
    }
}