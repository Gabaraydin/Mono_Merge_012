using System.Collections;
using UnityEngine;

namespace MonoMerge.VFX
{
    /// <summary>
    /// GDD 2: "ekran sarsintisi (camera shake) ile oyuncuya maksimum 'tatmin' hissi
    /// verilmelidir (Juiciness)." Simple additive positional jitter around the camera's own
    /// starting local position — no external shake library needed for a hyper-casual game.
    /// Attach directly to the Main Camera GameObject.
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        [SerializeField] private float defaultDuration = 0.15f;
        [SerializeField] private float defaultMagnitude = 0.1f;

        private Vector3 originalLocalPosition;
        private Coroutine activeShake;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            originalLocalPosition = transform.localPosition;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Shake() => Shake(defaultDuration, defaultMagnitude);

        public void Shake(float duration, float magnitude)
        {
            if (activeShake != null) StopCoroutine(activeShake);
            activeShake = StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                Vector2 offset = Random.insideUnitCircle * magnitude;
                transform.localPosition = originalLocalPosition + (Vector3)offset;
                yield return null;
            }

            transform.localPosition = originalLocalPosition;
            activeShake = null;
        }
    }
}
