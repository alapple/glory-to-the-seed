using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CreditController : MonoBehaviour
{
    private InputAction backAction;

    private void Awake()
    {
        backAction = new InputAction(
            type: InputActionType.Button,
            binding: "<Keyboard>/escape"
        );

        backAction.performed += _ =>
        {
            SceneManager.LoadScene("game");
        };
    }

    private void OnEnable()
    {
        backAction.Enable();
    }

    private void OnDisable()
    {
        backAction.Disable();
    }
}

