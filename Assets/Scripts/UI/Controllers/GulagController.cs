using UnityEngine;
using UnityEngine.SceneManagement;

public class GulagController : MonoBehaviour
{
    [SerializeField] private float delayBeforeReturn = 5f;

    void Start()
    {
        StartCoroutine(ReturnToGameAfterDelay());
    }

    private System.Collections.IEnumerator ReturnToGameAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeReturn);
        
        // Reset the game scene by loading it fresh
        SceneManager.LoadScene("game");
    }
}
