using Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Controllers
{
    public class GameResultLoader : MonoBehaviour
    {
        [Header("Scene Names")] [SerializeField]
        private string winSceneName = "VictoryScene";

        [SerializeField] private string loseSceneName = "GulagScene";

        [Header("Settings")] [SerializeField] private float delayBeforeLoad = 2.0f;

        void Start()
        {
            QuestManager.Instance.OnQuestCompleted += OnLevelFinished;
        }

        void OnDestroy()
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnQuestCompleted -= OnLevelFinished;
            }
        }

        private void OnLevelFinished(bool isSuccess)
        {
            if (isSuccess)
            {
                Debug.Log("Win");
                //StartCoroutine(LoadSceneRoutine(winSceneName));
                SceneManager.LoadScene(winSceneName);
            }
            else
            {
                Debug.Log("Loose");
                //StartCoroutine(LoadSceneRoutine(loseSceneName));
                SceneManager.LoadScene(loseSceneName);
            }
        }

        private System.Collections.IEnumerator LoadSceneRoutine(string sceneName)
        {
            yield return new WaitForSeconds(delayBeforeLoad);
            SceneManager.LoadScene(sceneName);
        }
    }
}