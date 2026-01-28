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

        [Header("End Game Scenes")]
        [Tooltip("Scene to load when player meets the potato quota (Victory)")]
#if UNITY_EDITOR
        [SerializeField] private SceneAsset victoryScene;
#endif
        [SerializeField, HideInInspector] private string victorySceneName;

        [Tooltip("Scene to load when player fails to meet the potato quota (Gulag)")]
#if UNITY_EDITOR
        [SerializeField] private SceneAsset gulagScene;
#endif
        [SerializeField, HideInInspector] private string gulagSceneName;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (victoryScene != null)
            {
                victorySceneName = victoryScene.name;
            }
            if (gulagScene != null)
            {
                gulagSceneName = gulagScene.name;
            }
        }
#endif

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            // Subscribe to the QuestCompleted event
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnQuestCompleted += HandleQuestCompletion;
            }
            else
            {
                Debug.LogError("GameOverManager: QuestManager.Instance is null!");
            }
        }

        void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnQuestCompleted -= HandleQuestCompletion;
            }
        }

        /// <summary>
        /// Handles the quest completion and loads the appropriate scene
        /// </summary>
        /// <param name="quotaMet">True if the player met the quota, false otherwise</param>
        private void HandleQuestCompletion(bool quotaMet)
        {
            if (quotaMet)
            {
                Debug.Log("GameOverManager: Quota met! Loading victory scene...");
                LoadScene(victorySceneName);
            }
            else
            {
                Debug.Log("GameOverManager: Quota not met! Loading gulag scene...");
                LoadScene(gulagSceneName);
            }
        }

        /// <summary>
        /// Loads a scene by name
        /// </summary>
        /// <param name="sceneName">The name of the scene to load</param>
        private void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"GameOverManager: Scene name is empty or null!");
                return;
            }

            // Check if scene exists in build settings
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError($"GameOverManager: Scene '{sceneName}' not found in build settings! Please add it to File > Build Settings > Scenes in Build.");
            }
        }
    }
}
