using System;
using UI.Controllers;
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

        public int potatoGoal;

        public event Action<bool> OnQuestCompleted;
        public event Action<int> OnPotatoGenerated;

        void Awake()
        {
            Instance = this;
            
        }

        private void Start()
        {
            GenerateNewGoal();
            TimeManager.Instance.OnGameOver += () =>
            {
                Debug.Log("Game Over!");
                foreach (var res in ResourceManager.Instance.GetResourcesAmount())
                {
                    if (res.Key.resourceName.Equals("Potato"))
                    {
                        Debug.Log($"Game Over! Potatoes: {res.Value}, Goal: {potatoGoal}");
                        if (res.Value >= potatoGoal)
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
        }
        public void GenerateNewGoal()
        {
            potatoGoal = Random.Range(minValue, maxValue);
            OnPotatoGenerated?.Invoke(potatoGoal);
        }
    }
}