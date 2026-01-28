using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core
{
    /// <summary>
    /// Handles the end game logic - loads the appropriate scene based on whether the player met the potato quota
    /// </summary>
    public class GameOverManager : MonoBehaviour
    {
        public static GameOverManager Instance;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            QuestManager.Instance.OnQuestCompleted += HandleQuestCompletion;
        }
        
        private void HandleQuestCompletion(bool quotaMet)
        {
            if (quotaMet)
            {
                SceneManager.LoadScene("VictoryScene");
            }
            else
            {
                SceneManager.LoadScene("GulagScene");
            }
        }
    }
}
