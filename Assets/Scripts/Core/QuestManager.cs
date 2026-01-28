using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Resources = Data.Resources.Resources;

namespace Core
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField]
        private ResourceManager resourceManager;
        public static QuestManager Instance;

        public int minValue;

        public int maxValue;
        
        private int PotatoGoal { get; set;}
        
        public event Action<bool> OnQuestCompleted;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            TimeManager.Instance.OnGameOver += () =>
            {
                foreach (var res in resourceManager.GetResourcesAmount())
                {
                    if (res.Key.resourceName.Equals("Potatoes"))
                    {
                        if (res.Value >= PotatoGoal)
                        {
                            OnQuestCompleted?.Invoke(true);
                        }
                        else
                        {
                            OnQuestCompleted?.Invoke(false);
                        }
                    } 
                }
            };
            PotatoGoal = Random.Range(minValue, maxValue);;
        }
    }
}