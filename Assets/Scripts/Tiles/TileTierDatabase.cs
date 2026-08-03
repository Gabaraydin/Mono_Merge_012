using System;
using UnityEngine;

namespace MonoMerge.Tiles
{
    /// <summary>
    /// GDD 1 &amp; 2: numbered tiles (1, 2, 3, ...) that merge upward, rendered in the
    /// black/white/neutral-gray palette that is the game's whole visual identity.
    /// Designers tune tiers by editing the asset in the Inspector — no tier data in code.
    /// </summary>
    [CreateAssetMenu(fileName = "TileTierDatabase", menuName = "MonoMerge/Tile Tier Database")]
    public class TileTierDatabase : ScriptableObject
    {
        [Serializable]
        public class TierDefinition
        {
            [Tooltip("1-based tier number, shown on the tile (GDD: uc adet \"1\" birlesip bir adet \"2\" olur).")]
            public int tier;
            public string label;
            [Tooltip("Monochrome only per Art Direction (GDD 2) — dark gray/black tones, no color.")]
            public Color color = Color.black;
        }

        [Tooltip("Ordered ascending by tier. Index 0 = tier 1, the lowest spawnable tile.")]
        [SerializeField] private TierDefinition[] tiers;

        [Tooltip("How many of the lowest tiers the spawner may hand out (GDD: rastgele 2 veya 3 sekil).")]
        [SerializeField] private int spawnableTierCount = 3;

        public int TierCount => tiers != null ? tiers.Length : 0;
        public int SpawnableTierCount => Mathf.Min(spawnableTierCount, TierCount);

        public TierDefinition GetTier(int tier)
        {
            int index = tier - 1;
            if (tiers == null || index < 0 || index >= tiers.Length) return null;
            return tiers[index];
        }

        public bool HasNextTier(int tier) => tier < TierCount;
    }
}
