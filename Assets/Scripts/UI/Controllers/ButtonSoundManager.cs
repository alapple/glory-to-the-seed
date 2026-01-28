using UnityEngine;

namespace UI.Controllers
{
    public class ButtonSoundManager : MonoBehaviour
    {
        public static ButtonSoundManager Instance;

        [Header("Button Sound")]
        [SerializeField] private AudioClip buttonClickSound;
        
        private AudioSource _audioSource;

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
                return;
            }

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        public void PlayButtonClick()
        {
            if (buttonClickSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(buttonClickSound);
            }
        }
    }
}
