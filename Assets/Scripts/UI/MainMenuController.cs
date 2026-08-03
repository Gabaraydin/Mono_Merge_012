using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MonoMerge.Core;

namespace MonoMerge.UI
{
    /// <summary>
    /// GDD 6 (Son Not): "Oyuncu uygulamayi actiginda 3 saniye icinde oynamaya baslayabilmelidir.
    /// Karmasik egitim ekranlarina (tutorial) gerek yoktur." Menu is deliberately minimal:
    /// title, high score, one Play button, one Settings button — no onboarding flow at all.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI highScoreLabel;
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private GameObject settingsPanelRoot;

        private void Awake()
        {
            if (playButton != null) playButton.onClick.AddListener(HandlePlayClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(HandleSettingsClicked);
        }

        private void Start()
        {
            RefreshHighScore();
        }

        private void RefreshHighScore()
        {
            int highScore = SaveManager.Instance != null ? SaveManager.Instance.HighScore : 0;
            if (highScoreLabel != null) highScoreLabel.text = highScore.ToString("N0");
        }

        private void HandlePlayClicked()
        {
            gameObject.SetActive(false);
            GameManager.Instance?.StartGame();
        }

        private void HandleSettingsClicked()
        {
            if (settingsPanelRoot != null) settingsPanelRoot.SetActive(true);
        }
    }
}
