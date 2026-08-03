using UnityEngine;
using UnityEngine.UI;
using MonoMerge.Audio;
using MonoMerge.Core;

namespace MonoMerge.UI
{
    /// <summary>
    /// GDD 4: the only persisted player preference besides high score is the mute toggle.
    /// Deliberately just one control — no volume sliders, no options-menu sprawl.
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Toggle muteToggle;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            if (closeButton != null) closeButton.onClick.AddListener(Close);
            if (muteToggle != null) muteToggle.onValueChanged.AddListener(HandleMuteToggled);
        }

        private void OnEnable()
        {
            if (muteToggle != null && SaveManager.Instance != null)
            {
                // SetIsOnWithoutNotify avoids re-firing HandleMuteToggled while syncing UI state.
                muteToggle.SetIsOnWithoutNotify(SaveManager.Instance.IsMuted);
            }
        }

        private void HandleMuteToggled(bool isMuted)
        {
            SaveManager.Instance?.SetMuted(isMuted);
            AudioManager.Instance?.SetMuted(isMuted);
        }

        private void Close()
        {
            if (root != null) root.SetActive(false);
            else gameObject.SetActive(false);
        }
    }
}
