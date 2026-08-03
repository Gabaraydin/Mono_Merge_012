using UnityEngine;
using MonoMerge.Core;

namespace MonoMerge.Audio
{
    /// <summary>
    /// GDD 2/3: "tatmin edici tik sesleri" for tile placement and merges, plus a game-over
    /// cue. A single AudioSource is enough — the brief asks for one-shot SFX only, no music
    /// layer. Mute state is sourced from Core.SaveManager so this class never touches
    /// PlayerPrefs directly (SaveManager is the sole gateway to it).
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioClip placeClip;
        [SerializeField] private AudioClip mergeClip;
        [SerializeField] private AudioClip gameOverClip;

        private AudioSource audioSource;
        private bool isMuted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }

        // Start (not Awake/OnEnable) guarantees SaveManager.Instance and GameManager.Instance
        // already exist — see HUDController's class doc for why this ordering matters.
        private void Start()
        {
            isMuted = SaveManager.Instance != null && SaveManager.Instance.IsMuted;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
            if (Instance == this) Instance = null;
        }

        /// <summary>Called by SettingsPanel whenever the mute toggle changes.</summary>
        public void SetMuted(bool muted) => isMuted = muted;

        /// <summary>Called by DragDropController right after a successful placement.</summary>
        public void PlayPlace() => PlayOneShot(placeClip);

        /// <summary>Called by MergeManager right after a merge resolves.</summary>
        public void PlayMerge() => PlayOneShot(mergeClip);

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.GameOver) PlayOneShot(gameOverClip);
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (isMuted || clip == null || audioSource == null) return;
            audioSource.PlayOneShot(clip);
        }
    }
}
