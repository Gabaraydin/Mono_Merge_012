using UnityEngine;

namespace MonoMerge.VFX
{
    /// <summary>
    /// GDD 2: "Birlesme anlarinda partikul efektleri (ufak siyah noktalarin sacilmasi)."
    /// Attach to a prefab whose ParticleSystem is preconfigured in the Inspector (small
    /// black/dark-gray dot burst using Assets/Sprites/particle_dot.png as the render texture,
    /// tinted via Start Color — the sprite itself stays white so it tints cleanly). This script
    /// only plays the burst and cleans itself up, so MergeManager never manages particle
    /// lifetimes directly.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class MergeParticleEffect : MonoBehaviour
    {
        private ParticleSystem burst;

        private void Awake()
        {
            burst = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            burst.Play();
            float lifetime = burst.main.duration + burst.main.startLifetime.constantMax;
            Destroy(gameObject, lifetime);
        }

        /// <summary>Convenience factory used by MergeManager — spawns the prefab at a world
        /// position and lets it play/clean itself up. No-ops if prefab is unassigned.</summary>
        public static void SpawnAt(MergeParticleEffect prefab, Vector3 position)
        {
            if (prefab == null) return;
            Instantiate(prefab, position, Quaternion.identity);
        }
    }
}
