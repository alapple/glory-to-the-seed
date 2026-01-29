using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace UI.Controllers
{
    public class SelectFieldController : MonoBehaviour
    {
        private PolygonCollider2D _field;
        [SerializeField] private RegionController regionController;
        
        private static SelectFieldController _activeField;
        private bool _isActive;

        void Start()
        {
            _field = GetComponent<PolygonCollider2D>();
        }

        void Update()
{
    if (!Mouse.current.leftButton.wasPressedThisFrame)
        return;

    Vector3 mousePos = Mouse.current.position.ReadValue();
    Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
    Vector2 mousePos2D = new Vector2(worldPos.x, worldPos.y);

    // Klick auf UI → ignorieren
    if (ResourceUIManager.Instance.IsPointerOverUI(mousePos))
        return;

    // 🔹 FALL 1: Es gibt bereits ein aktives Feld
    if (_activeField != null)
    {
        // Klick außerhalb des aktiven Feldes → schließen
        if (!_activeField._field.OverlapPoint(mousePos2D))
        {
            ResourceUIManager.Instance.HideResourceAllocator();
            _activeField._isActive = false;
            _activeField = null;
            return;
        }
    }

    // 🔹 FALL 2: Klick auf dieses Feld → öffnen
    if (_field.OverlapPoint(mousePos2D) && _activeField == null)
    {
        ResourceUIManager.Instance.ShowResourceAllocator(regionController);
        _activeField = this;
        _isActive = true;
    }
}

    }
}