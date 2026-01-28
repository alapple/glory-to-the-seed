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

        void Start()
        {
            _field = GetComponent<PolygonCollider2D>();
        }

        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 mousePos = Mouse.current.position.ReadValue();

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Vector2 mousePos2D = new Vector2(worldPos.x, worldPos.y);

                if (ResourceUIManager.Instance.IsPointerOverUI(mousePos))
                {
                    return; 
                }
                
                if (_field.OverlapPoint(mousePos2D))
                {
                    ResourceUIManager.Instance.ShowResourceAllocator();
                }
            }
            ResourceUIManager.Instance.UpdatePotatoPerSecond(regionController.production);
        }
    }
}