using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Controllers
{
    public class SelectFieldController : MonoBehaviour
    {
        private PolygonCollider2D _field;

        void Start()
        {
            Debug.Log("Hello World!");
            _field = GetComponent<PolygonCollider2D>();
        }

        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 mousePos = Mouse.current.position.ReadValue();

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Vector2 mousePos2D = new Vector2(worldPos.x, worldPos.y);

                if (_field.OverlapPoint(mousePos2D))
                {
                }
            }
        }
    }
}