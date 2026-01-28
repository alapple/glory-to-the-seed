using UI.Controllers;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    void Start()
    {
        if (MainMenuBtnController.Instance != null)
        {
            MainMenuBtnController.Instance.OnGameStart += HideLoadingScreen;
        }
    }

    void OnDestroy()
    {
        if (MainMenuBtnController.Instance != null)
        {
            MainMenuBtnController.Instance.OnGameStart -= HideLoadingScreen;
        }
    }

    private void HideLoadingScreen()
    {
        gameObject.SetActive(false);
    }
}
