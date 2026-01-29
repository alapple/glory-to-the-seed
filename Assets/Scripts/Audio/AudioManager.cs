using Core;
using UnityEngine;
using Data.Events;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] private AudioSource audioSource;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            RegionController.OnEventAppear += PlayEventSound;
        }

        void OnDisable()
        {
            RegionController.OnEventAppear -= PlayEventSound;
        }

        private void PlayEventSound(GameEvent gameEvent, string regionName)
        {
            if (gameEvent != null && gameEvent.eventSound != null)
            {
                audioSource.PlayOneShot(gameEvent.eventSound);
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip != null) audioSource.PlayOneShot(clip);
        }
    }
}