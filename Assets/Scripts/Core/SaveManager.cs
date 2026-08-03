using UnityEngine;

namespace MonoMerge.Core
{
    /// <summary>
    /// GDD 4: "Veri Saklama: Yuksek skorlar (High Score) ve oyuncunun sesi kapatma tercihleri
    /// yerel olarak (PlayerPrefs) kaydedilecektir. Backend/Veritabani kullanilmayacaktir."
    /// This is the ONLY class in the project allowed to touch PlayerPrefs directly, so the two
    /// persisted keys stay easy to find, rename, or migrate later.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private const string HighScoreKey = "MonoMerge.HighScore";
        private const string MutedKey = "MonoMerge.Muted";

        public static SaveManager Instance { get; private set; }

        public int HighScore { get; private set; }
        public bool IsMuted { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Load()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            IsMuted = PlayerPrefs.GetInt(MutedKey, 0) == 1;
        }

        /// <summary>Persists only if the new score actually beats the stored one. Returns
        /// whether a new high score was set, so UI (Week 3) can show a "new record" state.</summary>
        public bool TryUpdateHighScore(int score)
        {
            if (score <= HighScore) return false;

            HighScore = score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
            return true;
        }

        public void SetMuted(bool muted)
        {
            IsMuted = muted;
            PlayerPrefs.SetInt(MutedKey, muted ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
