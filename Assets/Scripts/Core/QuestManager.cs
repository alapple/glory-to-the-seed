using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Resources = Data.Resources.Resources;

namespace Core
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance;

        public int minValue;

        public int maxValue;

        private int PotatoGoal { get; set; }

        public event Action<bool> OnQuestCompleted;

        void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            TimeManager.Instance.OnGameOver += () =>
            {
                Debug.Log("Game Over!");
                foreach (var res in ResourceManager.Instance.GetResourcesAmount())
                {
                    if (res.Key.resourceName.Equals("Potato"))
                    {
                        Debug.Log($"Game Over! Potatoes: {res.Value}, Goal: {PotatoGoal}");
                        if (res.Value >= PotatoGoal)
                        {
                            OnQuestCompleted?.Invoke(true);
                            Debug.Log("Quest completed!");
                        }
                        else
                        {
                            OnQuestCompleted?.Invoke(false);
                            Debug.Log("Quest failed!");
                        }

                        return;
                    }
                }
                Debug.LogWarning("QuestManager: Potato resource not found!");
            };
            PotatoGoal = Random.Range(minValue, maxValue);
        }
    }
}